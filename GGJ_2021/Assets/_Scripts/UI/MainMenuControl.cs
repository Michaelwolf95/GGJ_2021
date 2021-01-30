using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuControl : MonoBehaviour
{
    [SerializeField]
    private ScreenFade _fadeObj;

    private Action loadCallback;

    public void LoadLevel(int levelIndex) {
        loadCallback = delegate() { SceneManager.LoadScene(levelIndex); };
        _fadeObj.DoFadeOut(loadCallback);
    }
    
    public void QuitGame() {
        Application.Quit(0);
    }
}
