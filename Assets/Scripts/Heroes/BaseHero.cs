using System;
using Items;
using RamenSea.Foundation.Extensions;
using RamenSea.Foundation3D.Extensions;
using Runner;
using Unity.VisualScripting;
using UnityEngine;

namespace Heroes {
    public enum HeroType: byte {
        None, Link, Karen, Witcher, Merlin
    }

    public static class HeroTypeExtensions {
        public static int GetPrefabHashCode(this HeroType heroType, byte variant) {
            return ((int)heroType) * 10000 + variant.ToInt(); //oof
        }
    }

    public enum BaseHeroState {
        NotSpawned, WalkingIn, WaitingForRequest, Leaving, Mad, Left
    }
    public class BaseHero: MonoBehaviour {
        // Enum specifying the type of hero the subclass is
        public virtual HeroType heroType => HeroType.None;
        // Allows you to get the variant enum without having to cast to a class
        public virtual byte heroVariantValue => 0;
        
        [DoNotSerialize] public GameRunner runner;
        
        // Editor fields
        [SerializeField] protected float walkingInSpeed = 1f;
        [SerializeField] protected float leavingSpeed = 1f;

        protected float requestTimer = 0f;
        protected BaseHeroState state;

        protected Item _requestItem;
        protected Vector2 spawnLocation;
        protected Vector2 requestLocation;
        
        public Item requestItem => this._requestItem;

        // Unity Lifecycle methods
        // Stubbed out a few of the common mono behavior life cycle classes to make it easier work in the future
        // We can remove these if we don't end up using em
        protected virtual void Awake() {
            this.state = BaseHeroState.NotSpawned;
        }
        protected virtual void Start() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

        protected virtual void Update() {
            var position = this.transform.position;
            switch (this.state) {
                case BaseHeroState.WalkingIn: {
                    position = Vector3.MoveTowards(position, this.requestLocation.ToVector3(position.y),
                                                   this.walkingInSpeed * Time.deltaTime);
                    if (Mathf.Approximately(position.x, this.requestLocation.x) &&
                        Mathf.Approximately(position.y, this.requestLocation.y)) {
                        this.transform.position = this.requestLocation;
                        this.MoveState(BaseHeroState.WaitingForRequest);
                    } else {
                        this.transform.position = position;
                    }
                    break;
                }
                case BaseHeroState.WaitingForRequest: {
                    this.requestTimer -= Time.deltaTime;
                    if (this.requestTimer <= 0f) {
                        this.MoveState(BaseHeroState.Mad);
                    }
                    break;
                }
                case BaseHeroState.Leaving: {
                    position = Vector3.MoveTowards(position, this.spawnLocation.ToVector3(position.y),
                                                   this.leavingSpeed * Time.deltaTime);
                    if (Mathf.Approximately(position.x, this.spawnLocation.x) &&
                        Mathf.Approximately(position.y, this.spawnLocation.y)) {
                        this.transform.position = this.spawnLocation;
                        this.MoveState(BaseHeroState.Left);
                    } else {
                        this.transform.position = position;
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

        public virtual void SetUp(Item requestingItem, Vector2 spawnLocation, Vector2 requestLocation) {
            this._requestItem = requestingItem;
            this.spawnLocation = spawnLocation;
            this.requestLocation = requestLocation;
            this.transform.position = spawnLocation;
            // this.transform.LookAt(this.requestLocation); //todo rotate the character towards the vector
            this.MoveState(BaseHeroState.WalkingIn);
        }

        protected virtual void MoveState(BaseHeroState newState) {
            Debug.Log($"{this.heroType} - is moving to {newState} from {this.state}"); //todo remove

            this.state = newState;
            switch (this.state) {
                case BaseHeroState.WaitingForRequest: {
                    this.requestTimer = 2f; // todo actually have this set a usable time. JUST A TEST VALUE
                    break;
                }
            }

        }
    }
}