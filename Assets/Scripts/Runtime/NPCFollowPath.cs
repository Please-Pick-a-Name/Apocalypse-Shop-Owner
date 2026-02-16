using UnityEngine;

public class SimpleFollowPath : MonoBehaviour {
    public Transform[] waypoints;
    public float moveSpeed = 1f;
    public float stoppingDistance;
    public float rotationSpeed = 5f;
    public int moveDir = 1;
    public bool talking;
    public LayerMask npcLayer;
    private float currentSpeed;
    private int currentWaypointIndex = 0;
    public DialogueTree dialogueTree;

    void Update() {
        if (waypoints.Length == 0) return;

        if (IsPathBlocked()) {
            currentSpeed = 0;
        } else {
            currentSpeed = moveSpeed;
        }

        Vector3 targetPosition = waypoints[currentWaypointIndex].position;
        Vector3 direction = targetPosition - transform.position;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (talking) {
            return;
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) {
            currentWaypointIndex = (currentWaypointIndex + moveDir) % waypoints.Length;
            if (moveDir == 1 & currentWaypointIndex + 1 == waypoints.Length) {
                DialogueManager.instance.StartDialogue(dialogueTree.nodes[0]);
                DialogueManager.instance.relatedNPC.Add(this);
                talking = true;
            }
            if (moveDir == -1 & currentWaypointIndex == 0) {
                Destroy(gameObject);
            }
        }
    }

    bool IsPathBlocked() {
        Ray ray = new(transform.position + Vector3.up * 0.5f, transform.forward);
        return Physics.Raycast(ray, out RaycastHit hit, stoppingDistance, npcLayer);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * stoppingDistance);
    }
}