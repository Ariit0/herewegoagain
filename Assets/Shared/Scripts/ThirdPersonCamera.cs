using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {
    [SerializeField] private bool debugLines = false;

    [SerializeField] private bool hideCursor = true;

    [SerializeField] private float mouseSensitivity = 2.0f;

    [SerializeField] private Transform target;

    [SerializeField] private float distanceFromTarget = 3.5f;

    [SerializeField] private float minDistanceFromTarget = 0.85f;

    [SerializeField] private float adjustedDistance = 0;

    [SerializeField] private Vector2 yMinMax = new Vector2(-40, 80);

    [SerializeField] private float rotationSmoothTime = 0.12f;
    private Vector3 rotationSmoothVelocity;

    private Vector3 currentRotation;

    private float xAxis;
    private float yAxis;

    CameraCollisionHandler collision;

    private void Start() {

        if (hideCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        collision = gameObject.AddComponent<CameraCollisionHandler>();

        collision.Init(Camera.main);
        collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.desiredCameraClipPoints);
    }

    private void FixedUpdate() {
        collision.UpdateCameraClipPoints(transform.position, transform.rotation, ref collision.desiredCameraClipPoints);

        // draw debug lines
        if (debugLines) {
            for (int i = 0; i < 5; i++) {
                Debug.DrawLine(target.position, collision.desiredCameraClipPoints[i], Color.green);
            }
        }

        adjustedDistance = collision.GetAdjustedDistanceRayFrom(target.position);
    }

    private void LateUpdate() {
        xAxis += Input.GetAxis("Mouse X") * mouseSensitivity;
        yAxis -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        yAxis = Mathf.Clamp(yAxis, yMinMax.x, yMinMax.y);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(yAxis, xAxis), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;

        // if adjustDistance is > 0 then there is a camera collision
        if (adjustedDistance > 0) {
            // todo: maybe add smoothing
            transform.position = target.position - transform.forward * Mathf.Clamp(adjustedDistance, minDistanceFromTarget, distanceFromTarget);
        } else {
            transform.position = target.position - transform.forward * distanceFromTarget;
        }
    }
}