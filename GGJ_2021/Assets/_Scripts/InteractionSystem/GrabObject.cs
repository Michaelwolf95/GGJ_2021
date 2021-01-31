using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : InteractableBase
{
    public bool isGrabbed { get; protected set; }
    public bool isGoalObject = false;

    [SerializeField] private GameObject grabObjectTarget;

    protected override void Start()
    {
        base.Start();
        isGrabbed = false;

        if (grabObjectTarget == null)
        {
            grabObjectTarget = this.gameObject;
        }
    }

    protected override void PerformInteraction()
    {
        Debug.Log("Grabbed: " + isGrabbed);
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
        this.gameObject.transform.parent = null;
        Interactor.instance.heldObject = null;
        UnhideReticle();
    }

    private void Grab()
    {
        isGrabbed = true;
        // Snap to beak position and disable rigidbody
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.transform.SetParent(Interactor.instance.grabPoint);

        transform.rotation = Interactor.instance.grabPoint.rotation * Quaternion.Inverse(Quaternion.Inverse(transform.rotation) * grabObjectTarget.transform.rotation);
        transform.position = Interactor.instance.grabPoint.position + (transform.position - grabObjectTarget.transform.position);
        Interactor.instance.heldObject = this;
        HideReticle();
    }
}
