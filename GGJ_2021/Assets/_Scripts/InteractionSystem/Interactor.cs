using System;
using System.Collections;
using System.Collections.Generic;
using MichaelWolfGames;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    public static Interactor instance = null;
    
    [SerializeField] private Camera mainCamera = null;
    [SerializeField] private float activationRange = 2f;
    [SerializeField] private float interactRange = 1f;

    [SerializeField] private LayerMask interactionLayerMask = new LayerMask();
    
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
    
    private void Awake()
    {
        instance = this;
        
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
        
        Ray ray = new Ray(viewOrigin.position, viewOrigin.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, interactRange, interactionLayerMask, QueryTriggerInteraction.Collide);
        if (hits.Length > 0)
        {
            InteractableBase raycastResultTarget = null;
            float nearestDist = float.MaxValue;
            for (int i = 0; i < hits.Length; i++)
            {
                InteractableBase raycastInteractable = hits[i].collider.GetComponent<InteractableBase>();
                if (raycastInteractable == null && hits[i].rigidbody != null)
                {
                    raycastInteractable = hits[i].rigidbody.GetComponent<InteractableBase>();
                }
                if (raycastInteractable != null && raycastInteractable.isInteractable)
                {
                    if (hits[i].distance < nearestDist && IsInteractableWithinViewAngle(raycastInteractable))
                    {
                        raycastResultTarget = raycastInteractable;
                        nearestDist = hits[i].distance;
                    }
                }
            }

            if (raycastResultTarget != null)
            {
                if (currentPointerTarget != null && currentPointerTarget != raycastResultTarget)
                {
                    ClearPointerTarget();
                }

                SetPointerTarget(raycastResultTarget);
            }
        }
        else
        {
            if (currentPointerTarget != null)
            {
                ClearPointerTarget();
            }
        }
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
