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
    [SerializeField] private AudioClip GrowSound;
    [SerializeField] private AudioClip growLimit; //not implemented
    [SerializeField] private AudioClip shrinkSound;
    [SerializeField] private AudioClip shrinkLimit; //not implemented
    

    [SerializeField] private Image growthAmountImage;
    [SerializeField] private TMP_Text growthAmountText;
    [SerializeField] private float startingGrowthJuice = 10f;
    [SerializeField] [ReadOnly] private float currentGrowthJuice = 10f;
    [field:SerializeField] public Transform FirePoint { get; private set; }

    [HideInInspector] public enum ResizingState
    {
        Idle,
        Growing,
        Shrinking,
        Bounds
    }

    [HideInInspector] public ResizingState ResizeState;
    private ResizingState resizeStateLastFrame;

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

    private void Start()
    {
        InputManager.Instance.Grow.started += Grow_started;
        InputManager.Instance.Shrink.started += Shrink_started;
    }

    

    private void Update()
    {
        resizeStateLastFrame = ResizeState;
        ResizeState = ResizingState.Idle;

        bool leftClick = InputManager.Instance.LeftClickPressed();
        bool rightClick = InputManager.Instance.RightClickPressed();

        if (leftClick && rightClick) return; //don't allow both to be pressed at same time.

        if (!(leftClick || rightClick)) return;

        if (PickUpController.Instance.CurrentlyHolding) return;
        
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
            if (hit.rigidbody && hit.rigidbody.TryGetComponent(out InterractableObject interact))
            {
                //no juice and tryna grow
                if (currentGrowthJuice <= 0.01f && leftClick)
                {
                    CrosshairManager.Instance.Crosshair = CrosshairManager.Mode.X;
                    if (resizeStateLastFrame != ResizingState.Bounds)
                    {
                        if (growLimit != null)
                        {
                            AudioSource.PlayClipAtPoint(growLimit, FirePoint.position);
                        }
                    }
                    ResizeState = ResizingState.Bounds;
                    return; 
                }
                //we have juice and we're tryna shrink further, gun is full.
                if (currentGrowthJuice >= startingGrowthJuice - 0.01f && rightClick)
                {
                    CrosshairManager.Instance.Crosshair = CrosshairManager.Mode.X;
                    if (resizeStateLastFrame != ResizingState.Bounds)
                    {
                        if (shrinkLimit != null)
                        {
                            AudioSource.PlayClipAtPoint(shrinkLimit, FirePoint.position);
                        }
                    }
                    ResizeState = ResizingState.Bounds;
                    return; 
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

    private void Shrink_started(InputAction.CallbackContext obj)
    {
        if (shrinkSound == null) return;
        AudioSource.PlayClipAtPoint(shrinkSound,transform.position);
    }

    private void Grow_started(InputAction.CallbackContext obj)
    {
        if (GrowSound == null) return;
        AudioSource.PlayClipAtPoint(GrowSound, transform.position);
    }
}