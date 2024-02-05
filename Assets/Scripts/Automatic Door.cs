using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoor : MonoBehaviour
{

    public GameObject movingDoor;

    bool playerIsHere;

    
    void Start()
    {
        playerIsHere = false;
    }

    
    void Update()
    {
        if (playerIsHere)
        {
            movingDoor.SetActive(false);
        }
        else
        {
            movingDoor.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerIsHere = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerIsHere = false;
        }
    }
}
