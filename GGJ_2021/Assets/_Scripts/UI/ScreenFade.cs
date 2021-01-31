using System;
using UnityEngine.SceneManagement;
using MichaelWolfGames;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    [SerializeField] private Image _fadeScreen;
    // [SerializeField]
    // private Image _illustrationContainer;
    // [SerializeField]
    // private Sprite[] _illustrations;

    [SerializeField] private CanvasGroup artCanvasGroup;

    [SerializeField] private AK.Wwise.Event onShowArt;
    


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
    
    public void DoFadeIn(Action callback = null, float duration = 1.25f){
        _fadeScreen.raycastTarget = true;
        this.DoTween(lerp => { _fadeScreen.color = Color.Lerp(opaqueB, alphaB, lerp); }, () =>
        {
            _fadeScreen.raycastTarget = false;
            if (callback != null) callback();
        }, duration, 0f, EaseType.easeInCubic, true);
    }

    public void DoFadeOut(Action callback = null, float duration = 1.25f, float delay = 0f){
        _fadeScreen.raycastTarget = true;
        Color start = _fadeScreen.color;
        this.DoTween(lerp => { _fadeScreen.color = Color.Lerp(alphaB, opaqueB, lerp); }, callback, duration, delay, EaseType.easeOutCubic, true);
    }

    /// <summary>
    /// Triggered by handing items into grandma
    /// </summary>
    public void PerformEndLevelArtSequence(Action argOnFadeComplete = null)
    {
        DoFadeOut(() =>
        {
            onShowArt.Post(gameObject);
            artCanvasGroup.gameObject.SetActive(true);
            DoFadeIn(() =>
            {
                DoFadeOut(() =>
                {
                    if (argOnFadeComplete != null)
                    {
                        argOnFadeComplete();
                    }
                }, 1.25f, 3f);
            });
        });
        //
        // //out of pics, load next level directly
        // if (cycleIndex > _illustrations.Length) {
        //     DoFadeOut(() =>
        //     {
        //         if (argOnFadeComplete != null)
        //         {
        //             argOnFadeComplete();
        //         }
        //     });
        // }
        //
        // //transparent to solid white
        // Color start = _fadeScreen.color;
        // _fadeScreen.raycastTarget = true;
        // this.DoTween(lerp => { _fadeScreen.color = Color.Lerp(start, opaqueW, lerp); },
        // ()=> {
        //     if (_illustrationContainer)
        //     {
        //         _illustrationContainer.gameObject.SetActive(true);
        //         FadeArtIn();
        //         _illustrationContainer.sprite = _illustrations[cycleIndex];
        //     }
        //     cycleIndex++;
        // }, duration: 1.0f, easeType: EaseType.linear, useUnscaledTime: true);
    }

    public void FadeArtIn() {
        Color start = _fadeScreen.color;
        this.DoTween(lerp => { _fadeScreen.color = Color.Lerp(start, alphaW, lerp); }, ()=> { _fadeScreen.raycastTarget = false; }, duration: 1.0f, easeType: EaseType.linear, useUnscaledTime: true);

    }


}
