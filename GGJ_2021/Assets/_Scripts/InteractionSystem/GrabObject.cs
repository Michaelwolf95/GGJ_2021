using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : InteractableBase
{
    public bool isGrabbed { get; protected set; }

    [SerializeField] private GameObject grabObjectTarget;

    protected override void PerformInteraction()
    {
        Debug.Log("Interacting");
        if(isGrabbed)
        {
            Drop();
        }
        else
        {
            Grab();
        }
        base.PerformInteraction();
    }

    private void Drop()
    {
        isGrabbed = false;
        this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    private void Grab()
    {
        isGrabbed = true;
        // Snap to beak position and disable rigidbody
        this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        //this.gameObject.transform.position = grabObjectTarget.transform.position;
        this.gameObject.transform.SetParent(grabObjectTarget.transform);
        
    }
}
