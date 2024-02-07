/*******************************************************************************
 * File Name :         DeathPlane.cs
 * Author(s) :         Jamison Parks, Toby Schamberger
 * Creation Date :     2/7/2024
 *
 * Brief Description : Resets the scene when collides with player. Sets Interractable
 * Objects back to their starting position.
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathPlane : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if(player != null)
        {
            OnPlayerCollision(player);
            return;
        }

        InterractableObject obj = collision.gameObject.GetComponent<InterractableObject>();
        if (obj != null)
        {
            OnObjectCollision(obj);
            return;
        }
    }

    private void OnPlayerCollision(PlayerController player)
    {
        Debug.Log("player died. reload scene!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnObjectCollision(InterractableObject obj)
    {
        obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        obj.transform.position = obj.StartingPosition;
    }
}
