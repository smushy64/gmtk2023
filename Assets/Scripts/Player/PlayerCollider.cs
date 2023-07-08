using UnityEngine;

namespace Player {
    public class PlayerCollider: MonoBehaviour {
        [SerializeField] private PlayerController _player;
        public PlayerController player => this._player;
    }
}