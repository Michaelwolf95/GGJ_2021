using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabObject : InteractableBase
{
    protected override void PerformInteraction()
    {
        Debug.Log("Interacting");
        base.PerformInteraction();
    }
}
