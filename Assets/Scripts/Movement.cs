using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour {

    
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Hoverer hoverer;
    [SerializeField] private PlayerInput playerInput;
    private InputAction movementAction;


    [Header("Movement")]
    [SerializeField] private float playerMovementSmoothTime = 0.1f;
    [SerializeField] private float playerMovementSpeed = 8f;


    [Header("Jumping")]
    [SerializeField] private float jumpForce = 24f;

    [Header("Crouching")]
    [SerializeField] private bool isCrouching = false;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float crouchSpeed = 4f;
    




    private Vector2 inputVelocity;
    private Vector2 currentInputVelocity;

    private Vector2 refInputVelocity;

    private void Start() {
        movementAction = playerInput.actions["Movement"];
    }

    private void Update() {
        crouching();
        movement();
        jumping();
    }

    private void movement() {
        // Get our vector2 from the InputSystem.
        inputVelocity = movementAction.ReadValue<Vector2>();

        // SmoothDamp our vector2 to give the movement a more smooth feeling instead of instant movement. Convert vector2 to vector3.
        currentInputVelocity = Vector2.SmoothDamp(currentInputVelocity, inputVelocity, ref refInputVelocity, playerMovementSmoothTime);
        Vector3 finalVelocity = new Vector3(currentInputVelocity.x, 0, currentInputVelocity.y);

        // Move in the direction of our rotation.
        _rigidBody.MovePosition(_rigidBody.position + transform.TransformDirection(finalVelocity) * (isCrouching ? crouchSpeed : playerMovementSpeed) * Time.deltaTime);


    }

    private void jumping() {
        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && hoverer.IsOnGround) {
            _rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // Crouching
    private void crouching() {
        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
            isCrouching = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl)) {
            transform.localScale = new Vector3(transform.localScale.x, 1, transform.localScale.z);
            isCrouching = false;
        }
    }



}
