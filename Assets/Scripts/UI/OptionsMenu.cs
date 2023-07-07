using UnityEngine;
using UnityEngine.UI;

public static class GlobalOptions {
    public static float volume       = 0.75f;
    public static float sfx_volume   = 1.0f;
    public static float music_volume = 0.9f;
}

public class OptionsMenu : MonoBehaviour {
    
    [SerializeField]
    Slider volume_slider;
    [SerializeField]
    Slider sfx_slider;
    [SerializeField]
    Slider music_slider;

    void Start() {
        volume_slider.value = GlobalOptions.volume;
        sfx_slider.value    = GlobalOptions.sfx_volume;
        music_slider.value  = GlobalOptions.music_volume;
    }

    public void OnVolumeSlider( float value ) {
        GlobalOptions.volume = value;
    }
    public void OnSFXSlider( float value ) {
        GlobalOptions.sfx_volume = value;
    }
    public void OnMusicSlider( float value ) {
        GlobalOptions.music_volume = value;
    }
}