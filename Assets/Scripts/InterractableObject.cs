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
 * - resizing behaviour done
 * - how the fuck do we make the speed of resizing consistent use a smooth dampen
 * - are we doing weight??? how the fuck do we calculate that use rb mass
 * - visual/audio effect for when object gets too big/too small
 * - picking up and dropping stuff done
 *****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InterractableObject : MonoBehaviour
{
    [Header("Settings")] 
    [Tooltip("If the object can be picked up, when it is at an appropriate size")]
    public bool CanBePickedUp = true;
    [Tooltip("If the object can be pushed by the player. Other forces will still push")]
    public bool CanBePushed = true;
    [Tooltip("If the object can be thrown")]
    public bool NoThrow;

    public float MaxPickUpSize = 1.0f;

    [Header("Scale Changes")]
    [Tooltip("Initial scale * this value, how big the object can get.")] [SerializeField]
    private float maxScaleMultiplier = 10f;
    [Tooltip("Initial scale * this value, how small the object can get.")] [SerializeField]
    private float minScaleMultiplier = 0.1f;

    [Tooltip("False: object can't shrink in _ direction")] [SerializeField] private bool scaleX = true, scaleY = true, scaleZ = true;
    //[Tooltip("False: object can't shrink in y directions")] [SerializeField] private bool scaleY = true;
    //[Tooltip("False: object can't shrink in z directions")] [SerializeField] private bool scaleZ = true;

    [SerializeField] [Tooltip("Multiplier so individual objects can grow/shrink faster if desired")]
    private float scaleChangeRateMultiplier = 1f;

    [SerializeField] private float scaleChangeSmoothingTime = 0.1f;

    [Header("Debug")] public float CurrentSize = 1.0f; //inv lerp for getting current size.

    [HideInInspector] public bool PlayerLooking;
    [HideInInspector] public Vector3 StartingPosition;

    private Rigidbody rb;
    private Vector3 initScale;
    private Vector3 minScale, maxScale;
    [ReadOnly] [SerializeField] private Vector3 scaleTarget;
    private Vector3 scaleDampRef;
    private float initMass;
    private OutlineBehavior outline;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        initMass = rb.mass;

        StartingPosition = transform.position;

        outline = GetComponent<OutlineBehavior>();
        initScale = transform.localScale;
        scaleTarget = initScale;

        SetBoundingScales();
    }

    private void FixedUpdate()
    {
        //smooths any changes in scale.s
        transform.localScale = Vector3.SmoothDamp(initScale, scaleTarget, ref scaleDampRef, scaleChangeSmoothingTime);
    }

    /// <summary>
    /// Returns the weight of the object, for pressure plates.
    /// </summary>
    /// <returns></returns>
    public float GetWeight()
    {
        return rb.mass;
    }

    /// <summary>
    /// it should run when the player looks at the object tho. shoot a raycast or smth.
    /// </summary>
    public virtual void OnPlayerLooking()
    {
        PlayerLooking = true;

        if (outline == null)
        {
            Debug.LogWarning("no outline? :(");
            return;
        }
        outline.CorrectOutline();

        CrosshairManager.Instance.Crosshair = CrosshairManager.Mode.Fill;

        CrosshairManager.Instance.StartCoroutine(CrosshairManager.Instance.CheckIfPlayerLooking(this));


    }

    public virtual void OnPlayerLookingExit()
    {
        PlayerLooking = false;

        if (outline!= null)
            outline.StopOutlining();

        if(!PickUpController.Instance.CurrentlyHolding)
            CrosshairManager.Instance.Crosshair = CrosshairManager.Mode.Empty;
    }

    /// <summary>
    /// Try to grow or shrink something.
    /// </summary>
    /// <param name="rate">How fast and in what direction to grow/shrink.</param>
    /// <returns></returns>
    public float GrowOrShrink(float rate)
    {
        SetBoundingScales();

        float change = rate * Time.deltaTime * scaleChangeRateMultiplier;
        var newScale = scaleTarget;
        newScale.x = Mathf.Clamp(newScale.x + change, minScale.x,
            maxScale.x);
        newScale.y = Mathf.Clamp(newScale.y + change, minScale.y,
            maxScale.y);
        newScale.z = Mathf.Clamp(newScale.z + change, minScale.z,
            maxScale.z);
        scaleTarget = newScale;

        if(rb != null)
            rb.mass = initMass * (scaleTarget.magnitude / initScale.magnitude);

        float prevSize = CurrentSize;
        CurrentSize = (scaleTarget.magnitude / initScale.magnitude);
        return prevSize - CurrentSize;
    }

    [Button]
    private void TEST_Grow()
    {
        //SetBoundingScales();
        GrowOrShrink(10f);
    }

    [Button]
    private void TEST_Shrink()
    {
        //SetBoundingScales();
        GrowOrShrink(-10f);
    }

    private void SetBoundingScales()
    {
        minScale = initScale * minScaleMultiplier;
        maxScale = initScale * maxScaleMultiplier;

        if (!scaleX) { minScale.x = initScale.x; maxScale.x = initScale.x; }
        if (!scaleY) { minScale.y = initScale.y; maxScale.y = initScale.y; }
        if (!scaleZ) { minScale.z = initScale.z; maxScale.z = initScale.z; }
    }
}