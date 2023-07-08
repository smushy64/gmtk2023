using UnityEngine;

namespace Items {
    [RequireComponent(typeof(SpriteRenderer))]
    public class ItemObject: MonoBehaviour {
        [SerializeField]
        Sprite[] sprites = new Sprite[(int)Items.Item.MAX];

        SpriteRenderer sprite_renderer;

        void Awake() {
            sprite_renderer = GetComponent<SpriteRenderer>();
            sprite_renderer.sprite = sprites[0];
        }

        public void set_sprite( Item item ) {
            sprite_renderer.sprite = sprites[(int)item];
        }
    }
}