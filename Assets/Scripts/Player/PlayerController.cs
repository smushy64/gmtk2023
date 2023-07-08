using System;
using System.Collections.Generic;
using DG.Tweening;
using Heroes;
using Items;
using RamenSea.Foundation3D.Extensions;
using Runner;
using UnityEngine;

namespace Player {
    public class PlayerController: GameMechanic {
        [SerializeField] private PlayerInput input;
        [SerializeField] private float movementSpeed;
        [SerializeField] private Rigidbody2D rigidbody;
        [SerializeField] private Transform _targetTransform;
        [SerializeField] private GameObject potionInHand;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public Transform targetTransform => _targetTransform;

        private BaseItemBuilder selectedBuilder;
        private bool isInteractingWithBuilder = false;

        private List<BaseHero> possibleSelectedHeroes;

        private bool isHoldingPotion = false;
        
        private void Awake() {
            this.potionInHand.SetActive(false);
            this.possibleSelectedHeroes = new();
        }

        private void Update() {
            if (this.runner.state.status != GameState.Status.Running) {
                return;
            }
            
            this.input.UpdateInput();
            if (selectedBuilder != null) {
                this.selectedBuilder.ProcessInput(this.input);
                if (this.selectedBuilder.itemIsFinished && this.input.actionPressedThisFrame) {
                    this.selectedBuilder.TakeItem();
                    this.potionInHand.SetActive(true);
                    this.isHoldingPotion = true;
                }
                this.isInteractingWithBuilder = this.selectedBuilder.isBeingInteractedWith;
            }

            if (this.isHoldingPotion && this.input.actionPressedThisFrame) {
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

            if (this.isInteractingWithBuilder == false) {
                Vector2 movementDelta = this.input.movementVector * (this.movementSpeed * Time.fixedDeltaTime);
                this.rigidbody.MovePosition(this.transform.position.ToVector2() + movementDelta);
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
    }
}