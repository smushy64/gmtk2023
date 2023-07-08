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

        public Input( Vector2 movement, bool interact_down, bool interact_pressed, bool interact_released ) {
            this.movement = movement;
            this.is_interact_down = interact_down;
            this.is_interact_pressed = interact_pressed;
            this.is_interact_released = interact_released;
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

        private void Update() {

            if (this.runner.state.status != GameState.Status.Running) {
                return;
            }

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
                        heldItem.set_sprite( held_item_type );
                        heldItem.gameObject.SetActive( true );
                    }
                }
                
                if (this.possibleSelectedHeroes.Count > 0) {
                    BaseHero bestHeroToSelect = null;
                    for (var i = 0; i < this.possibleSelectedHeroes.Count; i++) {
                        var possibleSelectedHero = this.possibleSelectedHeroes[i];
                        if (possibleSelectedHero.state != BaseHeroState.WaitingForRequest) {
                            continue;
                        }

                        if (bestHeroToSelect == null) {
                            bestHeroToSelect = possibleSelectedHero;
                        } else if (possibleSelectedHero.timeTilMad < bestHeroToSelect.timeTilMad) {
                            if (possibleSelectedHero.requestItem == this.held_item_type || possibleSelectedHero.requestsInteraction) {
                                bestHeroToSelect = possibleSelectedHero;
                            }
                        }
                    }

                    if(bestHeroToSelect != null) {
                        if (bestHeroToSelect.requestItem != Item.None) { // only clear out the held item if the hero is requesting an item
                            held_item_type = Item.None;
                            heldItem.gameObject.SetActive( false );
                        }
                        // Checks if the requested item is the same earlier
                        // That way it allows us to handle non item requests
                        bestHeroToSelect.ResolveRequest();
                    }
                }
            }

            if (Mathf.Approximately(this.player_input.movement.x, 0)) {
                this.animator.SetInteger(ANIMATOR_HORIZONTAL_MOVE, 0);
            } else {
                this.animator.SetInteger(ANIMATOR_HORIZONTAL_MOVE, this.player_input.movement.x >= 0 ? 1 : -1);
            }
            if (Mathf.Approximately(this.player_input.movement.y, 0)) {
                this.animator.SetInteger(ANIMATOR_VERTICAL_MOVE, 0);
            } else {
                this.animator.SetInteger(ANIMATOR_VERTICAL_MOVE, this.player_input.movement.y >= 0 ? 1 : -1);
            }
        }

        private void FixedUpdate() {
            if (this.runner.state.status != GameState.Status.Running) {
                return;
            }

            Vector2 movementDelta =
                player_input.movement * (movementSpeed * Time.fixedDeltaTime);
            this.r2d.MovePosition(this.transform.position.ToVector2() + movementDelta);
        }

        public override void OnStateChange(GameState state) {
            base.OnStateChange(state);
            switch (state.status) {
                case GameState.Status.SetUp: {
                    //clear the animator just cuz
                    this.animator.SetInteger(ANIMATOR_HORIZONTAL_MOVE, 0);
                    this.animator.SetInteger(ANIMATOR_VERTICAL_MOVE, 0);
                    this.animator.SetBool(ANIMATOR_DEAD, false);
                    break;
                }
                case GameState.Status.Running: {

                    break;
                }
                case GameState.Status.End: {
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
        public void TakeDamage(int damage) { // we could add source
            this.runner.PlayerDied();
            this.spriteRenderer.DOColor(Color.red, 0.4f);
        }

        private void OnDestroy() {
            this.spriteRenderer.DOKill();
        }

        void poll_input() {
            player_input = new Input(
                    mapped_input.Character.Movement.ReadValue<Vector2>(),
                    mapped_input.Character.Action.IsPressed(),
                    mapped_input.Character.Action.WasPressedThisFrame(),
                    mapped_input.Character.Action.WasReleasedThisFrame()
                    );
        }

        void OnEnable() {
            mapped_input.Enable();
        }
        void OnDisable() {
            mapped_input.Disable();
        }
    }
}