using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoor : MonoBehaviour
{

    public GameObject movingDoor;

    [SerializeField] public float automaticDoorsXPosition = 0f;
    [SerializeField] public float movingSpeed = 5f;

    float maxDoorOpening = 1.6f;
    bool playerIsHere;

    
    void Start()
    {
        playerIsHere = false;
    }

    //currently hard coded to move in the x direction only, will work on z 
    void Update()
    {
        if (playerIsHere)
        {
            if (movingDoor.transform.position.x < maxDoorOpening + automaticDoorsXPosition)
            {
                movingDoor.transform.Translate((movingSpeed * Time.deltaTime), 0f, 0f);

            }
        }
        else
        {
            if (movingDoor.transform.position.x > automaticDoorsXPosition)
            {
                movingDoor.transform.Translate((-movingSpeed * Time.deltaTime), 0f, 0f);
            }
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
