using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
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

    [Header("Sounds")] [SerializeField] [Foldout("Audio")]
    private AudioClip JumpSound;

    [SerializeField] private float viewBobAmount = 0.05f;
    [SerializeField] [Foldout("Audio")] private float jumpVolume = 0.8f;
    [SerializeField] [Foldout("Audio")] private AudioClip WalkingSound;
    [SerializeField] [Foldout("Audio")] private float walkingVolume = 0.8f;
    [SerializeField] [Foldout("Audio")] private float walkFrequency = 30f;
    [SerializeField] [Foldout("Audio")] private AudioSource footStepAudio;
    private Oscillator oscillator = new Oscillator();

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        oscillator.currentFrequency = walkFrequency;
    }

    private void Update()
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
        var pos = GrowthGun.Instance.transform.localPosition;
        pos.y = -0.5f + oscillator.Eval() * viewBobAmount;
        GrowthGun.Instance.transform.localPosition =
            Vector3.Slerp(GrowthGun.Instance.transform.localPosition, pos, Time.deltaTime * 5f);
        var pos2 = cam.transform.localPosition;
        // pos2.y = Mathf.Clamp01(oscillator.Eval()) * 0.04f;
        // cam.transform.localPosition = pos2;
        transform.eulerAngles = new Vector3(0f, xMovement, 0f);

        Vector2 movement = inputManager.GetPlayerMovement();
        Vector3 move = transform.TransformDirection(new Vector3(movement.x, 0f, movement.y));

        controller.Move(move * Time.deltaTime * playerSpeed);
        if (groundedPlayer && WalkingSound != null)
        {
            if (Time.timeScale == 0)
            {
                oscillator.Wrapped = false;
                oscillator.currentPhase = 0f;
                return;
            }

            oscillator.currentFrequency = walkFrequency;
            if (move.magnitude > 0.5f)
            {
                oscillator.Advance(Time.deltaTime);
            }
            else
            {
                oscillator.currentPhase = 0f;
                oscillator.Wrapped = false;
            }

            if (oscillator.Wrapped)
            {
                footStepAudio.PlayOneShot(WalkingSound, walkingVolume);
            }
        }

        if (inputManager.PlayerJumpedThisFrame() && groundedPlayer)
        {
            if (JumpSound != null)
            {
                AudioSource.PlayClipAtPoint(JumpSound, transform.position, jumpVolume);
            }
            else if (WalkingSound != null)
            {
                footStepAudio.PlayOneShot(WalkingSound, walkingVolume);
                footStepAudio.PlayOneShot(WalkingSound, walkingVolume);
            }

            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.rigidbody != null)
        {
            InterractableObject obj = hit.rigidbody.GetComponent<InterractableObject>();

            if (obj)
            {
                if (!obj.CanBePushed) return;
            }

            hit.rigidbody.velocity = hit.moveDirection * pushForce;
        }
    }
}