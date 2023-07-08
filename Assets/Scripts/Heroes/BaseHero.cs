using System;
using DG.Tweening;
using General;
using Items;
using Player;
using RamenSea.Foundation.Extensions;
using RamenSea.Foundation3D.Extensions;
using Runner;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Heroes {
    public enum HeroType: byte {
        None, Link, Karen, Gremlin, Merlin
    }

    public static class HeroTypeExtensions {
        public static int GetPrefabHashCode(this HeroType heroType, byte variant) {
            return ((int)heroType) * 10000 + variant.ToInt(); //oof
        }
    }

    public enum BaseHeroState {
        NotSpawned, WalkingIn, WaitingForRequest, Leaving, Mad, Left
    }
    public class BaseHero: MonoBehaviour, IPlayerCollisionDetectionListener {
        protected static readonly int ANIMATOR_IS_MAD = Animator.StringToHash("IsMad");
        protected static readonly int ANIMATOR_HORIZONTAL_MOVE = Animator.StringToHash("HorizontalMove");
        protected static readonly int ANIMATOR_VERTICAL_MOVE = Animator.StringToHash("VerticalMove");
        
        // Enum specifying the type of hero the subclass is
        public virtual HeroType heroType => HeroType.None;
        // Allows you to get the variant enum without having to cast to a class
        public virtual byte heroVariantValue => 0;
        
        [NonSerialized] public HeroRecycler recycler; // yay circlular
        [NonSerialized] public GameRunner runner;
        
        // Editor fields
        [SerializeField] protected GameProgressBar progressBar;
        [SerializeField] protected float walkingInSpeed = 1f;
        [SerializeField] protected float leavingSpeed = 1f;
        [SerializeField] protected float generalRequestTime = 5f;
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected PlayerCollisionDetection playerCollisionDetection;
        [SerializeField] protected Animator animator;
        [SerializeField] protected Rigidbody2D rigidbody; //todo


        protected float requestTimer = 0f;
        protected BaseHeroState _state;

        protected Item _requestItem;
        protected Vector2 spawnLocation;
        protected Vector2 requestLocation;

        public Item requestItem => this._requestItem;
        public float timeTilMad => this.requestTimer;
        public BaseHeroState state => this._state;

        // Unity Lifecycle methods
        // Stubbed out a few of the common mono behavior life cycle classes to make it easier work in the future
        // We can remove these if we don't end up using em
        protected virtual void Awake() {
            this._state = BaseHeroState.NotSpawned;
            this.progressBar.gameObject.SetActive(false);
            this.playerCollisionDetection.listener = this;
        }
        protected virtual void Start() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

        protected virtual void Update() {
            var position = this.transform.position;
            switch (this.state) {
                case BaseHeroState.WalkingIn: {
                    var updatedPosition = Vector3.MoveTowards(position, this.requestLocation.ToVector3(position.y),
                                                   this.walkingInSpeed * Time.deltaTime);
                    var moveSpeed = updatedPosition - position;
                    position = updatedPosition;
                    if (Mathf.Approximately(position.x, this.requestLocation.x) &&
                        Mathf.Approximately(position.y, this.requestLocation.y)) {
                        this.transform.position = this.requestLocation;
                        this.MoveState(BaseHeroState.WaitingForRequest);
                    } else {
                        this.transform.position = position;
                        this.SetMovementAnimation(moveSpeed);
                    }
                    break;
                }
                case BaseHeroState.WaitingForRequest: {
                    this.requestTimer -= Time.deltaTime;
                    this.progressBar.SetProgress((this.generalRequestTime - this.requestTimer) / this.generalRequestTime);
                    if (this.requestTimer <= 0f) {
                        this.MoveState(BaseHeroState.Mad);
                    }
                    break;
                }
                case BaseHeroState.Leaving: {
                    var updatedPosition = Vector3.MoveTowards(position, this.spawnLocation.ToVector3(position.y),
                                                              this.leavingSpeed * Time.deltaTime);
                    var moveSpeed = updatedPosition - position;
                    position = updatedPosition;
                    
                    if (Mathf.Approximately(position.x, this.spawnLocation.x) &&
                        Mathf.Approximately(position.y, this.spawnLocation.y)) {
                        this.transform.position = this.spawnLocation;
                        this.MoveState(BaseHeroState.Left);
                    } else {
                        this.transform.position = position;
                        this.SetMovementAnimation(moveSpeed);
                    }
                    break;
                }
            }
            
        }
        protected virtual void FixedUpdate() {
            // switch (this.state) {
            //     case BaseHeroState.WalkingIn:
            // }
        }
        // End of unity life cycle methods
        public bool GiveItem( Item item ) {
            if(
                this.state != BaseHeroState.WaitingForRequest ||
                requestItem != item
            ) {
                return false;
            }
            this.MoveState(BaseHeroState.Leaving);
            return true;
        }
        public virtual void OnSpawn() {
            this.spriteRenderer.color = Color.white;
        }
        public virtual void SetUp(Item requestingItem, Vector2 spawnLocation, Vector2 requestLocation) {
            this._requestItem = requestingItem;
            this.spawnLocation = spawnLocation;
            this.requestLocation = requestLocation;
            this.transform.position = spawnLocation;
            // this.transform.LookAt(this.requestLocation); //todo rotate the character towards the vector
            this.MoveState(BaseHeroState.WalkingIn);
        }

        protected virtual void MoveState(BaseHeroState newState) {
            var oldState = this.state;
            this._state = newState;
            Debug.Log($"{this.heroType} - is moving to {newState} from {oldState}"); //todo remove

            this.spriteRenderer.DOKill();
            switch (oldState) { //old state
                case BaseHeroState.WaitingForRequest: {
                    this.progressBar.gameObject.SetActive(false);
                    break;
                }
            }
            
            
            switch (this.state) {
                case BaseHeroState.WaitingForRequest: {
                    this.progressBar.gameObject.SetActive(true);
                    this.progressBar.SetProgress(0f);
                    this.requestTimer = this.generalRequestTime; // todo actually have this set a usable time. JUST A TEST VALUE
                    this.SetMovementAnimation(Vector2.zero);
                    break;
                }
                case BaseHeroState.Mad: {
                    this.spriteRenderer.DOColor(new Color(0.95f, 0.29f, 0.31f), 0.4f);
                    break;
                }
                case BaseHeroState.Leaving: {
                    this.spriteRenderer.DOColor(new Color(0.2f, 0.94f, 0.95f), 0.4f);
                    break;
                }
                case BaseHeroState.Left: {
                    this.recycler.Recycle(this);
                    this.ClearAnimator();
                    break;
                }
            }

        }

        //Animation stuff
        protected void ClearAnimator() {
            this.animator.SetBool(ANIMATOR_IS_MAD, false);
            this.animator.SetInteger(ANIMATOR_HORIZONTAL_MOVE, 0);
            this.animator.SetInteger(ANIMATOR_VERTICAL_MOVE, 0);
        }
        protected void SetMovementAnimation(Vector2 move) {
            if (Mathf.Approximately(move.x, 0)) {
                this.animator.SetInteger(ANIMATOR_HORIZONTAL_MOVE, 0);
            } else {
                this.animator.SetInteger(ANIMATOR_HORIZONTAL_MOVE, move.x >= 0 ? 1 : -1);
            }
            if (Mathf.Approximately(move.y, 0)) {
                this.animator.SetInteger(ANIMATOR_VERTICAL_MOVE, 0);
            } else {
                this.animator.SetInteger(ANIMATOR_VERTICAL_MOVE, move.y >= 0 ? 1 : -1);
            }
        }
        
        
        
        public void OnPlayerTriggerEnter2D(PlayerController player, Collider2D other) {
            player.AddSelectedHero(this);
        }
        public void OnPlayerTriggerExit2D(PlayerController player, Collider2D other) {
            player.RemoveSelectedHero(this);
        }
        protected virtual void OnDestroy() {
            this.spriteRenderer.DOKill();
        }
    }
}