using System;
using UnityEngine.SceneManagement;
using MichaelWolfGames;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [SerializeField]
    private Image _fadeScreen;
    [SerializeField]
    private Image _illustrationContainer;
    [SerializeField]
    private Sprite[] _illustrations;


    private Color alphaB, opaqueB, alphaW, opaqueW;
    private int cycleIndex = 0;

    private void OnEnable() {
        alphaB = opaqueB = Color.black;
        alphaB.a = 0;
        alphaW = opaqueW = Color.white;
        alphaW.a = 0;

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
        Color start = _fadeScreen.color;
        this.DoTween(lerp => { _fadeScreen.color = Color.Lerp(alphaB, opaqueB, lerp); }, callback, 1.25f, easeType: EaseType.easeOutCubic);
    }

    /// <summary>
    /// Triggered by handing items into grandma
    /// </summary>
    public void PerformArtSequence(Action argOnFadeComplete = null)
    {
        //out of pics, load next level directly
        if (cycleIndex > _illustrations.Length) {
            DoFadeOut(() =>
            {
                if (argOnFadeComplete != null)
                {
                    argOnFadeComplete();
                }
            });
        }

        //transparent to solid white
        Color start = _fadeScreen.color;
        _fadeScreen.raycastTarget = true;
        this.DoTween(lerp => { _fadeScreen.color = Color.Lerp(start, opaqueW, lerp); },
        ()=> {
            if (_illustrationContainer)
            {
                _illustrationContainer.gameObject.SetActive(true);
                FadeArtIn();
                _illustrationContainer.sprite = _illustrations[cycleIndex];
            }
            cycleIndex++;
        }, duration: 1.0f, easeType: EaseType.linear, useUnscaledTime: true);
    }

    public void FadeArtIn() {
        Color start = _fadeScreen.color;
        this.DoTween(lerp => { _fadeScreen.color = Color.Lerp(start, alphaW, lerp); }, ()=> { _fadeScreen.raycastTarget = false; }, duration: 1.0f, easeType: EaseType.linear, useUnscaledTime: true);

    }


}
