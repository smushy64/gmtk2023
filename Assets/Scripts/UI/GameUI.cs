// * Description:  Game UI logic
// * Author:       Alicia Amarilla (smushyaa@gmail.com)
// * File Created: July 08, 2023

using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

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

    IEnumerator ipopdown;
    IEnumerator popdown() {
        float timer = 0.0f;
        while( timer < POPUP_ANIMATION_LENGTH ) {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        pauseMenu.SetActive( false );
    }

    public void on_pause() {
        // TODO(alicia): actually pause the game too,
        // or send a message to game runner?
        
        if( !pauseMenu.activeSelf ) {
            pauseMenu.SetActive( true );
            pause_menu_animator.Play( "Popup" );
        } else {
            pause_menu_animator.Play( "Popdown" );
            if( ipopdown != null ) {
                this.StopCoroutine( ipopdown );
            }
            ipopdown = popdown();
            this.StartCoroutine( ipopdown );
        }
    }

    public void on_press_home() {
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
        SceneManager.LoadScene( SceneManager.GetActiveScene().buildIndex );
    }
}