using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    //guys apparantly scene variables don't work ??? evil
    public enum Scene
    {
        Hub,
        Adam,
        Andrea,
        Jaxson,
        Jamison,
        Tutorial
    }

    public Scene OutputScene;


    private void OnTriggerEnter(Collider collision)
    {
        string tag = collision.gameObject.tag;

        string scene = SceneManager.GetActiveScene().name;

        PlayerPrefs.SetInt(scene + "isDone", 1);

        if (tag.Equals("Player"))
        {
            if (OutputScene == Scene.Hub)
            {
                SceneManager.LoadScene("HubScene");
            }

            if (OutputScene == Scene.Adam)
            {
                SceneManager.LoadScene("AdamPuzzle");
            }

            if (OutputScene == Scene.Andrea)
            {
                SceneManager.LoadScene("AndreaPuzzle");
            }

            if (OutputScene == Scene.Jaxson)
            {
                SceneManager.LoadScene("JaxsonsLevel");
            }

            if (OutputScene == Scene.Jamison)
            {
                SceneManager.LoadScene("ParkourScene");
            }

            if (OutputScene == Scene.Tutorial)
            {
                SceneManager.LoadScene("TutorialScene");
            }
        }

        return;
        
    }
}
