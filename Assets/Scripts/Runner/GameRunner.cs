using System;
using Heroes;
using NaughtyAttributes;
using Player;
using Projectiles;
using RamenSea.Foundation3D.Services.KeyStore;
using UnityEngine;

namespace Runner {

    // a simple base class to make referencing the shared game state easier
    public abstract class GameMechanic : MonoBehaviour { 
        protected GameRunner runner { get; private set; } // that circle reference loops ftw!

        public virtual void OnSetGameRunner(GameRunner runner) {
            this.runner = runner;
        }
        public virtual void OnStateChange(
            GameRunner.Status status,
            GameRunner.LevelResult level_result
        ) { } // we could pass in the old state if need be, but eeeh
    }
    
    [DefaultExecutionOrder(GameRunner.EXECUTION_ORDER_GAME_RUNNER)] // this just makes it so the game runner will run before everything else
    public class GameRunner: MonoBehaviour {
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

        public Status status { set; get; } = Status.Running;
        Status previous_status = Status.Running;
        public LevelResult level_result { private set; get; }

        public const int EXECUTION_ORDER_GAME_RUNNER = -1000;

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

        public void update_status( Status new_status ) {
            previous_status = status;
            status = new_status;
        }
        public void update_level_result( LevelResult result ) {
            level_result = result;
        }

        // NOTE(alicia): i know, bad roundtripping but
        // hey we got a game to finish
        public GameUI game_ui = null;

        public void set_paused( bool paused ) {
            update_status(
                paused ? Status.Paused :
                previous_status
            );
            Time.timeScale = paused ? 0f : 1f;
        }
        public void player_pause() {
            game_ui.on_pause();
        }

        private void Awake() {
            this.keyStore = new KeyStoreService();
        }
        private void Start() {
            foreach (var mechanic in this.mechanics) {
                mechanic.OnSetGameRunner(this);
            }
            update_status( Status.Running );
            foreach (var mechanic in this.mechanics) {
                mechanic.OnStateChange( status, level_result );
            }
        }

        [Button("Start game", EButtonEnableMode.Playmode)]
        public void StartGame() {
            if (this.status != Status.SetUp) {
                Debug.LogError("The game has already started");
                return;
            }

            update_status( Status.Running );
            
            foreach (var mechanic in this.mechanics) {
                mechanic.OnStateChange( status, level_result );
            }
        }
        public void PlayerDied() {
            update_status( Status.End );
            update_level_result( LevelResult.Died );
            
            foreach (var mechanic in this.mechanics) {
                mechanic.OnStateChange( status, level_result );
            }
        }
        public void StoreDidClose() { //woo
            update_status( Status.End );
            update_level_result( LevelResult.Won );
            
            foreach (var mechanic in this.mechanics) {
                mechanic.OnStateChange( status, level_result );
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