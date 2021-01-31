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
        public AK.Wwise.Switch materialSwitch;
        public PhysicMaterial material;
    }

    private FootStepMaterial currentFootStepMaterial = null;

    [SerializeField] private FootStepMaterial[] footStepMaterials;

    [Header("Wwise Events")]
    public AK.Wwise.Event Wingflap;
    public AK.Wwise.Event footStepEvent;
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

    

    private void ResetHopInt()
    {
        // Nothing
    }

    public void OnGlideStart()
    {
        
    }
    
    public void OnGlideStop()
    {
        
    }

    public void PlayCaw()
    {
        Caw.Post(gameObject);
    }

    private void PlayHop() // Needs to be made.
    {
        footStepEvent.Post(gameObject);
    }
    
    public void PlayLandOnGroundSound()
    {
        footStepEvent.Post(gameObject); // Temp land sounds
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
                    stepMaterial.materialSwitch.SetValue(gameObject);
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
