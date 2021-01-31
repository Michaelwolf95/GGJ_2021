using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndLevelTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");
        CheckFinishConditions(other);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Stay");
        CheckFinishConditions(other);
    }

    private void CheckFinishConditions(Collider other)
    {
        if (other.gameObject.tag == "GoalObject")
        {
            GrabObject goal = other.gameObject.GetComponent<GrabObject>();
            if (goal == null)
            {
                Debug.LogError("Object tagged as goal does not have a GrabObject component attached!");
                return;
            }

            if (goal.isGrabbed)
                return;

            Debug.Log("Finish!");
            // TODO: Add end level logic
        }
    }
}
