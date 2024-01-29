/*******************************************************************************
 * File Name :         GrowthGun.cs
 * Author(s) :         Alec Pizzifero
 * Creation Date :     1/22/2024
 *
 * Brief Description : 
 *
 * TODO:
 * - Format code to use new input system / not update
 *****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class GrowthGun : MonoBehaviour
{
    public static GrowthGun Instance;

    [SerializeField] [Required("If null, screen center will be used.")]
    private Transform raycastOrigin;

    [SerializeField] [Required("Required for screen raycasting.")]
    private Camera cam;

    [SerializeField] public float maxRaycastDistance = 30f;

    [SerializeField] [Tooltip("How fast to grow or shrink")]
    private float scaleRate = 10f;

    [SerializeField] [Tooltip("If enabled, the mouse position will be used instead of the center of the screen")]
    private bool useMousePosition;

    [Header("Sound")]


    [SerializeField] private Image growthAmountImage;
    [SerializeField] private TMP_Text growthAmountText;
    [SerializeField] private float startingGrowthJuice = 10f;
    [SerializeField] [ReadOnly] private float currentGrowthJuice = 10f;

    [HideInInspector] public enum ResizingState
    {
        Idle,
        Growing,
        Shrinking,
        Bounds
    }

    [HideInInspector] public ResizingState ResizeState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        currentGrowthJuice = startingGrowthJuice;
    }

    private void Update()
    {
        ResizeState = ResizingState.Idle;

        bool leftClick = InputManager.Instance.LeftClickPressed();
        bool rightClick = InputManager.Instance.RightClickPressed();

        if (leftClick && rightClick) return; //don't allow both to be pressed at same time.

        if (!(leftClick || rightClick)) return;

        
        WhileClicking(leftClick, rightClick);
    }

    /// <summary>
    /// sorry for abstracting ur code
    /// </summary>
    protected void WhileClicking(bool leftClick, bool rightClick)
    {
        if (leftClick)
        {
            ResizeState = ResizingState.Growing;
        }

        if (rightClick)
        {
            ResizeState = ResizingState.Shrinking;
        }

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

                if (currentGrowthJuice <= 0.1f && leftClick)
                {
                    ResizeState = ResizingState.Bounds;
                    return; //no juice and tryna grow
                }
                //there is a problem here!!!                     V
                if (currentGrowthJuice >= startingGrowthJuice + 0.1f && rightClick)
                {
                    ResizeState = ResizingState.Bounds;
                    return; //we have juice and we're tryna shrink further, gun is full.
                }

                float change = interact.GrowOrShrink(sign * scaleRate); //no need for a delta time, we handle it at the object level.

                currentGrowthJuice = Mathf.Clamp(currentGrowthJuice + change, 0f, startingGrowthJuice);
            }
        }

        UpdateGunUI();
    }

    protected void UpdateGunUI()
    {
        growthAmountImage.fillAmount = currentGrowthJuice / startingGrowthJuice;
        growthAmountText.text = ((int)(currentGrowthJuice / startingGrowthJuice * 100)).ToString()+"%";
    }
}