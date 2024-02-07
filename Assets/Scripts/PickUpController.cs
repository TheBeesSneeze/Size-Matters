/*******************************************************************************
 * File Name :         PickUpController.cs
 * Author(s) :         Toby Schamberger
 * Creation Date :     1/24/2024
 *
 * Brief Description : Handles the inputs and behaviour for picking up objects
 *
 * TODO:
 * - outline stuff?
 * - disable resizing (you cant resize held object but you CAN resize other stuff)
 * - crosshair icon
 * - stop rotating velocity when picked up
 * 
 * TEST:
 *****************************************************************************/

using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using digitalopus.physics.kinematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

public class PickUpController : MonoBehaviour
{
    public static PickUpController Instance;

    [Header("Settings")] [Tooltip("The heaviest the player can carry")]
    public float MaxWeight;

    [Tooltip("*Before picking up the object*, how far the it can be from the player")]
    public float MaxPickupDistance;

    [Tooltip("How far the object can be from the player")]
    public float MaxDistanceFromPlayer;

    [Tooltip("true for movement via velocity (good), false for snapping to raycast point(bad")] [SerializeField]
    private bool MoveViaPhysics = true; //kill your babies

    //actions stuff
    //public PlayerInput playerInput;
    //private InputAction PickUp;

    [SerializeField] private LayerMask NoObjectLM;
    [SerializeField] private Transform raycastOrigin;

    [SerializeField] private float spring = 100;
    [SerializeField] private float damper = 10;
    [SerializeField] private float maxForce = 100;
    [SerializeField] private SPDVector3CalculatorV2 forcePD;
    [SerializeField] private Transform holdPoint;
    [Foldout("Audio")]
    [SerializeField] private AudioClip pickupClip;
    [Foldout("Audio")]
    [SerializeField] private float pickupClipVolume = 0.6f;
    [Foldout("Audio")]
    [SerializeField] private AudioClip dropClip;
    [Foldout("Audio")]
    [SerializeField] private float dropClipVolume = 0.6f;

    private InterractableObject currentlyHeldObject;
    private Vector3 holdPosition; //where the raycast hits
    private float startingPlayerYRotation, startingObjectYRotation;

    private Rigidbody objectRB;
    [HideInInspector] public bool CurrentlyHolding;
    private Vector3 LastPosition;

    private float maxDistanceForMovement = 0.05f;

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
        InputManager.Instance.mainControls.StandardLayout.Pickup.started += PickUp_Started;
    }

    private void PickUp_Started(InputAction.CallbackContext obj)
    {
        if (currentlyHeldObject == null)
        {
            CrosshairManager.Instance.Crosshair = CrosshairManager.Mode.X;
            AttemptPickup();
            return;
        }

        DropObject();
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
            //float distance = Vector3.Distance(raycastOrigin.position, hit.transform.position);

            //if (distance > maxDistanceForMovement) return;

            if (hit.rigidbody == null)
            {
                CrosshairManager.Instance.Crosshair = CrosshairManager.Mode.X;
                return;
            }

            if (hit.rigidbody.TryGetComponent(out InterractableObject interact))
            {
                //fumbles around and tries to find SOME reason not to do it
                if (interact == null) return;

                if (!interact.CanBePickedUp) return;

                if (interact.GetWeight() > MaxWeight) return;

                PickUpObject(interact);
                return;
            }
        }
        CrosshairManager.Instance.Crosshair = CrosshairManager.Mode.X;
    }

    private void PickUpObject(InterractableObject obj)
    {
        CurrentlyHolding = true;
        if (pickupClip)
        {
            AudioSource.PlayClipAtPoint(pickupClip, transform.position, pickupClipVolume);
        }
        currentlyHeldObject = obj;

        startingPlayerYRotation = transform.rotation.eulerAngles.y;
        startingObjectYRotation = currentlyHeldObject.transform.rotation.eulerAngles.y;

        currentlyHeldObject.transform.gameObject.layer = 2;

        objectRB = currentlyHeldObject.GetComponent<Rigidbody>();
        objectRB.useGravity = false;
        objectRB.constraints = RigidbodyConstraints.FreezeRotation;
        LastPosition = holdPoint.position;

        //snap object to center of players screen

        if (!MoveViaPhysics)
            StartCoroutine(UpdateObjectPosition());

        CrosshairManager.Instance.Crosshair = CrosshairManager.Mode.Fill;
    }

    private void DropObject()
    {
        CurrentlyHolding = false;

        if (dropClip)
        {
            AudioSource.PlayClipAtPoint(dropClip, transform.position, dropClipVolume);
        }

        currentlyHeldObject.transform.gameObject.layer = 0;
        currentlyHeldObject.GetComponent<Collider>().enabled = true;
        objectRB.useGravity = true;
        objectRB.constraints = RigidbodyConstraints.None;
        objectRB.transform.position = holdPoint.position;

        if (currentlyHeldObject.NoThrow)
        {
            //objectRB.velocity = Vector3.zero;
            objectRB.velocity = objectRB.velocity / 5;
        }

        currentlyHeldObject = null;

        CrosshairManager.Instance.Crosshair = CrosshairManager.Mode.Hand;
    }

    private IEnumerator UpdateObjectPosition()
    {
        currentlyHeldObject.GetComponent<Collider>().enabled = false;
        while (currentlyHeldObject != null)
        {
            holdPosition = GetHoldPoint();

            currentlyHeldObject.transform.position = holdPosition;

            RotateHeldObject();

            yield return null;
        }
    }

    private void FixedUpdate()
    {
        if (!MoveViaPhysics || !CurrentlyHolding) return;

        holdPosition = holdPoint.position;

        forcePD.dt = Time.fixedDeltaTime;
        forcePD.kd = damper;
        forcePD.kp = spring;
        forcePD.mass = objectRB.mass;
        Vector3 targetVelocity = (holdPosition - LastPosition) / Time.fixedDeltaTime;
        objectRB.AddForce(forcePD.ComputSPDForce(objectRB.transform.position, objectRB.velocity, holdPosition,
            targetVelocity));
        LastPosition = holdPosition;
        RotateHeldObject();
        // if(Vector3.Distance(holdPosition, currentlyHeldObject.transform.position) <= maxDistanceForMovement)
        // {
        //     currentlyHeldObject.transform.position = holdPosition;
        //     objectRB.velocity = Vector3.zero;
        //     LastPosition = holdPosition;
        //     return;
        // }
        //
        // Vector3 direction = holdPosition - currentlyHeldObject.transform.position;
        //
        // direction = direction * spring;
        //
        // Vector3 Damper = (targetVelocity - objectRB.velocity) * damper;
        //
        // Vector3 force = direction + Damper;
        // force = Vector3.ClampMagnitude(force, maxForce);

       // objectRB.AddForce(force, ForceMode.Acceleration);


        //objectRB.velocity = direction.normalized * 10;
        //objectRB.AddForce(direction.normalized*100);

        //rb.MovePosition(holdPosition);

    }

    private Vector3 GetHoldPoint()
    {
        Transform origin = Camera.main.transform;
        Ray ray = new Ray(origin.position, origin.forward);

        return origin.position + (origin.forward * MaxDistanceFromPlayer);
        Physics.Raycast(ray, out RaycastHit hit, MaxDistanceFromPlayer); //, NoObjectLM);

        if (hit.transform == null)
        {
        }

        return hit.point;
    }

    private void RotateHeldObject()
    {
        float difference = transform.rotation.eulerAngles.y - startingPlayerYRotation;

        Vector3 newObjectRotation = currentlyHeldObject.transform.rotation.eulerAngles;
        newObjectRotation.y = startingObjectYRotation + difference;

        //currentlyHeldObject.transform.rotation.eulerAngles = newObjectRotation;
        currentlyHeldObject.transform.rotation = Quaternion.Euler(newObjectRotation);
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