using System;
using Heroes;
using NaughtyAttributes;
using Player;
using Projectiles;
using RamenSea.Foundation3D.Services.KeyStore;
using UnityEngine;

namespace Runner {
    public struct GameState {
        public enum Status : byte {
            SetUp, // Could be main menu? Depends on where we end up putting this
            Running, // Game is running
            End, // Game has completed, there will probably need to be 
        }
        public enum LevelResult : byte {
            None,
            Won,
            Died,
        }

        public Status status;
        public LevelResult levelResult;

        public void set_status( Status status ) {
            this.status = status;
        }
    }

    // a simple base class to make referencing the shared game state easier
    public abstract class GameMechanic : MonoBehaviour { 
        protected GameRunner runner { get; private set; } // that circle reference loops ftw!

        public virtual void OnSetGameRunner(GameRunner runner) {
            this.runner = runner;
        }
        public virtual void OnStateChange(GameState state) { } // we could pass in the old state if need be, but eeeh
    }
    
    [DefaultExecutionOrder(GameRunner.EXECUTION_ORDER_GAME_RUNNER)] // this just makes it so the game runner will run before everything else
    public class GameRunner: MonoBehaviour {
        public const int EXECUTION_ORDER_GAME_RUNNER = -1000;
        public GameState state { private set; get; }

        [SerializeField] private GameMechanic[] mechanics;
        
        
        // List of specific game mechanics so we can reference them if we want
        [SerializeField] private PlayerController _player;
        public PlayerController player => this._player;
        [SerializeField] private ProjectileRecycler _projectileRecycler;
        public ProjectileRecycler projectileRecycler => this._projectileRecycler;
        [SerializeField] private HeroRecycler _heroRecycler;
        public HeroRecycler heroRecycler => this._heroRecycler;
        [SerializeField] private LevelController _levelController;
        public LevelController levelController => this._levelController;
        public KeyStoreService keyStore { private get; set; }

        private void Awake() {
            this.state = new GameState() {
                status = GameState.Status.SetUp
            };
            this.keyStore = new KeyStoreService();
        }
        private void Start() {
            foreach (var mechanic in this.mechanics) {
                mechanic.OnSetGameRunner(this);
            }
            foreach (var mechanic in this.mechanics) {
                mechanic.OnStateChange(this.state);
            }
        }
        private void Update() {
        }

        [Button("Start game", EButtonEnableMode.Playmode)]
        public void StartGame() {
            if (this.state.status != GameState.Status.SetUp) {
                Debug.LogError("The game has already started");
                return;
            }
            var s = this.state;
            s.status = GameState.Status.Running;
            this.state = s;
            
            foreach (var mechanic in this.mechanics) {
                mechanic.OnStateChange(this.state);
            }
        }
        public void PlayerDied() {
            var s = this.state;
            s.status = GameState.Status.End; // reverting this, since accessing a synthetic property in c# isn't which as intuitive as you'd think
            s.levelResult = GameState.LevelResult.Died;
            this.state = s;
            
            foreach (var mechanic in this.mechanics) {
                mechanic.OnStateChange(this.state);
            }
        }
        public void StoreDidClose() { //woo
            var s = this.state;
            s.status = GameState.Status.End; // reverting this, since accessing a synthetic property in c# isn't which as intuitive as you'd think
            s.levelResult = GameState.LevelResult.Won;
            this.state = s;
            
            foreach (var mechanic in this.mechanics) {
                mechanic.OnStateChange(this.state);
            }
        }
        
#if UNITY_EDITOR
        private void Reset() {
            this.UpdateGameMechanics();
        }

        // Call this method in the editor if you add a new game mechanic
        // This runs in the editor so it can be as slow as we want!!
        [Button("Fetch game mechanics", EButtonEnableMode.Editor)]
        private void UpdateGameMechanics() {
            this.mechanics = FindObjectsByType<GameMechanic>(FindObjectsSortMode.None);
            this._player = FindObjectOfType<PlayerController>();
            this._projectileRecycler = FindObjectOfType<ProjectileRecycler>();
            this._heroRecycler = FindObjectOfType<HeroRecycler>();
            this._levelController = FindObjectOfType<LevelController>();
            
            this._projectileRecycler.UpdatePrefabs();
            this._heroRecycler.UpdatePrefabs();
        }
#endif
    }
}