using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Transform cam;
    public Transform gunTip;
    public LineRenderer lineRenderer;
    public LayerMask grappableObjects;

    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshoot;

    private Vector3 pointOfImpact;

    public KeyCode grappleKey = KeyCode.Mouse1;

    private bool isGrappling = false;


    private void Update() {
        if (Input.GetKeyDown(grappleKey)) {
            startGrapple();
        }
    }
    private void LateUpdate() {
        if (isGrappling) {
            lineRenderer.SetPosition(0, gunTip.position);
            lineRenderer.SetPosition(1, pointOfImpact);
        }
    }

    private void startGrapple() {
        isGrappling = true;
        playerMovement.isFrozen = true;

        Debug.Log("Started Grapple");

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, grappableObjects)) {

            Debug.Log("Hit " + hit.collider.name);

            pointOfImpact = hit.point;
            Invoke(nameof(executeGrapple), grappleDelayTime);

        } else {
            pointOfImpact = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(stopGrapple), grappleDelayTime);
        }

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(1, gunTip.position);
    }

    private void executeGrapple() {
        playerMovement.isFrozen = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.y);
        float grapplePoint = pointOfImpact.y - lowestPoint.y;
        float highestPoint = grapplePoint + overshoot;

        if (grapplePoint < 0) {
            highestPoint = overshoot;
        }

        playerMovement.jumpToPosition(pointOfImpact, highestPoint);

        Invoke(nameof(stopGrapple), 1f);

    }

    public void stopGrapple() {
        playerMovement.isFrozen = false;
        isGrappling = false;
        lineRenderer.enabled = false;
    }
}
