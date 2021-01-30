using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class InteractableBase : MonoBehaviour
{
    [SerializeField]
    protected InteractableReticle reticle;
    [SerializeField]
    protected InteractReticleType interactReticleType = InteractReticleType.Look;
    
    public Vector2 visibleAngleRange = new Vector2(360f, 360f);
    
    [Header("Wwise Events")]
    public AK.Wwise.Event InteractStartAudio;
    public AK.Wwise.Event InteractEndAudio;
    
    [Header("Unity Events")]
    public UnityEvent OnInteractStart = new UnityEvent();
    public UnityEvent OnInteractEnd = new UnityEvent();
    
    public bool isActivated { get; protected set; }
    public bool isInteracting { get; protected set; }
    
    
    public bool isInteractable { get; protected set; }
    
    
    protected virtual void Start()
    {
        SetInteractable(true);
        reticle.SetInteractType(interactReticleType);
        if (Interactor.instance != null)
        {
            Interactor.instance.onBeginInteractionEvent += HideReticle;
            Interactor.instance.onFinishInteractionEvent += UnhideReticle;
        }
    }

    protected virtual void OnDestroy()
    {
        if (Interactor.instance != null)
        {
            Interactor.instance.onBeginInteractionEvent -= HideReticle;
            Interactor.instance.onFinishInteractionEvent -= UnhideReticle;
        }
    }

    protected virtual void Update()
    {
        if (Interactor.instance != null)
        {
            Interactor.instance.ProcessInteractableActivationRange(this);
        }
    }
    
    public void SetInteractable(bool argInteractable)
    {
        isInteractable = argInteractable;

        if (isInteractable == false)
        {
            HideReticle();
        }
    }


    public virtual Transform GetLookTarget()
    {
        return (reticle != null) ? reticle.transform : this.transform;
    }
    
    public virtual void OnEnterActivationRange()
    {
        isActivated = true;
        reticle.ToggleActivatedReticle(true);
    }
    
    public virtual void OnExitActivationRange()
    {
        isActivated = false;
        reticle.ToggleActivatedReticle(false);
    }
    
    public virtual void OnBecomePointerTarget()
    {
        reticle.ToggleInteractReticle(true);
        reticle.ToggleActivatedReticle(false);
    }
    
    public virtual void OnNoLongerPointerTarget()
    {
        reticle.ToggleInteractReticle(false);
        reticle.ToggleActivatedReticle(false);
    }
    
    public virtual void OnBeginInteraction()
    {
        isInteracting = true;
        OnInteractStart.Invoke();
        PlayBeginInteractionSound();
        PerformInteraction();
    }
    
    protected virtual void PerformInteraction()
    {
        Interactor.instance.QuitInteraction();
    }
    
    public virtual void OnFinishInteraction()
    {
        isInteracting = false;
        PlayFinishInteractionSound();
        OnInteractEnd.Invoke();
    }

    protected virtual void HideReticle()
    { 
        reticle.gameObject.SetActive(false);
    }
    
    protected virtual void UnhideReticle()
    {
        if (isInteractable)
        {
            reticle.gameObject.SetActive(true);
        }
    }

    protected virtual void PlayBeginInteractionSound()
    {
        InteractStartAudio.Post(gameObject);
    }
    
    protected virtual void PlayFinishInteractionSound()
    {
        InteractEndAudio.Post(gameObject);
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Transform lookTarget = GetLookTarget();
        if (visibleAngleRange.y < 360f)
        {
            Handles.color = new Color(0f, 1f, 0f, 0.35f);
            Handles.DrawSolidArc(lookTarget.position, lookTarget.up,
                Quaternion.AngleAxis(-visibleAngleRange.y / 2f, lookTarget.up) * (lookTarget.forward), 
                visibleAngleRange.y, 0.2f);
        }

        if (visibleAngleRange.x < 360f)
        {
            Handles.color  = new Color(1f, 0f, 0f, 0.35f);
            Handles.DrawSolidArc(lookTarget.position, lookTarget.right,
                Quaternion.AngleAxis(-visibleAngleRange.x / 2f, lookTarget.right) * (lookTarget.forward),
                visibleAngleRange.x, 0.2f);
        }
    }


#endif
    
}
