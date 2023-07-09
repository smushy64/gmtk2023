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
            Paused, // game is paused, time.deltaTime is 0
            End, // Game has completed, there will probably need to be 
        }
        public enum LevelResult : byte {
            None,
            Won,
            Died,
        }

        public Status status;
        public Status previous_status;
        public LevelResult levelResult;

        public void set_status( Status new_status ) {
            previous_status = status;
            status = new_status;
        }
        public void set_level_result( LevelResult result ) {
            levelResult = result;
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


        // NOTE(alicia): i know, bad roundtripping but
        // hey we got a game to finish
        public GameUI game_ui = null;

        public void set_paused( bool paused ) {
            state.set_status(
                paused ? GameState.Status.Paused :
                state.previous_status
            );
            Time.timeScale = paused ? 0f : 1f;
        }
        public void player_pause() {
            game_ui.on_pause();
        }

        private void Awake() {
            this.state = new GameState() { status = GameState.Status.Running };
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

        [Button("Start game", EButtonEnableMode.Playmode)]
        public void StartGame() {
            if (this.state.status != GameState.Status.SetUp) {
                Debug.LogError("The game has already started");
                return;
            }
            state.set_status( GameState.Status.Running );
            
            foreach (var mechanic in this.mechanics) {
                mechanic.OnStateChange(this.state);
            }
        }
        public void PlayerDied() {
            state.set_status( GameState.Status.End );
            state.set_level_result( GameState.LevelResult.Died );
            
            foreach (var mechanic in this.mechanics) {
                mechanic.OnStateChange(this.state);
            }
        }
        public void StoreDidClose() { //woo
            state.set_status( GameState.Status.End );
            state.set_level_result( GameState.LevelResult.Won );
            
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