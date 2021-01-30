using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTestSound : MonoBehaviour
{
    [Header("Wwise Events")]
    public AK.Wwise.Event TestSound;

    public void TestAudio()
    {
        TestSound.Post(gameObject);
    }
}
