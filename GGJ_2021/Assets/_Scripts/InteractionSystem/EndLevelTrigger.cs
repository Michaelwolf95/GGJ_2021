using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EndLevelTrigger : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput = null;
    [SerializeField] private ScreenFade screenFade = null;

    private void OnTriggerEnter(Collider other)
    {
        CheckFinishConditions(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckFinishConditions(other);
    }

    private void CheckFinishConditions(Collider other)
    {
        if (other.gameObject.tag == "Grabable")
        {
            GrabObject goal = other.gameObject.GetComponent<GrabObject>();
            if (goal == null)
            {
                Debug.LogError("Object tagged as goal does not have a GrabObject component attached!");
                return;
            }

            if (goal.isGrabbed)
                return;

            if(goal.isGoalObject)
            {
                playerInput.DeactivateInput();
                screenFade.FadeArtOut();
            }            
        }
    }
}
