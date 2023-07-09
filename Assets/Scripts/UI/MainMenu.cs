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

    public void Play() {
        SceneManager.LoadScene( startGameSceneIndex );
    }

    public static float FULL_VOLUME = 0.0f;
    public static float NO_VOLUME   = -80.0f;
    public void ToggleAudio() {
        is_audio_enabled = !is_audio_enabled;
        mixer.SetFloat( "MixerVolume", is_audio_enabled ? FULL_VOLUME : NO_VOLUME );
    }

    public void Exit() {
        Application.Quit( 0 );
    }

}