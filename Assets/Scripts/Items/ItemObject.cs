using UnityEngine;

namespace Items {
    public class ItemObject: MonoBehaviour {
        [SerializeField] private Item _item;
        public Item item => _item;
    }
}