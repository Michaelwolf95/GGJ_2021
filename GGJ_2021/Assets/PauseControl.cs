using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using MichaelWolfGames;
using UnityEngine;

public class PauseControl : MonoBehaviour
{
    [SerializeField] private bool isPaused = false;


    [SerializeField] private Canvas pauseMenu;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private EventButton pauseButton;
    [SerializeField] private GameObject cinemachineFreeLook; 

    // Start is called before the first frame update
    void Start()
    {
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        pauseButton = new EventButton(playerInput, "Pause");
        cinemachineFreeLook = GameObject.FindGameObjectWithTag("LookCam");

    }

    // Update is called once per frame
    void Update()
    {
        pauseButton.HandleUpdate(Time.deltaTime);

        if (pauseButton.isPressedDown)
        {
            Debug.Log("Pause");
            Pause(); 
        }
    }



    public void Pause()
    {
        
        if (!isPaused)
        {
            pauseMenu.gameObject.SetActive(true);
            cinemachineFreeLook.SetActive(false);
            Time.timeScale = 0;
            isPaused = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            pauseMenu.gameObject.SetActive(false);
            cinemachineFreeLook.SetActive(true);
            Time.timeScale = 1;
            isPaused = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void OnResume()
    {
        Time.timeScale = 1; 
    }
}
