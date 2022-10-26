using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float mouseSensitivity = 5f;
    [SerializeField] private float cameraClampAngle = 80f;

    private InputAction mouseAction;
    private float xRotation = 0f;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;

        mouseAction = playerInput.actions["Mouse"];
    }

    private void Update() {
        Vector2 getMouseInput = mouseAction.ReadValue<Vector2>();

        float mouseX = getMouseInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = getMouseInput.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -cameraClampAngle, cameraClampAngle);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

}
