using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // meter / second
    public float yawSpeed = 180f; // deg/sec
    public float pitchSpeed = 180f; // deg/sec
    public float xSensitivity = 1f;
    public float ySensitivity = 1f;
    public bool invertY = true;

    public GameObject camera;

    // Start is called before the first frame update
    void Start()
    {

    }
    [Header("debug stuff")]
    public float moveX = 0f;
    public float moveZ = 0f;
    
    public float rotYaw = 0f;
    public float rotPitch = 0f;
    public float camPitch = 0f;

    // Update is called once per frame
    void Update() {
        UpdateGetInput();
        if (invertY) {
            camPitch -= rotPitch * Time.deltaTime * pitchSpeed * ySensitivity;
        } else {
            camPitch += rotPitch * Time.deltaTime * pitchSpeed * ySensitivity;
        }
        camPitch = Mathf.Clamp(camPitch, -89, 89);
    }

    void UpdateGetInput() {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");

        rotYaw = Input.GetAxis("Mouse X");
        rotPitch = Input.GetAxis("Mouse Y");
    }

    void FixedUpdate() {
        transform.Translate(new Vector3(moveX * Time.fixedDeltaTime, 0f, moveZ * Time.fixedDeltaTime) * moveSpeed);
        transform.Rotate(0f, rotYaw * yawSpeed * Time.fixedDeltaTime * xSensitivity, 0f);
        camera.transform.localRotation = Quaternion.Euler(camPitch, 0, 0);
    }
}
