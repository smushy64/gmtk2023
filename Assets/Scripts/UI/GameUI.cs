// * Description:  Game UI logic
// * Author:       Alicia Amarilla (smushyaa@gmail.com)
// * File Created: July 08, 2023

using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Runner;

public class GameUI : GameMechanic {

    [SerializeField]
    GameObject pauseMenu;

    const float POPUP_ANIMATION_LENGTH = 0.1f;
    Animator pause_menu_animator;

    AudioMixer mixer;
    bool is_audio_enabled = true;

    void Awake() {
        mixer = Resources.Load<AudioMixer>( "MainMixer" );
        pause_menu_animator = pauseMenu.GetComponent<Animator>();
    }
    void Start() {
        runner.game_ui = this;
    }

    IEnumerator ipopdown;
    IEnumerator popdown() {
        float timer = 0.0f;
        while( timer < POPUP_ANIMATION_LENGTH ) {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        pauseMenu.SetActive( false );
    }

    void pause( bool pause ) {
        if( pause ) {
            runner.set_paused( true );
            pauseMenu.SetActive( true );
            pause_menu_animator.Play( "Popup" );
        } else {
            runner.set_paused( false );
            pause_menu_animator.Play( "Popdown" );
            if( ipopdown != null ) {
                this.StopCoroutine( ipopdown );
            }
            ipopdown = popdown();
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
}