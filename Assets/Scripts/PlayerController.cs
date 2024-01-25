using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    //this is using the unity provided CharacterControler.Move() 
    //and a youtube video I found to help me code this up 
    //I will provide the links 
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    private InputManager inputManager; //isnt the point of singletons to not have this variable?
    
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float lookSensitivity = 50f;
    [SerializeField] private float pushForce = 30f;
    [SerializeField] private Camera cam;
    private float xMovement;
    private float yMovement;
    
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }


        yMovement += inputManager.GetPlayerLook().y * lookSensitivity * Time.deltaTime;
        xMovement += inputManager.GetPlayerLook().x * lookSensitivity * Time.deltaTime;
        yMovement = Mathf.Clamp(yMovement, -90, 90);
        cam.transform.localEulerAngles = new Vector3(-yMovement, 0f, 0f);
        transform.eulerAngles = new Vector3(0f, xMovement, 0f);

        Vector2 movement = inputManager.GetPlayerMovement();
        Vector3 move = transform.TransformDirection(new Vector3(movement.x, 0f, movement.y));
        controller.Move(move * Time.deltaTime * playerSpeed);
        // if (move != Vector3.zero)
        // {
        //     gameObject.transform.forward = move;
        // }

        // Changes the height position of the player..
        if (inputManager.PlayerJumpedThisFrame() && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.rigidbody != null)
        {
            hit.rigidbody.velocity = hit.moveDirection * pushForce;
        }
    }
}
