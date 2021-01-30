using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuControl : MonoBehaviour
{
    [SerializeField]
    private ScreenFade _fadeObj;

    [SerializeField]
    private Button[] _levelButtons; 

    private Action loadCallback;

    private void OnEnable() {
        string unlocks = PlayerPrefs.GetString("LevelUnlocks");
        return;
        _levelButtons[0].interactable = true;
        _levelButtons[1].interactable = unlocks[1].Equals('x') ? true : false;
        _levelButtons[2].interactable = unlocks[2].Equals('x') ? true : false;
        _levelButtons[3].interactable = unlocks[3].Equals('x') ? true : false;
    }

    /// <summary>
    /// Fade out then load new level when fade ends
    /// </summary>
    /// <param name="levelIndex"></param>
    public void LoadLevel(int levelIndex) {
        loadCallback = delegate() { SceneManager.LoadScene(levelIndex); };
        _fadeObj.DoFadeOut(loadCallback);
    }
    
    public void QuitGame() {
        Application.Quit(0);
    }
}
