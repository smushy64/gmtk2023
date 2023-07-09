using System;
using RamenSea.Foundation.Extensions;
using RamenSea.Foundation.Pools;
using Runner;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI {
    public class DebugLevelUI: GameMechanic {
        
        [SerializeField] private GameObject startButton;
        [SerializeField] private GameObject resetButton;
        [SerializeField] private TMP_Text closingTimeCountDown;
        [SerializeField] private TMP_Text closingTimeCountDownHeader;
        [SerializeField] private TMP_Text heroCount;
        [SerializeField] private TMP_Text endOfLevelText;

        private int lastClosingTimeValue = -1;
        private void Awake() {
            this.resetButton.SetActive(false);
            this.closingTimeCountDown.gameObject.SetActive(false);
            this.closingTimeCountDownHeader.gameObject.SetActive(false);
            this.heroCount.gameObject.SetActive(false);
        }

        private void Update() {
            if (this.runner.levelController.isClosingTime && this.runner.state.status == GameState.Status.Running) {
                var closingTimeSecond = this.runner.levelController.timeTilClosing.Ceil().ToInt();
                if (closingTimeSecond != this.lastClosingTimeValue) {
                    this.lastClosingTimeValue = closingTimeSecond;
                    this.closingTimeCountDown.text = closingTimeSecond.ToString();
                }
            }
        }

        public void StartGame() {
            this.runner.StartGame();
        }

        public override void OnSetGameRunner(GameRunner runner) {
            base.OnSetGameRunner(runner);
            this.endOfLevelText.text = $"Level: {LevelController.currentLevel}";
            runner.levelController.onHeroMadHappyCountDidChange += OnHeroMadHappyCountDidChange;
            runner.levelController.onClosingTimeStarted += OnClosingTimeStarted;
        }

        private void OnClosingTimeStarted() {
            this.closingTimeCountDown.gameObject.SetActive(true);
            this.closingTimeCountDownHeader.gameObject.SetActive(true);
        }

        private void OnHeroMadHappyCountDidChange() {
            using var s = StringBuilderPool.Get();
            s.Append(this.runner.levelController.happyHeroesCount);
            s.Append("/");
            s.Append(this.runner.levelController.totalHeroCount);
            s.Append(" heroes");
            if (this.runner.levelController.madHeroesCount > 0) {
                s.Append("\n<color=red>");
                s.Append(this.runner.levelController.madHeroesCount);
                s.Append("</color>");
            }
            this.heroCount.text = s.ToString();
        }

        public override void OnStateChange(GameState state) {
            base.OnStateChange(state);
            switch (state.status) {
                case GameState.Status.Running: {
                    this.heroCount.gameObject.SetActive(true);
                    this.endOfLevelText.gameObject.SetActive(false);
                    this.startButton.SetActive(false);
                    this.OnHeroMadHappyCountDidChange();
                    break;
                }
                case GameState.Status.End: {
                    this.resetButton.SetActive(true);
                    var endOfGameMessage = this.runner.state.levelResult == GameState.LevelResult.Died
                        ? "You died"
                        : "Next level"; 
                    this.endOfLevelText.gameObject.SetActive(true);
                    this.endOfLevelText.text = $"Level: {LevelController.currentLevel}\n{endOfGameMessage}";
                    break;
                }
            }
        }
        public void NextLevel() {
            SceneManager.LoadScene("FinalLevel", LoadSceneMode.Single);
        }
    }
}