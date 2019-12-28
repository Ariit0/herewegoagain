using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {
    [SerializeField]
    private bool hideCursor = true;

    [SerializeField]
    private float mouseSensitivity = 2.0f;

    [SerializeField]
    private Transform target;

    [SerializeField]
    private float distanceFromTarget = 2;

    [SerializeField]
    private Vector2 yMinMax = new Vector2 (-40, 80);

    [SerializeField]
    private float rotationSmoothTime = 0.12f;
    private Vector3 rotationSmoothVelocity;
    private Vector3 currentRotation;

    private float xAxis;
    private float yAxis;

    private void Start() {
        if (hideCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void LateUpdate () {
        xAxis += Input.GetAxis ("Mouse X") * mouseSensitivity;
        yAxis -= Input.GetAxis ("Mouse Y") * mouseSensitivity;
        yAxis = Mathf.Clamp (yAxis, yMinMax.x, yMinMax.y);

        currentRotation = Vector3.SmoothDamp (currentRotation, new Vector3 (yAxis, xAxis), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;

        transform.position = target.position - transform.forward * distanceFromTarget;
    }
}