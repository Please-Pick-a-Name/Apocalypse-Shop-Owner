using UnityEngine;

public class SimpleFollowPath : MonoBehaviour {
    public Transform[] waypoints;
    [Tooltip("2.5 seems to match animation walk speed perfectly but feels too fast")]
    public float moveSpeed = 1f;
    public float stoppingDistance;
    public float rotationSpeed = 5f;
    public int moveDir = 1;
    public bool talking;
    public LayerMask npcLayer;
    private float currentSpeed;
    private int currentWaypointIndex = 0;
    public DialogueTree dialogueTree;
    public Animator anim;
    public AudioClip enterStoreSound;
    private Transform enterWaypoint;
    private bool hasPlayedStoreSound = false;
    void Start() {
        GameObject foundObject = GameObject.Find("EnterWaypoint");
    if (foundObject != null) {
        enterWaypoint = foundObject.transform; 
    }
    }
    void Update() {
        if (waypoints.Length == 0) {
            anim.SetBool("isWalking", false);
            return;
        }
        
        if (IsPathBlocked()) {
            currentSpeed = 0;
        } else {
            currentSpeed = moveSpeed;
            anim.SetBool("isWalking", true);
        }
        
        Vector3 targetPosition = waypoints[currentWaypointIndex].position;
        Vector3 direction = targetPosition - transform.position;

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
        
        if (direction != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        if (talking) {
            anim.SetBool("isWalking", false);
            return;
        }
        if (enterWaypoint != null && !hasPlayedStoreSound) {
            if (Vector3.Distance(transform.position, enterWaypoint.position) < 0.5f) {
                SoundFXManager.instance.PlaySoundFXClip(enterStoreSound, transform, 0.3f, 0f);
                hasPlayedStoreSound = true; 
            }
        }

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) {
            currentWaypointIndex = (currentWaypointIndex + moveDir) % waypoints.Length;
            if (moveDir == 1 && currentWaypointIndex + 1 == waypoints.Length) {
                DialogueManager.instance.StartDialogue(dialogueTree.nodes[0]);
                DialogueManager.instance.relatedNPC.Add(this);
                talking = true;
                anim.SetTrigger("Talk");
                anim.SetBool("isWalking", false);
            }
            if (moveDir == -1 && currentWaypointIndex == 0) {
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