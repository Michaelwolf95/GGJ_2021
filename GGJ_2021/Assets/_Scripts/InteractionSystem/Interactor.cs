using System;
using System.Collections;
using System.Collections.Generic;
using MichaelWolfGames;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Interactor : MonoBehaviour
{
    public static Interactor instance = null;
    
    [SerializeField] private Camera mainCamera = null;
    [SerializeField] private float activationRange = 2f;
    [SerializeField] private float interactRange = 1f;

    [SerializeField] private LayerMask interactionLayerMask = new LayerMask();

    [SerializeField] private Transform _grabPoint;
    public Transform grabPoint => _grabPoint;
    
    [SerializeField] private TextMeshPro inputPromptText = null;

    private InteractableBase currentPointerTarget = null;        // Currently being looked at.
    private InteractableBase currentInteractionTarget = null;    // Actively interacting with
    private bool isInteracting => currentInteractionTarget != null;
    
    private Transform viewOrigin => mainCamera.transform;

    private bool canInteract
    {
        get { return true; }
    }

    public Action onBeginInteractionEvent = delegate {  };
    public Action onFinishInteractionEvent = delegate {  };

    [SerializeField] private PlayerInput inputController;

    public GrabObject heldObject { get; set; }

    private void Awake()
    {
        instance = this;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        inputController.actions["Grab"].performed += ctx =>
        {
            HandleGrabInput();
        };
    }

    private void HandleGrabInput()
    {
        if (isInteracting == false && canInteract)
        {
            if (currentPointerTarget != null)
            {
                Debug.Log("Begin");
                BeginInteraction(currentPointerTarget);
            }
        }
    }
    
    public void FixedUpdate()
    {
        if (isInteracting)
        {
            return;
        }
        
        if(heldObject != null)
        {
            if (heldObject != currentPointerTarget) 
                SetPointerTarget(heldObject);
            return;
        }
        
        InteractableBase closestObject = null;
        float shortest = float.MaxValue;
        foreach (InteractableBase interactable in FindObjectsOfType<InteractableBase>())
        {
            float dist = Vector3.Distance(this.gameObject.transform.position, interactable.transform.position);
            if(dist <= activationRange && dist < shortest)
            {
                closestObject = interactable;
                shortest = dist;                
            }
        }
        
        if(closestObject != null && shortest < interactRange)
        {
            if(currentPointerTarget != null && currentPointerTarget != closestObject)
            {
                ClearPointerTarget();
            }
            SetPointerTarget(closestObject);
            if (closestObject != heldObject)
            {
                ToggleInputPrompt(true);
            }
        }
        else
        {
            ClearPointerTarget();
            ToggleInputPrompt(false);
        }
    }

    public void ToggleInputPrompt(bool argShow)
    {
        //ToDo: Set Input Text based on input scheme.
        // DISABLED
        //inputPromptText.gameObject.SetActive(argShow);
    }
    

    public void BeginInteraction(InteractableBase interactable)
    {
        if (isInteracting)
        {
            //QuitInteraction();
            return;
        }
        
        if (currentPointerTarget == interactable)
        {
            ClearPointerTarget();
        }
        
        // NOTE: This needs to be called before beginning the interaction, in case the interactable finishes interaction immediately. 
        onBeginInteractionEvent();
        
        currentInteractionTarget = interactable;
        currentInteractionTarget.OnBeginInteraction();
    }
    
    public void QuitInteraction()
    {
        if (isInteracting)
        {
            currentInteractionTarget.OnFinishInteraction();
            currentInteractionTarget = null;

            onFinishInteractionEvent();
        }
    }
    
    
    private void SetPointerTarget(InteractableBase interactable)
    {
        currentPointerTarget = interactable;
        currentPointerTarget.OnBecomePointerTarget();
    }
    
    private void ClearPointerTarget()
    {
        if (currentPointerTarget != null)
        {
            currentPointerTarget.OnNoLongerPointerTarget();
            if (IsInteractableWithinActivationRange(currentPointerTarget))
            {
                currentPointerTarget.OnExitActivationRange();
            }
            currentPointerTarget = null;
        }
    }

    public bool IsInteractableWithinActivationRange(InteractableBase interactable)
    {
        if (IsInteractableWithinViewAngle(interactable) == false)
        {
            return false;
        }
        return Vector3.Distance(interactable.GetLookTarget().position, viewOrigin.position) <= activationRange;
    }

    public bool IsInteractableWithinViewAngle(InteractableBase interactable)
    {
        if (interactable.visibleAngleRange.x >= 360f && interactable.visibleAngleRange.y >= 360f)
        {
            return true;
        }

        Transform lookTarget = interactable.GetLookTarget();
        Vector3 toInteractor = viewOrigin.position - lookTarget.position;
        
        // Y-angle range check.
        Vector3 projPlaneY = Vector3.ProjectOnPlane(toInteractor, lookTarget.up);
        float yAngle = Vector3.Angle(lookTarget.forward, projPlaneY);
        if (yAngle > interactable.visibleAngleRange.y / 2f)
        {
            return false;
        }
        // X-angle range check.
        Vector3 projPlaneX = Vector3.ProjectOnPlane(toInteractor, lookTarget.right);
        float xAngle = Vector3.Angle(lookTarget.forward, projPlaneX);
        if (xAngle > interactable.visibleAngleRange.x / 2f)
        {
            return false;
        }
        return true;
    }

    public void ProcessInteractableActivationRange(InteractableBase interactable)
    {
        if (IsInteractableWithinActivationRange(interactable))
        {
            if (interactable.isActivated == false)
            {
                interactable.OnEnterActivationRange();
            }
        }
        else
        {
            if (interactable.isActivated)
            {
                interactable.OnExitActivationRange();
                
                if (interactable == currentPointerTarget)
                {
                    ClearPointerTarget();
                }
            }
        }
    }
}
