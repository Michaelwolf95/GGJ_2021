using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    [Header("Wwise Events")]
    public AK.Wwise.Event buttonPress;

    public AK.Wwise.Event buttonHover;
    

    public void ButtonPress()
    {
        buttonPress.Post(gameObject);
    }
    public void ButtonHover()
    {
        buttonHover.Post(gameObject);
    }
    
}
