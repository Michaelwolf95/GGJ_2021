using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowAnimationAudio : MonoBehaviour
{
    // Script to trigger audio with animation events

    [Header("Wwise Events")]

    public AK.Wwise.Event Wingflap;

    public void PlayWindFlap()
    {
        Wingflap.Post(gameObject);
    }
}
