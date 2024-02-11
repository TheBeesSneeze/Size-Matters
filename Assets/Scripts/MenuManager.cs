/*******************************************************************************
 * File Name :         MenuManager.cs
 * Author(s) :         Toby Schamberger
 * Creation Date :     2/5/2024
 *
 * Brief Description : Used on pause menu, feel free to use it for more than that
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject PauseMenuCanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void OnResumeClick()
    {
        Debug.Log("unpause");
        PauseMenuCanvas.SetActive(false);
        InputManager.Instance.Unpause();
    }

    public void GoToHub()
    {
        Debug.LogWarning("not not done");
        SceneManager.LoadScene("HubScene");
    }

    public void OnExitClick()
    {
        Application.Quit();
    }
}
