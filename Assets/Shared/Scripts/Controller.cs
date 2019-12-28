using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float gravity = -12.0f;
    [SerializeField] private float movementSpeed = 2.5f;

    [SerializeField] private float slopeForce = 5.0f;
    [SerializeField] private float slopeForceRayLength = 1.5f;

    [SerializeField] private float turnSmoothTime = 0.02f;
    private float turnSmoothVelocity;

    [SerializeField] private float speedSmoothTime = 0.05f;
    private float speedSmoothVelocity;

    private float currentSpeed;
    private float velocityY;
    private Animator animator;
    private CharacterController controller;
    // Start is called before the first frame update
    private void Start () {
        animator = GetComponent<Animator> ();
        cameraTransform = Camera.main.transform;
        controller = GetComponent<CharacterController> ();
    }

    // Update is called once per frame
    void Update () {
        Vector2 input = new Vector2 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"));
        // convert input vector into direction to identify character rotation
        Vector2 inputDirection = input.normalized;

        Move (inputDirection);

        // animation
        float animationSpeedPercentage = currentSpeed / movementSpeed;
        animator.SetFloat ("speedPercent", animationSpeedPercentage, speedSmoothTime, Time.deltaTime);
    }

    private void Move (Vector2 inputDirection) {
        // In Unity, when a character is facing forwards it has a rotation of 0 degrees,
        // Using trig we can find the angle the character is facing from its original direction
        if (inputDirection != Vector2.zero) {
            float targetRotation = Mathf.Atan2 (inputDirection.x, inputDirection.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle (transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, turnSmoothTime);
        }

        float targetSpeed = movementSpeed * inputDirection.magnitude;
        currentSpeed = Mathf.SmoothDamp (currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        // apply downwards force on character given they drop from an elevated high
        velocityY += Time.deltaTime * gravity;

        Vector3 velocity = (transform.forward * currentSpeed + Vector3.up * velocityY);

        controller.Move (velocity * Time.deltaTime);
        // update currentSpeed to its actual speed after the controller has moved
        currentSpeed = new Vector2 (controller.velocity.x, controller.velocity.z).magnitude;

        // check if moving down a slope and apply downwards slopeforce (removes bouncing when moving down slopes)
        if ((inputDirection.x != 0 || inputDirection.y != 0) && OnSlope()) {
            controller.Move(Vector3.down * controller.height / 2 * slopeForce * Time.deltaTime);
        }

        // reset velocityY when character is standing
        if (controller.isGrounded) {
            velocityY = 0;
        }
    }

    private bool OnSlope () {
        RaycastHit hit;

        // Debug.DrawRay(transform.position, Vector3.down, Color.red);
        // send a raycast downwards from the character and if the normals do not equal 0,1,0 then we're on a slope
        if (Physics.Raycast (transform.position, Vector3.down, out hit, controller.height / 2 * slopeForceRayLength)) {
            if (hit.normal != Vector3.up) {
                return true;
            }
        }

        return false;
    }

}