using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro; 
using UnityEngine;
using UnityEngine.InputSystem;


public class TutorialChat : MonoBehaviour
{

    [SerializeField]private bool doneFadeOut = true;
    [SerializeField]private TMP_Text helperText;

    [SerializeField]private Animator anim; 

    [SerializeField] private string[] startChat;

    public bool inProgress = true;
    private int introChatCount = 3; 
    private int currentChat = 0; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (doneFadeOut)
        {
            

            if(currentChat < introChatCount)
            {
                helperText.text = startChat[currentChat];
                anim.SetTrigger("FadeIn");
            }

            currentChat++;
            doneFadeOut = false;


        }
        
        if(currentChat > introChatCount)
        {
            inProgress = false; 
        }
        else
        {
            inProgress = true; 
        }
    }

    public void DoneFadeOut()
    {
        doneFadeOut = true;

        if(currentChat < introChatCount)
        StartFadeIn(); 
    }

    public void StartFadeIn()
    {
        anim.SetTrigger("FadeIn");

    }


    public void StartFadeOut()
    {
        anim.SetTrigger("FadeOut");
    }



    

}
