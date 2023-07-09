// * Description:  Game UI logic
// * Author:       Alicia Amarilla (smushyaa@gmail.com)
// * File Created: July 08, 2023

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

    [SerializeField]
    GameObject pauseMenu;

    AudioMixer mixer;
    bool is_audio_enabled = true;

    void Awake() {
        mixer = Resources.Load<AudioMixer>( "MainMixer" );
    }

    public void on_pause() {
        // TODO(alicia): actually pause the game too,
        // or send a message to game runner?
        pauseMenu.SetActive( !pauseMenu.activeSelf );
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