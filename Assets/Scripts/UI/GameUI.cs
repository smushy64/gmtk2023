// * Description:  Game UI logic
// * Author:       Alicia Amarilla (smushyaa@gmail.com)
// * File Created: July 08, 2023

using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Runner;

public class GameUI : GameMechanic {

    [SerializeField]
    GameObject pauseMenu;
    [SerializeField]
    GameObject levelCompleteMenu;
    [SerializeField]
    Slider health_bar;

    const float POPUP_ANIMATION_LENGTH = 0.1f;
    Animator pause_menu_animator;

    Animator level_menu_animator;

    AudioMixer mixer;
    bool is_audio_enabled = true;

    int popup_hash   = Animator.StringToHash( "Popup" );
    int popdown_hash = Animator.StringToHash( "Popdown" );

    void Awake() {
        mixer = Resources.Load<AudioMixer>( "MainMixer" );
        pause_menu_animator = pauseMenu.GetComponent<Animator>();
        level_menu_animator = levelCompleteMenu.GetComponent<Animator>();
    }
    void Start() {
        runner.game_ui = this;
        int max_health = runner.player.max_health;
        update_health( max_health, max_health );
        runner.player.on_health_update += update_health;
    }

    void OnEnable() {
        if( runner != null && runner.player != null ) {
            runner.player.on_health_update += update_health;
        }
    }
    void OnDisable() {
        runner.player.on_health_update -= update_health;
    }

    void update_health( int health, int max_health ) {
        health_bar.value = (float)health / (float)max_health;
    }

    IEnumerator ipopdown;
    IEnumerator popdown( GameObject obj ) {
        float timer = 0.0f;
        while( timer < POPUP_ANIMATION_LENGTH ) {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        obj.SetActive( false );
    }

    void pause( bool pause ) {
        if( pause ) {
            runner.set_paused( true );
            pauseMenu.SetActive( true );
            pause_menu_animator.Play( popup_hash );
        } else {
            runner.set_paused( false );
            pause_menu_animator.Play( popdown_hash );
            if( ipopdown != null ) {
                this.StopCoroutine( ipopdown );
            }
            ipopdown = popdown( pauseMenu );
            this.StartCoroutine( ipopdown );
        }
    }
    public void on_pause() {
        pause( !pauseMenu.activeSelf );
    }

    public void on_press_home() {
        pause( false );
        SceneManager.LoadScene( 0 );
    }

    public void on_audio_toggle() {
        is_audio_enabled = !is_audio_enabled;
        mixer.SetFloat(
            "MixerVolume",
            is_audio_enabled ?
            MainMenu.FULL_VOLUME : MainMenu.NO_VOLUME
        );
    }

    public void on_reload() {
        pause( false );
        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );
    }

    public void on_next() {
        on_reload();
    }

    public override void OnStateChange(
        GameRunner.Status status,
        GameRunner.LevelResult level_result
    ) {
        base.OnStateChange( status, level_result );
        if( status == GameRunner.Status.End ) {
            levelCompleteMenu.SetActive( true );
        }
    }
}