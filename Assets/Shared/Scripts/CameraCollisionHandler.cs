using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollisionHandler: MonoBehaviour {

    // all the objects that the camera can collide with
    public LayerMask collisionLayer;
    [HideInInspector] public Vector3[] desiredCameraClipPoints;
    [SerializeField] private float rayMaxDistance = 3.5f;
    private Camera m_camera;
    private float cameraPadding = 3.41f;

    public void Init(Camera cam) {
        m_camera = cam;
        desiredCameraClipPoints = new Vector3[5];
        collisionLayer = LayerMask.GetMask(LayerMask.LayerToName(0));
    }

    public void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray) {
        if (!m_camera) {
            return;
        }

        // as the camera moves we are getting new clip point data, therefore we need to clear the previous instance to input new data
        intoArray = new Vector3[5];

        float z = m_camera.nearClipPlane;
        float x = Mathf.Tan(m_camera.fieldOfView / cameraPadding) * z; // determines how much of a padding between the camera and what will it collide
        float y = x / m_camera.aspect;

        // find clip points for each one of clip points on the near clip plane
        // top left
        intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition; // add / rotate the point relative to the camera
        // top right
        intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition;
        // bot left
        intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition;
        // bot right
        intoArray[3] = (atRotation * new Vector3(x, -y, z)) + cameraPosition;
        // camera position
        intoArray[4] = cameraPosition - m_camera.transform.forward; // provides more room for the camera to collide with
    }

    /// <summary>
    /// Determines how far we move out camera forward if there is a collision
    /// </summary>
    /// <param name="from"></param>
    /// <returns></returns>
    public float GetAdjustedDistanceRayFrom(Vector3 from) {
        float distance = -1;

        for (int i = 0; i < desiredCameraClipPoints.Length; i++) {
            Ray ray = new Ray(from, desiredCameraClipPoints[i] - from);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, rayMaxDistance, collisionLayer)) { 
                if (distance == -1) {
                    distance = hit.distance;
                } else {
                    if (hit.distance < distance) {
                        distance = hit.distance;
                    } 

                }
            }
        }

        if (distance == -1) {
            return 0;
        } else {
            return distance;
        }
    }
}
