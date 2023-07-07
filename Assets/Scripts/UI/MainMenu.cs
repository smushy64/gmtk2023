using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [SerializeField]
    GameObject quitGameButton;
    [SerializeField]
    GameObject optionsMenu;
    [SerializeField]
    int startGameSceneIndex = -1;

    void Start() {
#if UNITY_WEBGL && !UNITY_EDITOR
        quitGameButton.SetActive( false );
#endif
    }

    public void StartGame() {
        SceneManager.LoadScene( startGameSceneIndex );
    }

    public void OpenOptions() {
        optionsMenu.SetActive( true );
    }

    public void CloseOptions() {
        optionsMenu.SetActive( false );
    }

    public void QuitGame() {
        Application.Quit( 0 );
    }

}