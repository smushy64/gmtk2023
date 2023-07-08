using System;
using NaughtyAttributes;
using RamenSea.Foundation.Extensions;
using UnityEngine;

namespace General {
    public class GameProgressBar: MonoBehaviour {

        [SerializeField] private SpriteRenderer background;
        [SerializeField] private SpriteRenderer bar;


        public void SetProgress(float progress) {
            progress = progress.Clamp01();
            var backgroundScale = this.background.transform.localScale;
            bar.transform.localScale = new Vector3(progress, backgroundScale.y, 1f);
            this.bar.transform.localPosition = new Vector3(progress * 0.5f - 0.5f, 0f, 0f);
        }


        [Button("Test")]
        private void Test() {
            this.SetProgress(0.8f);
        }
    }
}