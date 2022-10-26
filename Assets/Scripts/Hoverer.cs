using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoverer : MonoBehaviour {

    public Rigidbody rb;
    public Vector3 groundVelocity { get; private set; } = Vector3.zero;
    public bool isJumping = false;

    [SerializeField]
    [Range(0, 2)]
    float rideHeight = 1f;
    readonly float rideSpringStrength = 150f;
    readonly float stickHeight = 0.5f;
    readonly float rideSpringDamper = 8f;

    readonly float uprightSpringStrenght = 20f;
    readonly float uprightSpringDamper = 0.1f;

    bool isOnGround = false;
    public bool IsOnGround { get => isOnGround; }

    public Rigidbody groundRigidbody { get; private set; } = null;

    void Start() {

    }

    void FixedUpdate() {
        Hover();
        UprightForce();
    }

    private void Hover() {
        Vector3 raycastDirection = Vector3.down;
        RaycastHit hit;
        bool raycastHit = Physics.Raycast(transform.position, raycastDirection, out hit, rideHeight + (isJumping ? 0 : stickHeight));
        isOnGround = raycastHit;
        groundRigidbody = hit.rigidbody;

        if(raycastHit) {
            Vector3 velocity = rb.velocity;

            Vector3 otherVelocity = Vector3.zero;
            Rigidbody hitBody = hit.rigidbody;
            if (hitBody != null) otherVelocity = hitBody.velocity;

            groundVelocity = otherVelocity;

            float raycastDirectionVelocity = Vector3.Dot(raycastDirection, velocity);
            float otherDirectionVelocity = Vector3.Dot(raycastDirection, otherVelocity);

            float relativeVelocity = raycastDirectionVelocity - otherDirectionVelocity;

            float desiredHeightDistance = hit.distance - rideHeight;

            float springForce = (desiredHeightDistance * rideSpringStrength) - (relativeVelocity * rideSpringDamper);

            /*
            Debug.DrawLine(transform.position, transform.position + raycastDirection * (rideHeight + stickHeight), Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
            */

            rb.AddForce(raycastDirection * springForce * rb.mass);

            if (hitBody != null) hitBody.AddForceAtPosition(-raycastDirection * springForce * rb.mass, hit.point);
        }
    }

    private void UprightForce() {
        Quaternion characterCurrent = transform.rotation;

        Vector3 up = transform.up;

        Vector3 characterTargetUp = Vector3.up;

        Quaternion toGoal = Quaternion.FromToRotation(up, characterTargetUp);

        Vector3 rotationAxis;
        float rotationDegrees;

        toGoal.ToAngleAxis(out rotationDegrees, out rotationAxis);
        rotationAxis.Normalize();

        float rotationRadians = rotationDegrees * Mathf.Deg2Rad;

        rb.AddTorque((rotationAxis * (rotationRadians * uprightSpringStrenght)) - (rb.angularVelocity * uprightSpringDamper));
    }
}
