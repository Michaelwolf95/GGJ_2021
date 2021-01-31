using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : InteractableBase
{
    public bool isGrabbed { get; protected set; }

    [SerializeField] private GameObject grabObjectTarget;

    protected override void Start()
    {
        base.Start();
        isGrabbed = false;
    }

    protected override void PerformInteraction()
    {
        Debug.Log("Grabbed: " + isGrabbed);
        if(isGrabbed)
        {
            Debug.Log("Dropping...");
            Drop();
        }
        else
        {
            Debug.Log("Grabbing...");
            Grab();
        }
        base.PerformInteraction();
    }

    private void Drop()
    {
        isGrabbed = false;
        this.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        this.gameObject.transform.parent = null;
        Interactor.instance.heldObject = null;
    }

    private void Grab()
    {
        isGrabbed = true;
        // Snap to beak position and disable rigidbody
        this.gameObject.GetComponent<Rigidbody>().isKinematic = true;
        this.gameObject.transform.SetParent(grabObjectTarget.transform);
        this.gameObject.transform.localPosition = Vector3.zero;
        Interactor.instance.heldObject = this;

    }
}
