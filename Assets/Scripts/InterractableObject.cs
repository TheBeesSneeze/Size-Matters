/*******************************************************************************
* File Name :         InterractableObject.cs
* Author(s) :         Toby Schamberger
* Creation Date :     1/22/2024
*
* Brief Description : Basic class which stores information about objects which
* can be picked up and resized.
* 
* feel free to change as much code as you want :) 
* 
* TODO:
* - Detect when player is looking at them
* - outlines with dynamic colors
* - resizing behaviour
* - how the fuck do we make the speed of resizing consistent
* - are we doing weight??? how the fuck do we calculate that
* - visual/audio effect for when object gets too big/too small
* - picking up and dropping stuff
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterractableObject : MonoBehaviour
{
    [Header("Settings")]

    [Tooltip("If the object can be picked up, when it is at an appropriate size")]
    public bool CanBePickedUp = true;
    public float MaxPickUpSize = 1.0f;

    [Tooltip("The default size is always 1.")]
    public float MaxSize = 10f;
    [Tooltip("The default size is always 1.")]
    public float MinSize = 0.1f;

    [Tooltip("Static variable. Units per second resized")]
    public static float ResizedSpeed;

    [Header("Debug")]
    public float CurrentSize = 1.0f;

    private Coroutine resizingCoroutine;
    private bool currentlyResizing;

    /// <summary>
    /// Returns the weight of the object, for pressure plates.
    /// </summary>
    /// <returns></returns>
    public float GetWeight()
    {
        //TODO: how the fuck are we calculating this
        return -1;
    }

    /// <summary>
    /// THIS FUNCTION DOES NOT GET CALLED YET!! ERASE THIS COMMENT WHEN IT DOES
    /// it should run when the player looks at the object tho. shoot a raycast or smth.
    /// </summary>
    public virtual void OnPlayerLooking()
    {
        //TODO
    }

    /// <summary>
    /// Call this function when the player is going to grow something.
    /// </summary>
    public void AttemptGrow()
    {
        //TODO: add code that checks for the players matter

        if (CurrentSize >= MaxSize)
        {
            //TODO: code that changes the outline?
            return;
        }

        if(resizingCoroutine != null || currentlyResizing)
        {
            StopResizing();
        }

        currentlyResizing = true;
        resizingCoroutine = StartCoroutine(GrowObject());
    }

    /// <summary>
    /// Call this function when the player is going to shrink something.
    /// </summary>
    public void AttemptShrink()
    {
        //TODO: add code that checks for the players matter

        if (CurrentSize <= MinSize)
        {
            //TODO: code that changes the outline?
            return;
        }

        if (resizingCoroutine != null || currentlyResizing)
        {
            StopResizing();
        }

        currentlyResizing = true;
        resizingCoroutine = StartCoroutine(ShrinkObject());
    }

    /// <summary>
    /// Same function for growing/shrinking
    /// </summary>
    public void StopResizing()
    {
        if (resizingCoroutine != null)
        {
            StopCoroutine(resizingCoroutine);
            resizingCoroutine = null;
        }

        currentlyResizing = false;
    }

    public void AttemptPickUp()
    {
        if(CurrentSize > MaxPickUpSize)
        {
            return;
        }

        PickUpItem();
    }

    protected virtual void PickUpItem()
    {
        //TODO
    }

    public void DropItem()
    {
        //TODO
    }

    protected IEnumerator GrowObject()
    {
        while(CurrentSize < MaxSize)
        {
            //TODO resizing math

            CurrentSize = Mathf.Clamp(CurrentSize, MinSize, MaxSize);
            yield return null;
        }
    }

    protected IEnumerator ShrinkObject()
    {
        while (CurrentSize > MinSize)
        {
            //TODO resizing math

            CurrentSize = Mathf.Clamp(CurrentSize, MinSize, MaxSize);
            yield return null;
        }
    }
}
