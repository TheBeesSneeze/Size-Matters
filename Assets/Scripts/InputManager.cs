/*******************************************************************************
 * File Name :         InputManager.cs
 * Author(s) :         
 * Creation Date :     1/22/2024
 *
 * Brief Description : 
 *
 * TODO:
 * - 
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance; 

    public static InputManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private MainControls mainControls;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        mainControls = new MainControls();
    }

    private void OnEnable()
    {
        mainControls.Enable();
    }

    private void OnDisable()
    {
        mainControls.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return mainControls.StandardLayout.Movement.ReadValue<Vector2>();
    }

    public Vector2 GetPlayerLook()
    {
        return mainControls.StandardLayout.Look.ReadValue<Vector2>();
    }

    public bool PlayerJumpedThisFrame()
    {
        return mainControls.StandardLayout.Jump.IsPressed(); 
    }

    public bool LeftClickPressed()
    {
        return mainControls.StandardLayout.Grow.IsPressed();
    }

    public bool RightClickPressed()
    {
        return mainControls.StandardLayout.Shrink.IsPressed();
    }

    public bool PickUpPressed()
    {
        return mainControls.StandardLayout.Pickup.IsPressed();
    }
}
