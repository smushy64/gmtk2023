using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Heroes;
using Items;
using RamenSea.Foundation3D.Extensions;
using Runner;
using UnityEngine;

namespace Player {

    public struct Input {
        public Vector2 movement;
        public bool is_interact_down;
        public bool is_interact_pressed;
        public bool is_interact_released;

        public bool is_discard_down;
        public bool is_discard_pressed;
        public bool is_discard_released;

        public Input(
            Vector2 movement,
            bool interact_down,
            bool interact_pressed,
            bool interact_released,
            bool discard_down,
            bool discard_pressed,
            bool discard_released
        ) {
            this.movement = movement;
            this.is_interact_down = interact_down;
            this.is_interact_pressed = interact_pressed;
            this.is_interact_released = interact_released;
            is_discard_down = discard_down;
            is_discard_pressed = discard_pressed;
            is_discard_released = discard_released;
        }
    };

    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController: GameMechanic {
        private static readonly int ANIMATOR_HORIZONTAL_MOVE = Animator.StringToHash("HorizontalMove");
        private static readonly int ANIMATOR_VERTICAL_MOVE = Animator.StringToHash("VerticalMove");
        private static readonly int ANIMATOR_DEAD = Animator.StringToHash("IsDead");
        
        [SerializeField] private float movementSpeed;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;

        [SerializeField]
        Items.ItemObject heldItem;

        [SerializeField, Min(1)]
        int maxHealth = 5;

        public int health { private set; get; }
        public int max_health => maxHealth;

        public Action<int, int> on_health_update;
        public Action<Item> on_hold_item;
        public Action on_stop_hold_item;

        bool last_is_moving = false;
        bool is_moving      = false;

        // NOTE(alicia): i-frames
        public bool is_invincible { private get; set; } = false;
        [SerializeField, Min(0.0f)]
        float iFrameTime = 1.0f;

        // NOTE(alicia): moved input handling into playercontroller
        public Player.Input player_input { private set; get; }
        MappedInput mapped_input;

        // NOTE(alicia): unity was complaining about
        // hiding rigidbody inherited member so i renamed
        // rigidbody to r2d.
        // i also think it's better to load it at runtime
        // since it's part of the gameobject anyway
        Rigidbody2D r2d;
        public Transform targetTransform => _targetTransform;

        Item held_item_type = Item.None;

        private List<BaseHero> possibleSelectedHeroes;

        private void Awake() {
            r2d = GetComponent<Rigidbody2D>();
            mapped_input = new MappedInput();

            heldItem.gameObject.SetActive(false);
            this.possibleSelectedHeroes = new();
        }

        void Start() {
            health = max_health;
        }

        private void Update() {

            if( mapped_input.Character.Pause.WasPressedThisFrame() ) {
                runner.player_pause();
                return;
            }

            if( runner.status != GameRunner.Status.Running ) {
                // NOTE(alicia): maybe hash these animation names
                // if there's time later
                animator.Play( "Idle" );
                return;
            }

            last_is_moving = is_moving;
            poll_input();
            if( player_input.is_interact_pressed ) {
                if( held_item_type == Item.None ) {
                    Item item = Item.None;
                    LayerMask item_station_mask = (1 << 3);
                    const int MAX_ITEM_STATION_COLLIDERS = (int)Item.MAX;
                    Collider2D[] colliders =
                        new Collider2D[MAX_ITEM_STATION_COLLIDERS];

                    int num_colliders = Physics2D.OverlapCircleNonAlloc(
                        transform.position,
                        2.0f,
                        colliders,
                        item_station_mask
                    );

                    if( num_colliders != 0 ) { 
                        // NOTE(alicia): pick the collider that is
                        // closest to the player
                        
                        //Moved this into the array size check
                        Collider2D collider = colliders[0];
                        float shortest_distance =
                            (collider.transform.position - transform.position)
                            .sqrMagnitude;
                        for( int i = 1; i < num_colliders; ++i ) {
                            float distance = 
                                (colliders[i].transform.position - transform.position)
                                .sqrMagnitude;
                            if( distance < shortest_distance ) {
                                shortest_distance = distance;
                                collider = colliders[i];
                            }
                        }
                        
                        ItemStation station =
                            collider.gameObject.GetComponent<ItemStation>();
                        item = station.on_interact();
                    }
                    if( item != Item.None ) {
                        held_item_type = item;
                        heldItem.gameObject.SetActive( true );
                        heldItem.set_sprite( held_item_type );
                        on_hold_item?.Invoke( held_item_type );
                    }
                }
                
                if (this.possibleSelectedHeroes.Count > 0) {
                    BaseHero bestHeroToSelect = null;
                    for (var i = 0; i < this.possibleSelectedHeroes.Count; i++) {
                        var possibleSelectedHero = this.possibleSelectedHeroes[i];
                        if (possibleSelectedHero.state != BaseHeroState.WaitingForRequest || possibleSelectedHero.requestItem != this.held_item_type) {
                            continue;
                        }

                        if (bestHeroToSelect == null) {
                            bestHeroToSelect = possibleSelectedHero;
                        } else if (possibleSelectedHero.timeTilMad < bestHeroToSelect.timeTilMad) {
                            bestHeroToSelect = possibleSelectedHero;
                        }
                    }

                    if(bestHeroToSelect != null) {
                        held_item_type = Item.None;
                        heldItem.gameObject.SetActive( false );
                        on_stop_hold_item?.Invoke();
                        bestHeroToSelect.ResolveRequest();
                    }
                }
            }

            if( player_input.is_discard_pressed ) {
                held_item_type = Item.None;
                heldItem.gameObject.SetActive( false );
                on_stop_hold_item?.Invoke();
            }

            // NOTE(alicia): mmm performance

            if( !is_moving ) {
                animator.Play( "Idle" );
            } else {
                bool moving_vertically = 
                    Mathf.Abs( player_input.movement.x ) <
                    Mathf.Abs( player_input.movement.y );

                if( moving_vertically ) {
                    if( player_input.movement.y > 0f ) {
                        animator.Play( "MoveUp" );
                    } else {
                        animator.Play( "MoveDown" );
                    }
                } else {
                    if( player_input.movement.x < 0f ) {
                        animator.Play( "MoveLeft" );

                    } else {
                        animator.Play( "MoveRight" );
                    }
                }
            }

        }

        private void FixedUpdate() {
            if( runner.status != GameRunner.Status.Running ) {
                return;
            }

            Vector2 movementDelta =
                player_input.movement * (movementSpeed * Time.fixedDeltaTime);
            this.r2d.MovePosition(this.transform.position.ToVector2() + movementDelta);
        }

        public override void OnStateChange(
            GameRunner.Status status,
            GameRunner.LevelResult level_result
        ) {
            base.OnStateChange(status, level_result);
            switch( status ) {
                case GameRunner.Status.SetUp: {
                    //clear the animator just cuz
                    this.animator.SetInteger(ANIMATOR_HORIZONTAL_MOVE, 0);
                    this.animator.SetInteger(ANIMATOR_VERTICAL_MOVE, 0);
                    this.animator.SetBool(ANIMATOR_DEAD, false);
                    break;
                }
                case GameRunner.Status.Running: {

                    break;
                }
                case GameRunner.Status.End: {
                    this.animator.SetInteger(ANIMATOR_HORIZONTAL_MOVE, 0);
                    this.animator.SetInteger(ANIMATOR_VERTICAL_MOVE, 0);
                    this.animator.SetBool(ANIMATOR_DEAD, true);
                    break;
                }
            }
        }

        public void AddSelectedHero(BaseHero hero) {
            if (this.possibleSelectedHeroes.Contains(hero) == false) {
                this.possibleSelectedHeroes.Add(hero);
            }
        }
        public void RemoveSelectedHero(BaseHero hero) {       
            this.possibleSelectedHeroes.Remove(hero);

        }

        IEnumerator iinvincibility_frames;
        IEnumerator invincibility_frames() {
            float timer = 0f;
            float blink_timer = 0f;
            float blink_time = iFrameTime / 10.0f;
            bool blinking = false;
            while( timer < iFrameTime ) {
                timer += Time.deltaTime;
                blink_timer += Time.deltaTime;
                if( blink_timer >= blink_time ) {
                    blinking = !blinking;
                    spriteRenderer.color = blinking ? Color.clear : Color.white;
                    blink_timer = 0f;
                }
                yield return null;
            }
            spriteRenderer.color = Color.white;
            is_invincible = false;
        }

        public void TakeDamage(int damage) { 
            if( is_invincible ) {
                return;
            }
            health -= damage;
            is_invincible = true;

            if( iinvincibility_frames != null ) {
                this.StopCoroutine( iinvincibility_frames );
            }

            iinvincibility_frames = invincibility_frames();
            this.StartCoroutine( iinvincibility_frames );

            if( on_health_update != null ) {
                on_health_update.Invoke( health, maxHealth );
            }
            if( health <= 0 ) {
                this.runner.PlayerDied();
                return;
            }

        }

        private void OnDestroy() {
            this.spriteRenderer.DOKill();
        }

        void poll_input() {
            player_input = new Input(
                mapped_input.Character.Movement.ReadValue<Vector2>(),
                mapped_input.Character.Action.IsPressed(),
                mapped_input.Character.Action.WasPressedThisFrame(),
                mapped_input.Character.Action.WasReleasedThisFrame(),
                mapped_input.Character.DiscardItem.IsPressed(),
                mapped_input.Character.DiscardItem.WasPressedThisFrame(),
                mapped_input.Character.DiscardItem.WasReleasedThisFrame()
            );

            is_moving = player_input.movement.sqrMagnitude >= 0.001f;
        }

        void OnEnable() {
            mapped_input.Enable();
        }
        void OnDisable() {
            mapped_input.Disable();
        }
    }
}