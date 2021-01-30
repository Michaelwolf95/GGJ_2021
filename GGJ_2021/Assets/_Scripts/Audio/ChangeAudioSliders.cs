using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeAudioSliders : MonoBehaviour
{
    public Slider thisSlider;
    public static float masterVolume;
    public static float musicVolume;
    public static float SFXVolume;
    public static float AMBVolume;



    public void VolumeSliderChange(string whichSlider)
    {
        float sliderValue = thisSlider.value;

        if (whichSlider == "Master")
        {
            masterVolume = thisSlider.value;
            AkSoundEngine.SetRTPCValue("MasterVolume", masterVolume);
        }

        if (whichSlider == "Music")
        {
            musicVolume = thisSlider.value;
            AkSoundEngine.SetRTPCValue("MXVolume", musicVolume);
        }

        if (whichSlider == "SFX")
        {
            SFXVolume = thisSlider.value;
            AkSoundEngine.SetRTPCValue("SFXVolume", SFXVolume);
        }

        if (whichSlider == "AMB")
        {
            AMBVolume = thisSlider.value;
            AkSoundEngine.SetRTPCValue("AMBVolume", AMBVolume);
        }


    }
}
