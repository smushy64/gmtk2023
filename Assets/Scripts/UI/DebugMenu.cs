using Runner;
using UnityEngine;

namespace UI {
    public class DebugMenu: MonoBehaviour {
        
        [SerializeField] private GameObject startButton;
        [SerializeField] private GameRunner runner;

        public void StartGame() {
            this.runner.StartGame();
            this.startButton.SetActive(false);
        }
    }
}