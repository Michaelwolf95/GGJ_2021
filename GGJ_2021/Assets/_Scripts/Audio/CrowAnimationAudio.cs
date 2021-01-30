using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowAnimationAudio : MonoBehaviour
{
    [Header("Wwise Events")]

    public AK.Wwise.Event Wingflap;

    public void PlayWindFlap()
    {
        Wingflap.Post(gameObject);
    }
}
