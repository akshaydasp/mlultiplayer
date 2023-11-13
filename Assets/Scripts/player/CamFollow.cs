using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    [SerializeField]
    public float distance = 7.0f;
    [SerializeField]
    public float height = 3.0f;
    [SerializeField]
    public Vector3 centerOffset = Vector3.zero;
    [SerializeField]
    private bool followOnStart = false;
    [SerializeField]
    public float smoothSpeed = 0.125f;

    Transform cameraTransform;
    bool isFollowing;
    Vector3 cameraOffset = Vector3.zero;

    // Add a variable to control the mouse sensitivity.
    public float mouseSensitivity = 2.0f;

    private float rotationX = 0;

    void Start()
    {
        if (followOnStart)
        {
            OnStartFollowing();
        }
    }

    void LateUpdate()
    {
        if (cameraTransform == null && isFollowing)
        {
            OnStartFollowing();
        }

        if (isFollowing)
        {
            Follow();
        }
    }

    public void OnStartFollowing()
    {
        cameraTransform = Camera.main.transform;
        isFollowing = true;
        Cut();
    }

    void Follow()
    {
        cameraOffset.z = -distance;
        cameraOffset.y = height;

        // Handle mouse input for camera rotation.
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationX = Mathf.Clamp(rotationX, -90, 90);

        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        this.transform.rotation *= Quaternion.Euler(0, mouseX, 0);

        cameraTransform.position = Vector3.Lerp(cameraTransform.position, this.transform.position + this.transform.TransformVector(cameraOffset), smoothSpeed * Time.deltaTime);

        cameraTransform.LookAt(this.transform.position + centerOffset);
    }

    void Cut()
    {
        cameraOffset.z = -distance;
        cameraOffset.y = height;

        cameraTransform.position = this.transform.position + this.transform.TransformVector(cameraOffset);

        cameraTransform.LookAt(this.transform.position + centerOffset);
    }
}
