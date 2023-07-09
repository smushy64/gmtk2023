// * Description:  Game UI logic
// * Author:       Alicia Amarilla (smushyaa@gmail.com)
// * File Created: July 08, 2023

using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Runner;
using TMPro;

public class GameUI : GameMechanic {

    [SerializeField]
    GameObject pauseMenu;
    [SerializeField]
    GameObject levelCompleteMenu;
    [SerializeField]
    GameObject gameOverMenu;
    [SerializeField]
    Slider health_bar;

    [SerializeField]
    GameObject sword_ready_icon;
    [SerializeField]
    GameObject potion_ready_icon;
    [SerializeField]
    GameObject book_ready_icon;

    [SerializeField]
    TMP_Text dayCounter;

    [SerializeField]
    TMP_Text levelEndTitle;

    [SerializeField]
    TMP_Text happyCountLose, happyCountWin;
    [SerializeField]
    TMP_Text angryCountLose, angryCountWin;

    [SerializeField]
    Image heldItem;
    [SerializeField]
    Sprite[] items = new Sprite[(int)Items.Item.MAX];

    const float POPUP_ANIMATION_LENGTH = 0.1f;
    Animator pause_menu_animator;

    Animator level_menu_animator;
    Animator game_over_animator;

    AudioMixer mixer;
    bool is_audio_enabled = true;

    int popup_hash   = Animator.StringToHash( "Popup" );
    int popdown_hash = Animator.StringToHash( "Popdown" );

    void enable_sword_icon() {
        sword_ready_icon.SetActive( true );
    }
    void enable_potion_icon() {
        potion_ready_icon.SetActive( true );
    }
    void enable_book_icon() {
        book_ready_icon.SetActive( true );
    }

    void disable_sword_icon() {
        sword_ready_icon.SetActive( false );
    }
    void disable_potion_icon() {
        potion_ready_icon.SetActive( false );
    }
    void disable_book_icon() {
        book_ready_icon.SetActive( false );
    }

    void Awake() {

        ItemStation[] stations = FindObjectsOfType<ItemStation>();
        for( int i = 0; i < stations.Length; ++i ) {
            ItemStation current = stations[i];
            switch( current.item_type ) {
                case Items.Item.Sword:
                    current.on_ready   += enable_sword_icon;
                    current.on_not_ready += disable_sword_icon;
                    break;
                case Items.Item.Potion:
                    current.on_ready   += enable_potion_icon;
                    current.on_not_ready += disable_potion_icon;
                    break;
                case Items.Item.SpellBook:
                    current.on_ready   += enable_book_icon;
                    current.on_not_ready += disable_book_icon;
                    break;
            }
        }

        mixer = Resources.Load<AudioMixer>( "MainMixer" );
        pause_menu_animator = pauseMenu.GetComponent<Animator>();
        level_menu_animator = levelCompleteMenu.GetComponent<Animator>();
        game_over_animator  = gameOverMenu.GetComponent<Animator>();
    }

    void on_hold_item( Items.Item item ) {
        heldItem.gameObject.SetActive( true );
        heldItem.sprite = items[(int)item];
    }
    void on_stop_hold_item() {
        heldItem.gameObject.SetActive( false );
    }

    void Start() {
        runner.game_ui = this;
        int max_health = runner.player.max_health;
        update_health( max_health, max_health );
        runner.player.on_health_update += update_health;

        runner.player.on_hold_item += on_hold_item;
        runner.player.on_stop_hold_item += on_stop_hold_item;

        dayCounter.SetText(
            runner
            .levelController
            .GetLevelData()
            .level.ToString()
        );
    }

    void OnEnable() {
        if( runner != null && runner.player != null ) {
            runner.player.on_health_update += update_health;
        }
        ItemStation[] stations = FindObjectsOfType<ItemStation>();
        for( int i = 0; i < stations.Length; ++i ) {
            ItemStation current = stations[i];
            switch( current.item_type ) {
                case Items.Item.Sword:
                    current.on_ready   += enable_sword_icon;
                    current.on_not_ready += disable_sword_icon;
                    break;
                case Items.Item.Potion:
                    current.on_ready   += enable_potion_icon;
                    current.on_not_ready += disable_potion_icon;
                    break;
                case Items.Item.SpellBook:
                    current.on_ready   += enable_book_icon;
                    current.on_not_ready += disable_book_icon;
                    break;
            }
        }
    }
    void OnDisable() {
        runner.player.on_hold_item -= on_hold_item;
        runner.player.on_stop_hold_item -= on_stop_hold_item;
        runner.player.on_health_update -= update_health;
        ItemStation[] stations = FindObjectsOfType<ItemStation>();
        for( int i = 0; i < stations.Length; ++i ) {
            ItemStation current = stations[i];
            switch( current.item_type ) {
                case Items.Item.Sword:
                    current.on_ready   -= enable_sword_icon;
                    current.on_not_ready -= disable_sword_icon;
                    break;
                case Items.Item.Potion:
                    current.on_ready   -= enable_potion_icon;
                    current.on_not_ready -= disable_potion_icon;
                    break;
                case Items.Item.SpellBook:
                    current.on_ready   -= enable_book_icon;
                    current.on_not_ready -= disable_book_icon;
                    break;
            }
        }
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
            if( pauseMenu.activeSelf ) {
                pause_menu_animator.Play( popdown_hash );
                if( ipopdown != null ) {
                    this.StopCoroutine( ipopdown );
                }
                ipopdown = popdown( pauseMenu );
                this.StartCoroutine( ipopdown );
            }
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
            switch( level_result ) {
                case GameRunner.LevelResult.Won:
                    levelEndTitle.SetText(
                        "DAY " +
                        runner
                        .levelController
                        .GetLevelData()
                        .level.ToString()
                    );
                    happyCountWin.SetText(
                        runner.levelController.happyHeroesCount.ToString()
                    );
                    angryCountWin.SetText(
                        runner.levelController.madHeroesCount.ToString()
                    );
                    levelCompleteMenu.SetActive( true );
                    break;
                case GameRunner.LevelResult.Died:
                    happyCountLose.SetText(
                        runner.levelController.happyHeroesCount.ToString()
                    );
                    angryCountLose.SetText(
                        runner.levelController.madHeroesCount.ToString()
                    );
                    gameOverMenu.SetActive( true );
                    break;
                default: break;
            }
        }
    }
}