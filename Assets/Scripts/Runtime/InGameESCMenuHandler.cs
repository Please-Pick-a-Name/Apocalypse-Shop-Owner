using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class InGameESCMenuHandler : MonoBehaviour {
    public UnityEvent onEscOn = new();
    public UnityEvent onEscOff = new();
    [Header("debug")]
    public bool on = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        onEscOff.Invoke();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            on = !on;
            if (on) {
                onEscOn.Invoke();
                PlayerController.instance.EnableCursor();
            } else {
                onEscOff.Invoke();
                PlayerController.instance.DisableCursor();
            }
        }
    }
}
