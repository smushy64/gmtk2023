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
        [SerializeField] private float movementSpeed;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private GameObject heldItem;
        [SerializeField] private SpriteRenderer spriteRenderer;

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

            this.heldItem.SetActive(false);
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
                    const int MAX_ITEM_STATION_COLLIDERS = 1;
                    Collider2D[] colliders =
                        new Collider2D[MAX_ITEM_STATION_COLLIDERS];

                    int num_colliders = Physics2D.OverlapCircleNonAlloc(
                        transform.position,
                        2.0f,
                        colliders,
                        item_station_mask
                    );

                    if( num_colliders != 0 ) {
                        Collider2D collider = colliders[0];
                        ItemStation station =
                            collider.gameObject.GetComponent<ItemStation>();
                        item = station.on_interact();
                    }
                    if( item != Item.None ) {
                        held_item_type = item;
                        heldItem.SetActive( true );
                    }
                } else {
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
                                bestHeroToSelect = possibleSelectedHero;
                            }
                        }

                        if( bestHeroToSelect != null ) {
                            bestHeroToSelect.GiveItem();
                            held_item_type = Item.None;
                            heldItem.SetActive( false );
                        }
                    }
                }
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

        public void SetSelectedItemBuilderArea(BaseItemBuilder builder) {
            // this.selectedBuilder = builder;
        }
        public void ClearSelectedItemBuilderArea(BaseItemBuilder builder) {
            // if (this.selectedBuilder == builder) {
            //     this.selectedBuilder = null;
            // }
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