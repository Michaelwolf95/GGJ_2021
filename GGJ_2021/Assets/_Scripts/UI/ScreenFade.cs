using System;
using UnityEngine.SceneManagement;
using MichaelWolfGames;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [SerializeField]
    private Image _fadeScreen;
    private Color alphaB, opaqueB;

    private void OnEnable() {
        alphaB = opaqueB = Color.black;
        alphaB.a = 0;
        _fadeScreen.color = opaqueB;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode mode) {
        _fadeScreen.raycastTarget = true;
        this.DoTween(lerp => { _fadeScreen.color = Color.Lerp(opaqueB, alphaB, lerp); }, () => { _fadeScreen.raycastTarget = false; }, 1.25f, easeType: EaseType.easeInCubic);
    }

    public void DoFadeOut(Action callback = null){
        _fadeScreen.raycastTarget = true;
        this.DoTween(lerp => { _fadeScreen.color = Color.Lerp(alphaB, opaqueB, lerp); }, callback, 1.25f, easeType: EaseType.easeOutCubic);
    }
}
