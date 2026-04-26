using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class InGameESCMenuHandler : MonoBehaviour {
    public UnityEvent onEscOn = new();
    public UnityEvent onEscOff = new();
    [Header("debug")]
    public bool on = false;
    
    [SerializeField] private InputActionReference vrControllerAction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        onEscOff.Invoke();
        vrControllerAction.action.performed += vrButtonPressed;
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

    private void vrButtonPressed(InputAction.CallbackContext ctx) {
        on = !on;
        if (on) {
            onEscOn.Invoke();
        } else {
            onEscOff.Invoke();
        }
    }
}
