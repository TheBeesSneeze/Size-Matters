/*******************************************************************************
 * File Name :         PickUpController.cs
 * Author(s) :         Toby Schamberger
 * Creation Date :     1/24/2024
 *
 * Brief Description : Handles the inputs and behaviour for picking up objects
 *
 * TODO:
 * - outline stuff
 *****************************************************************************/

using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class PickUpController : MonoBehaviour
{
    public static PickUpController Instance;

    [Header("Settings")]

    [Tooltip("The heaviest the player can carry")]
    public float MaxWeight;
    [Tooltip("*Before picking up the object*, how far the it can be from the player")]
    public float MaxPickupDistance;
    [Tooltip("How far the object can be from the player")]
    public float MaxDistanceFromPlayer;

    //actions stuff
    //public PlayerInput playerInput;
    //private InputAction PickUp;

    [SerializeField] private LayerMask NoObjectLM;
    [SerializeField] private Transform raycastOrigin;
    private InterractableObject currentlyHeldObject;
    private Vector3 holdPosition; //where the raycast hits

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        /*
        playerInput = GetComponent<PlayerInput>();
        PickUp = playerInput.currentActionMap.FindAction("Pickup");

        PickUp.started += pickup_started;
        */
    }

    private void Update()
    {
        bool pickup = InputManager.Instance.PickUpPressed();

        if(pickup)
        {
            if (currentlyHeldObject == null)
            {
                AttemptPickup();
                return;
            }
            DropObject();

        }
    }

    private void AttemptPickup()
    {
        //thanks alec for letting me steal your code
        Ray originPoint =
            raycastOrigin
                ? new Ray(raycastOrigin.position, raycastOrigin.forward)
                : Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(originPoint, out RaycastHit hit, MaxPickupDistance))
        {
            if (hit.rigidbody == null) return;

            if (hit.rigidbody.TryGetComponent(out InterractableObject interact))
            {
                //fumbles around and tries to find SOME reason not to do it
                if (interact == null) return;

                if (!interact.CanBePickedUp) return;

                if (interact.GetWeight() > MaxWeight) return;

                PickUpObject(interact);
            }
        }
    }

    private void PickUpObject(InterractableObject obj)
    {
        currentlyHeldObject = obj;

        currentlyHeldObject.GetComponent<Collider>().enabled=false;

        //snap object to center of players screen
        StartCoroutine(UpdateObjectPosition());
    }

    private void DropObject()
    {
        currentlyHeldObject = null;
        currentlyHeldObject.GetComponent<Collider>().enabled = true;
    }

    private IEnumerator UpdateObjectPosition()
    {
        while(currentlyHeldObject != null)
        {
            holdPosition = GetHoldPoint();

            currentlyHeldObject.transform.position = holdPosition;

            yield return null;
        }
    }

    private Vector3 GetHoldPoint()
    {
        Transform origin = Camera.main.transform;
        Ray ray = new Ray(origin.position, origin.forward);
        Physics.Raycast(ray, out RaycastHit hit, MaxDistanceFromPlayer, NoObjectLM);

        if (hit.transform == null)
        {
            return Camera.main.transform.forward * MaxDistanceFromPlayer;
        }

        return hit.point;
    }

    /// <summary>
    /// Returns how far away the currently held object is from the player.
    /// percent=0: exactly at the player
    /// percent=1: exactly MaxDistanceFromPlayer away from
    /// </summary>
    /// <returns></returns>
    private float GetPercentAwayFromPlayer()
    {
        if (currentlyHeldObject == null)
        {
            Debug.LogWarning("nothing is held!");
            return -1;
        }

        float d = Vector3.Distance(raycastOrigin.position, currentlyHeldObject.transform.position);

        return d / MaxDistanceFromPlayer;
    }
}
