using System;
using DG.Tweening;
using General;
using Items;
using Player;
using RamenSea.Foundation.Extensions;
using RamenSea.Foundation3D.Components.Audio;
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
        protected static readonly int ANIMATOR_ATTACK = Animator.StringToHash("Attack");
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
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected PlayerCollisionDetection playerCollisionDetection;
        [SerializeField] protected Animator animator;
        [SerializeField] protected ThoughtBubble thoughtBubble;
        [SerializeField] protected VariationAudioSource entranceSound;
        [SerializeField] protected VariationAudioSource gettingMadSound;
        [SerializeField] protected VariationAudioSource wantPotionSound;
        [SerializeField] protected VariationAudioSource wantBookSound;
        [SerializeField] protected VariationAudioSource wantSwordSound;
        [SerializeField] protected VariationAudioSource thanksSound;
        [SerializeField] protected Collider2D collider;

        protected Rigidbody2D r2d; //todo


        protected float requestTimer = 0f;
        protected BaseHeroState _state;

        protected Item _requestItem;
        protected Vector2 spawnLocation;
        protected Vector2 requestLocation;
        protected HeroSpawn spawnInfo;

        public Item requestItem => this._requestItem;
        public int gameId { private set; get; }

        public float timeTilMad => this.requestTimer;
        public BaseHeroState state => this._state;

        public Action<BaseHero> onStateChange;
        // Unity Lifecycle methods
        // Stubbed out a few of the common mono behavior life cycle classes to make it easier work in the future
        // We can remove these if we don't end up using em
        protected virtual void Awake() {
            r2d = GetComponent<Rigidbody2D>();
            this._state = BaseHeroState.NotSpawned;
            this.progressBar.gameObject.SetActive(false);
            this.playerCollisionDetection.listener = this;
            this.collider.enabled = false;
        }
        protected virtual void Start() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

        protected virtual void Update() {
            if (this.runner.status != GameRunner.Status.Running) {
                return;
            }

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
                    // this.progressBar.SetProgress((this.generalRequestTime - this.requestTimer) / this.generalRequestTime);
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
        public void ResolveRequest() {
            if(this.state != BaseHeroState.WaitingForRequest) {
                return;
            }
            this.MoveState(BaseHeroState.Leaving);
        }
        public virtual void OnSpawn() {
        }
        public virtual void SetUp(HeroSpawn spawn, Vector2 spawnLocation, Vector2 requestLocation) {
            this.spawnInfo = spawn;
            this.gameId = spawn.id;
            this._requestItem = spawn.requestItem;
            this.requestTimer = spawn.requestTime;
            this.spawnLocation = spawnLocation;
            this.requestLocation = requestLocation;
            this.transform.position = spawnLocation;
            // this.transform.LookAt(this.requestLocation); //todo rotate the character towards the vector
            this.MoveState(BaseHeroState.WalkingIn);
        }

        protected virtual void MoveState(BaseHeroState newState) {
            var oldState = this.state;
            this._state = newState;

            this.spriteRenderer.DOKill();
            switch (oldState) { //old state
                case BaseHeroState.WaitingForRequest: {
                    this.progressBar.gameObject.SetActive(false);
                    break;
                }
            }
            
            
            switch (this.state) {
                case BaseHeroState.WalkingIn: {
                    this.entranceSound.Play();
                    this.thoughtBubble.thoughtBubbleAnimation = ThoughtBubbleAnimation.None;
                    if (this.r2d != null) {
                        this.r2d.bodyType = RigidbodyType2D.Kinematic;
                    }
                    break;
                }
                case BaseHeroState.WaitingForRequest: {
                    switch (this.requestItem) {
                        case Item.Potion: {
                            this.wantPotionSound.Play();
                            break;
                        }
                        case Item.SpellBook: {
                            this.wantBookSound.Play();
                            break;
                        }
                        case Item.Sword: {
                            this.wantSwordSound.Play();
                            break;
                        }
                    }
                    this.thoughtBubble.SetAnimationWithTime(this.requestItem, this.spawnInfo.requestTime);
                    // this.progressBar.gameObject.SetActive(true);
                    // this.progressBar.SetProgress(0f);
                    this.requestTimer = this.spawnInfo.requestTime;
                    this.SetMovementAnimation(Vector2.zero);
                    break;
                }
                case BaseHeroState.Mad: {
                    this.collider.enabled = true;
                    this.gettingMadSound.Play();
                    this.thoughtBubble.thoughtBubbleAnimation = ThoughtBubbleAnimation.Mad;
                    if (this.r2d != null) {
                        this.r2d.bodyType = RigidbodyType2D.Dynamic;
                    }
                    
                    this.runner.levelController.HeroStatusChange(this.gameId, true); // ooof this is bad to do lol
                    break;
                }
                case BaseHeroState.Leaving: {
                    this.thanksSound.Play();
                    this.thoughtBubble.thoughtBubbleAnimation = ThoughtBubbleAnimation.Happy;
                    
                    this.runner.levelController.HeroStatusChange(this.gameId, false); // ooof this is bad to do lol
                    break;
                }
                case BaseHeroState.Left: {
                    this.recycler.Recycle(this);
                    this.ClearAnimator();
                    break;
                }
            }
            
            this.onStateChange?.Invoke(this);
        }

        //Animation stuff
        protected void ClearAnimator() {
            this.thoughtBubble.thoughtBubbleAnimation = ThoughtBubbleAnimation.None;
            this.animator.SetBool(ANIMATOR_ATTACK, false);
            this.animator.SetInteger(ANIMATOR_HORIZONTAL_MOVE, 0);
            this.animator.SetInteger(ANIMATOR_VERTICAL_MOVE, 0);
        }
        protected void SetMovementAnimation(Vector2 move) {
            var isHorizontalMovement = Mathf.Approximately(move.x, 0) == false;
            var isVerticalMovement = Mathf.Approximately(move.y, 0) == false;
            if (isHorizontalMovement && isVerticalMovement) {
                if (move.x.Abs() > move.y.Abs() * 0.4f) {
                    isVerticalMovement = false;
                } else {
                    isHorizontalMovement = false;
                }
            }
            if (isHorizontalMovement == false) {
                this.animator.SetInteger(ANIMATOR_HORIZONTAL_MOVE, 0);
            } else {
                this.animator.SetInteger(ANIMATOR_HORIZONTAL_MOVE, move.x >= 0 ? 1 : -1);
            }
            if (isVerticalMovement == false) {
                this.animator.SetInteger(ANIMATOR_VERTICAL_MOVE, 0);
            } else {
                this.animator.SetInteger(ANIMATOR_VERTICAL_MOVE, move.y >= 0 ? 1 : -1);
            }
        }
        
        
        
        public virtual void OnPlayerTriggerEnter2D(PlayerController player, PlayerCollisionDetection detection, Collider2D other) {
            if (detection == this.playerCollisionDetection) {
                player.AddSelectedHero(this);
            }
        }
        public virtual void OnPlayerTriggerExit2D(PlayerController player, PlayerCollisionDetection detection, Collider2D other) {
            if (detection == this.playerCollisionDetection) {
                player.RemoveSelectedHero(this);
            }
        }
        protected virtual void OnDestroy() {
            this.spriteRenderer.DOKill();
        }
    }
}