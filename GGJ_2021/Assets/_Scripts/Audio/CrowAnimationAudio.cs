using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowAnimationAudio : MonoBehaviour
{
    // Script to trigger audio with animation events

    [Header("Wwise Events")]

    public AK.Wwise.Event Wingflap;
    public AK.Wwise.Event Hop;

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
}
