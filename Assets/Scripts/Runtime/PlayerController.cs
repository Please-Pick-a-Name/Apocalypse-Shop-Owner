using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public static PlayerController instance;
    public float moveSpeed = 5f; // meter / second
    public float yawSpeed = 180f; // deg/sec
    public float pitchSpeed = 180f; // deg/sec
    public float xSensitivity = 1f;
    public float ySensitivity = 1f;
    public bool invertY = true;

    public GameObject camera;

    Rigidbody rb;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
            return;
        }
    }
    [Header("debug stuff")]
    public float moveX = 0f;
    public float moveZ = 0f;

    public float rotYaw = 0f;
    public float rotPitch = 0f;
    public float camPitch = 0f;

    public static bool cursorMode = false;

    void Awake() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update() {
        UpdateGetInput();
        if (invertY) {
            camPitch -= rotPitch * Time.deltaTime * pitchSpeed * ySensitivity;
        } else {
            camPitch += rotPitch * Time.deltaTime * pitchSpeed * ySensitivity;
        }
        camPitch = Mathf.Clamp(camPitch, -89, 89);
        //Debug.Log("Cursor Mode: " + cursorMode);
    }

    void UpdateGetInput() {
        if (!cursorMode) {
            moveX = Input.GetAxis("Horizontal");
            moveZ = Input.GetAxis("Vertical");

            rotYaw = Input.GetAxis("Mouse X");
            rotPitch = Input.GetAxis("Mouse Y");
        } else {
            moveX = 0;
            moveZ = 0;
            rotYaw = 0;
            rotPitch = 0;
        }


        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (cursorMode) {
                DisableCursor();
            } else {
                EnableCursor();
            }
        }
    }

    void FixedUpdate() {
        /*transform.Translate(new Vector3(moveX * Time.fixedDeltaTime, 0f, moveZ * Time.fixedDeltaTime) * moveSpeed);
        transform.Rotate(0f, rotYaw * yawSpeed * Time.fixedDeltaTime * xSensitivity, 0f);
        camera.transform.localRotation = Quaternion.Euler(camPitch, 0, 0);*/

        Vector3 input = new Vector3(moveX, 0f, moveZ);

        Vector3 move = transform.TransformDirection(input) *
                       moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + move);

        rb.MoveRotation(
            rb.rotation *
            Quaternion.Euler(0f, rotYaw * yawSpeed * Time.fixedDeltaTime * xSensitivity, 0f)
        );

        camera.transform.localRotation = Quaternion.Euler(camPitch, 0, 0);
    }

    public void EnableCursor() {
        cursorMode = true;
        Cursor.lockState = CursorLockMode.None;
        //Cursor.visible = true;
    }

    public void DisableCursor() {
        cursorMode = false;
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
}
