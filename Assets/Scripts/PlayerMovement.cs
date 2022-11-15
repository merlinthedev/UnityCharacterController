using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour {

    public Rigidbody rb;
    public Transform orientation;
    public Grappling grappling;

    float horizontalInput;
    float verticalInput;

    Vector3 movementDirection;


    [Header("Movement")]
    public float movementSpeed;
    public float playerHeight;
    public float groundDrag;
    public LayerMask groundCheck;
    public bool isGrounded;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public KeyCode jumpKey = KeyCode.Space;

    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    private bool isCrouching = false;
    public KeyCode crouchKey = KeyCode.LeftControl;

    public bool isFrozen = false;
    public bool isGrappling = false;

    private Vector3 goalVelocity;
    private bool enableMovement;


    private void Start() {
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    private void Update() {
        if (isFrozen) {
            rb.velocity = Vector3.zero;
        }

        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f, groundCheck);
        keyboardInput();
        speedControl();

        if (isGrounded && !isGrappling) {
            rb.drag = groundDrag;
        } else {
            rb.drag = 0;
        }
    }

    private void FixedUpdate() {
        playerMovement();
    }

    private void keyboardInput() {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && isGrounded) {
            readyToJump = false;

            jump();
            Invoke(nameof(resetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey)) {
            isCrouching = true;
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey)) {
            isCrouching = false;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void playerMovement() {
        if (isGrappling) return;

        movementDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (isGrounded) {
            rb.AddForce(movementDirection.normalized * (isCrouching ? crouchSpeed : movementSpeed) * 10f, ForceMode.Force);
        } else if (!isGrounded) {
            rb.AddForce(movementDirection.normalized * movementSpeed * 10f * airMultiplier, ForceMode.Force);
        }


    }

    private void speedControl() {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (flatVelocity.magnitude > movementSpeed) {
            Vector3 limitedVelocity = flatVelocity.normalized * movementSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }

    private void jump() {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void resetJump() {
        readyToJump = true;
    }


    private void setVelocity() {
        enableMovement = true;
        rb.velocity = goalVelocity;
    }

    private void OnCollisionEnter(Collision collision) {
        if (enableMovement) {
            enableMovement = false;
            isGrappling = false;

            grappling.stopGrapple();
        }
    }

    public void jumpToPosition(Vector3 targetPosition, float trajectoryHeight) {
        isGrappling = true;
        
        goalVelocity = calculateJumpForce(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(setVelocity), 0.1f);
    }

    public Vector3 calculateJumpForce(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight) {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
}
