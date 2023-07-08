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
        [SerializeField] private GameObject potionInHand;
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

        private BaseItemBuilder selectedBuilder;
        private bool isInteractingWithBuilder = false;

        private List<BaseHero> possibleSelectedHeroes;

        private bool isHoldingPotion = false;
        
        private void Awake() {
            r2d = GetComponent<Rigidbody2D>();
            mapped_input = new MappedInput();

            this.potionInHand.SetActive(false);
            this.possibleSelectedHeroes = new();
        }

        private void Update() {

            if (this.runner.state.status != GameState.Status.Running) {
                return;
            }
            
            poll_input();
            if (selectedBuilder != null) {
                this.selectedBuilder.ProcessInput(player_input);
                if(
                    selectedBuilder.itemIsFinished &&
                    player_input.is_interact_pressed
                ) {
                    this.selectedBuilder.TakeItem();
                    this.potionInHand.SetActive(true);
                    this.isHoldingPotion = true;
                }
                this.isInteractingWithBuilder = this.selectedBuilder.isBeingInteractedWith;
            }

            if( isHoldingPotion && player_input.is_interact_pressed ) {
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

                    if (bestHeroToSelect != null) {
                        bestHeroToSelect.GiveItem();
                        this.isHoldingPotion = false;
                        this.potionInHand.SetActive(false);
                    }
                }
            }
        }

        private void FixedUpdate() {
            if (this.runner.state.status != GameState.Status.Running) {
                return;
            }

            if( !isInteractingWithBuilder ) {
                Vector2 movementDelta =
                    player_input.movement * (movementSpeed * Time.fixedDeltaTime);
                this.r2d.MovePosition(this.transform.position.ToVector2() + movementDelta);
            }
        }

        public void SetSelectedItemBuilderArea(BaseItemBuilder builder) {
            this.selectedBuilder = builder;
        }
        public void ClearSelectedItemBuilderArea(BaseItemBuilder builder) {
            if (this.selectedBuilder == builder) {
                this.selectedBuilder = null;
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