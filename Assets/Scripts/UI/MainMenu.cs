// * Description:  Main Menu
// * Author:       Alicia Amarilla (smushyaa@gmail.com)
// * File Created: July 08, 2023

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour {

    [SerializeField]
    GameObject intro;
    [SerializeField]
    GameObject exitGameButton;
    [SerializeField]
    GameObject creditsPanel;
    [SerializeField]
    int startGameSceneIndex = -1;

    AudioMixer mixer;
    bool is_audio_enabled = true;

    // NOTE(alicia): does not play properly in editor
    // but i know from experience that it works in 
    // release build
    const float INTRO_LENGTH = 1.0f;
    IEnumerator disable_intro_coroutine;
    IEnumerator disable_intro() {
        float timer = 0.0f;
        while( timer < INTRO_LENGTH ) {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        intro.SetActive( false );
    }

    void Start() {
        mixer = Resources.Load<AudioMixer>("MainMixer");

#if UNITY_EDITOR
        intro.SetActive(false);
#else
        intro.SetActive(true);
        if( disable_intro_coroutine != null ) {
            this.StopCoroutine( disable_intro_coroutine );
        }
        disable_intro_coroutine = disable_intro();
        this.StartCoroutine( disable_intro_coroutine );
#endif

#if UNITY_WEBGL || UNITY_EDITOR
        exitGameButton.SetActive( false );
#endif
    }

    // NOTE(alicia): added a tiny delay so
    // you can see the button being pressed
    const float BUTTON_DELAY = 0.025f;
    IEnumerator iload_game;
    IEnumerator load_game() {
        float timer = 0f;
        while( timer < BUTTON_DELAY ) {
            timer += Time.unscaledTime;
            yield return null;
        }
        SceneManager.LoadScene( startGameSceneIndex );
    }

    IEnumerator iexit_game;
    IEnumerator exit_game() {
        float timer = 0f;
        while( timer < BUTTON_DELAY ) {
            timer += Time.unscaledTime;
            yield return null;
        }
        Application.Quit( 0 );
    }

    public void Play() {
        if( iload_game != null ) {
            this.StopCoroutine( iload_game );
        }
        iload_game = load_game();
        this.StartCoroutine( iload_game );
    }

    public static float FULL_VOLUME = 0.0f;
    public static float NO_VOLUME   = -80.0f;
    public void ToggleAudio() {
        is_audio_enabled = !is_audio_enabled;
        mixer.SetFloat( "MixerVolume", is_audio_enabled ? FULL_VOLUME : NO_VOLUME );
    }

    IEnumerator itoggle_credits;
    IEnumerator toggle_credits_e() {
        float timer = 0.0f;
        while( timer < BUTTON_DELAY ) {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        creditsPanel.SetActive( !creditsPanel.activeSelf );
    }

    public void toggle_credits() {
        if( itoggle_credits != null ) {
            this.StopCoroutine( itoggle_credits );
        }
        itoggle_credits = toggle_credits_e();
        this.StartCoroutine( itoggle_credits );
    }

    public void Exit() {
        if( iexit_game != null ) {
            this.StopCoroutine( iexit_game );
        }
        iexit_game = exit_game();
        this.StartCoroutine( iexit_game );
    }

}