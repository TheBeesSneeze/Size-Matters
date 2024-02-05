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
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    [HideInInspector] public InputAction Look;
    [HideInInspector] public InputAction Grow;
    [HideInInspector] public InputAction Shrink;
    [HideInInspector] public InputAction Movement;

    private static InputManager _instance;

    private InterractableObject currentlyViewedObject;

    public static InputManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public MainControls mainControls;
    private Camera cam;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        
        cam = Camera.main;

        mainControls = new MainControls();
        mainControls.StandardLayout.Quit.performed += context => { Application.Quit(); };
        mainControls.StandardLayout.Restart.performed += context =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        };
        
    }

    private void Start()
    {
        Look = mainControls.StandardLayout.Look;
        Grow = mainControls.StandardLayout.Grow;
        Shrink = mainControls.StandardLayout.Shrink;
        Movement = mainControls.StandardLayout.Movement;
    }

    private void Update()
    {
        DetectLookingObject();
    }

    /// <summary>
    /// raycasting
    /// </summary>
    public void DetectLookingObject()
    {
        //thanks alec for letting me steal your code
        Ray originPoint =
                 cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.Raycast(originPoint, out RaycastHit hit, GrowthGun.Instance.maxRaycastDistance))
        {
            if (hit.rigidbody && hit.rigidbody.TryGetComponent(out InterractableObject interact))
            {
                if (currentlyViewedObject != null && interact != currentlyViewedObject)
                {
                    currentlyViewedObject.OnPlayerLookingExit();
                    currentlyViewedObject = null;
                }
                interact.OnPlayerLooking();
                currentlyViewedObject = interact;
                return;
            }
        }
     
        if (currentlyViewedObject != null)
        { 
            currentlyViewedObject.OnPlayerLookingExit();
            currentlyViewedObject = null;
        }
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
