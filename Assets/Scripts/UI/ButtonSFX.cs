// * Description:  Plays sound effect when pressing a ui button
// * Author:       Alicia Amarilla (smushyaa@gmail.com)
// * File Created: July 09, 2023

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ButtonSFX : MonoBehaviour {
    [SerializeField]
    AudioClip clip;

    AudioSource source;

    void Awake() {
        source = GetComponent<AudioSource>();
        source.clip = clip;
    }

    public void on_click() {
        source.Play();
    }
}