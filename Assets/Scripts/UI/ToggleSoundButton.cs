// * Description:  Changes sound toggle button sprite
// * Author:       Alicia Amarilla (smushyaa@gmail.com)
// * File Created: July 08, 2023

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Image))]
public class ToggleSoundButton : MonoBehaviour {

    [SerializeField]
    Sprite soundEnabledSprite;
    [SerializeField]
    Sprite soundEnabledPressedSprite;
    [SerializeField]
    Sprite soundDisabledSprite;
    [SerializeField]
    Sprite soundDisabledPressedSprite;

    Button button;
    Image  image;

    bool is_enabled = true;

    void Awake() {
        button = GetComponent<Button>();
        image  = GetComponent<Image>();
    }

    public void on_press() {
        is_enabled = !is_enabled;
        update_sprites();
    }

    void update_sprites() {
        SpriteState sprite_state;
        sprite_state.pressedSprite = is_enabled ? soundEnabledPressedSprite : soundDisabledPressedSprite;
        button.spriteState = sprite_state;

        image.sprite = is_enabled ? soundEnabledSprite : soundDisabledSprite;
    }

}