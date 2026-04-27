using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(SpriteRenderer))]
public class Projectile : MonoBehaviour {
    public ProjectileOptions projectileOptions;
    public ProjectileVisualOptions visualOptions;
    public Vector3 lastPos;
    public Vector3 vel;
    public LayerMask projectileHitMask;
    
    [Header("debug")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private MeshRenderer meshRenderer;
    public bool physicsEnabled = false;

    public void Init(Vector3 origin, Vector3 velocity, ProjectileOptions newProjectileOptions, ProjectileVisualOptions newVisualOptions) {
        lastPos = origin;
        transform.position = origin;
        vel = velocity;
        projectileOptions = newProjectileOptions;
        visualOptions = newVisualOptions;

        enabled = true;
        physicsEnabled = true;

        spriteRenderer.sprite = visualOptions.sprite;
        spriteRenderer.enabled = visualOptions.visualType.HasFlag(ProjectileVisualType.SPRITE);
        
        lineRenderer.colorGradient = visualOptions.lineGradient;
        lineRenderer.enabled = visualOptions.visualType.HasFlag(ProjectileVisualType.LINE);
        
        meshFilter.mesh = visualOptions.mesh;
        meshRenderer.materials = visualOptions.meshMaterials;
        meshRenderer.enabled = visualOptions.visualType.HasFlag(ProjectileVisualType.MESH);
    }
    public void Awake() {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        meshFilter = GetComponentInChildren<MeshFilter>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        enabled = false;
    }

    public void FixedUpdate() {
        if (!physicsEnabled) {
            return;
        }

        if (transform.position.y <= -10) {
            Reset();
            return;
        }

        var dt = Time.fixedDeltaTime;
        ProjectileFixedUpdate(dt);

    }

    public void ProjectileFixedUpdate(float dt) {
        // why update vel first? because i remembered i saw a vid about it, https://youtu.be/nCg3aXn5F3M?si=dS-NVbnGRbkDG-uz&t=1230
        vel += Physics.gravity * dt;

        lastPos = transform.position;
        var maxDistance = vel.magnitude * dt;
        if (Physics.Raycast(transform.position, vel, out RaycastHit hitInfo, maxDistance, projectileHitMask)) {
            transform.position = hitInfo.point;
            OnProjectileHit(hitInfo.collider, transform.position);
            Reset();
            return;
        }

        transform.SetPositionAndRotation(transform.position + vel * dt, Quaternion.LookRotation(vel));

    }

    public void Update() {
        if (visualOptions.visualType.HasFlag(ProjectileVisualType.SPRITE)) {

        }
        if (visualOptions.visualType.HasFlag(ProjectileVisualType.LINE)) {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, lastPos);
        }
        if (visualOptions.visualType.HasFlag(ProjectileVisualType.MESH)) {

        }
    }

    [SerializeField] private Collider[] hitColliders = new Collider[64];
    public void OnProjectileHit(Collider hitTarget, Vector3 hitPosition) {
        physicsEnabled = false;
        if (hitTarget != null) {
            var zombieHealth = hitTarget.GetComponentInParent<ZombieHealth>();
            if (zombieHealth) {
                if (projectileOptions.ignoreArmour) {
                    zombieHealth.OnDamage(projectileOptions.damage);
                } else {
                    zombieHealth.OnHit(hitTarget, projectileOptions.damage);
                }
            }
        }

        if (projectileOptions.projectileType.HasFlag(ProjectileType.EXPLOSIVE)) {
            var hitColliderCount = Physics.OverlapSphereNonAlloc(hitPosition, projectileOptions.radius, hitColliders);
            for (int i = 0; i < hitColliderCount; i++) {
                var hitCollider = hitColliders[i];
                var zombieHealthAOE = hitCollider.GetComponentInParent<ZombieHealth>();
                if (zombieHealthAOE) {
                    zombieHealthAOE.OnHit(hitCollider, projectileOptions.damage);
                }
            }
        }
    }


    private void Reset() {
        lineRenderer.enabled = false;
        spriteRenderer.enabled = false;
        meshRenderer.enabled = false;
        enabled = false;
    }
}
