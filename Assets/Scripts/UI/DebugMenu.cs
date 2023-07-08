using System;
using Runner;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    public class DebugMenu: GameMechanic {
        
        [SerializeField] private GameObject startButton;
        [SerializeField] private GameObject resetButton;

        private void Awake() {
            this.resetButton.SetActive(false);
        }

        public void StartGame() {
            this.runner.StartGame();
        }

        public override void OnStateChange(GameState state) {
            base.OnStateChange(state);
            switch (state.status) {
                case GameState.Status.Running: {
                    this.startButton.SetActive(false);
                    break;
                }
                case GameState.Status.End: {
                    this.resetButton.SetActive(true);
                    break;
                }
            }
        }
        public void ResetButton() {
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
    }
}