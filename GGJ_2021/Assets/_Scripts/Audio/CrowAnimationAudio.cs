using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowAnimationAudio : MonoBehaviour
{
    // Script to trigger audio with animation events
    
    // DONT CHANGE THESE PLS
    [Serializable]
    public class FootStepMaterial
    {
        public AK.Wwise.Event footStepEvent;
        public PhysicMaterial material;
    }

    [SerializeField] private FootStepMaterial dirtFootStepMaterial;
    private FootStepMaterial currentFootStepMaterial = null;
    private FootStepMaterial defaultFootStepMaterial => dirtFootStepMaterial;

    [SerializeField] private FootStepMaterial[] footStepMaterials;

    [Header("Wwise Events")]
    public AK.Wwise.Event Wingflap;
    public AK.Wwise.Event Hop;
    public AK.Wwise.Event Caw; 

    private bool isGrounded = false;
    
    private void Awake()
    {
        currentFootStepMaterial = (footStepMaterials.Length > 0)? footStepMaterials[0] : null;
    }

    public void PlayWingFlap()
    {
        Wingflap.Post(gameObject);
    }

    public void PlayHop()
    {
        Hop.Post(gameObject);
    }

    private void ResetHopInt()
    {
        // Nothing
    }

    public void OnLandOnGround()
    {
        
    }

    public void PlayCaw()
    {
        Caw.Post(gameObject);
    }

    private void HopAnimationEvent() // Needs to be made.
    {
        if (currentFootStepMaterial != null)
        {
            currentFootStepMaterial.footStepEvent.Post(gameObject);
        }
    }

    public void HandleControllerColliderHit(CharacterController controller, ControllerColliderHit hit)
    {
        isGrounded = controller.isGrounded;
        
        if (isGrounded)
        {
            bool hasMaterial = false;
            foreach (FootStepMaterial stepMaterial in footStepMaterials)
            {
                if (hit.collider.sharedMaterial == stepMaterial.material)
                {
                    currentFootStepMaterial = stepMaterial;
                    hasMaterial = true;
                    break;
                }
            }

            if (hasMaterial == false)
            {
                // ToDo: Default material? Or keep current?
                //currentFootStepMaterial = 
            }
        }
    }
    
    
    
}
