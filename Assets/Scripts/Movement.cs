using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour {

    [SerializeField] private Rigidbody _rigidBody;

    [SerializeField] private PlayerInput playerInput;
    private InputAction movementAction;


    [SerializeField]
    private float playerMovementSmoothTime = 0.1f;
    [SerializeField]
    private float playerMovementSpeed = 8f;

    private Vector2 inputVelocity;
    private Vector2 currentInputVelocity;

    private Vector2 refInputVelocity;

    private void Start() {
        movementAction = playerInput.actions["Movement"];
    }

    private void Update() {
        // Get our vector2 from the InputSystem.
        inputVelocity = movementAction.ReadValue<Vector2>();

        // SmoothDamp our vector2 to give the movement a more smooth feeling instead of instant movement. Convert vector2 to vector3.
        currentInputVelocity = Vector2.SmoothDamp(currentInputVelocity, inputVelocity, ref refInputVelocity, playerMovementSmoothTime);
        Vector3 finalVelocity = new Vector3(currentInputVelocity.x, 0, currentInputVelocity.y);

        // Move in the direction of our rotation.
        _rigidBody.MovePosition(_rigidBody.position + transform.forward * finalVelocity.z * playerMovementSpeed * Time.deltaTime);

    }




}
