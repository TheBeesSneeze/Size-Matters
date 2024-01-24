/*******************************************************************************
 * File Name :         GrowthGun.cs
 * Author(s) :         Alec Pizzifero
 * Creation Date :     1/22/2024
 *
 * Brief Description : 
 *
 * TODO:
 * - 
 *****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GrowthGun : MonoBehaviour
{
    [SerializeField] [Required("If null, screen center will be used.")]
    private Transform raycastOrigin;

    private MainControls mainControls; //should prolly singleton this.

    [SerializeField] [Required("Required for screen raycasting.")]
    private Camera cam;

    [SerializeField] private float maxRaycastDistance = 30f;

    [SerializeField] [Tooltip("How fast to grow or shrink")]
    private float scaleRate = 10f;

    [SerializeField] [Tooltip("If enabled, the mouse position will be used instead of the center of the screen")]
    private bool useMousePosition;

    [SerializeField] private Image growthAmountImage;
    [SerializeField] [Range(0f, 1f)] private float growthAmountTest;
    [SerializeField] private float startingGrowthJuice = 10f;
    [SerializeField] [ReadOnly] private float currentGrowthJuice = 10f;

    private void Awake()
    {
        mainControls = new MainControls();
        mainControls.Enable();
        currentGrowthJuice = startingGrowthJuice;
    }

    private void Update()
    {
        bool leftClick = mainControls.StandardLayout.Grow.IsPressed();
        bool rightClick = mainControls.StandardLayout.Shrink.IsPressed();

        if (leftClick && rightClick) return; //don't allow both to be pressed at same time.

        if (leftClick || rightClick)
        {
            float sign = leftClick ? 1f : -1f;
            Ray originPoint =
                raycastOrigin
                    ? new Ray(raycastOrigin.position, raycastOrigin.forward)
                    : cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            if (useMousePosition)
            {
                originPoint = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            }

            if (Physics.Raycast(originPoint, out RaycastHit hit, maxRaycastDistance))
            {
                if (hit.rigidbody == null) return;
                if (hit.rigidbody.TryGetComponent(out InterractableObject interact))
                {
                    if (currentGrowthJuice <= 1.1f && leftClick) return; //no juice and tryna grow
                    if (currentGrowthJuice >= startingGrowthJuice + 0.1f && rightClick) return; //we have juice and we're tryna shrink further, gun is full.
                    float change = interact.GrowOrShrink(sign *
                                                         scaleRate); //no need for a delta time, we handle it at the object level.
                    currentGrowthJuice = Mathf.Clamp(currentGrowthJuice + change, 0f, startingGrowthJuice);
                }
            }
        }

        growthAmountImage.fillAmount = currentGrowthJuice / startingGrowthJuice;
    }
}