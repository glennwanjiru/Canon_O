using UnityEngine;

public class PatrolChaseStateMachine : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Chase
    }

    public Transform pointA;
    public Transform pointB;
    public float patrolSpeed = 5f;
    public float chaseSpeed = 7f;
    public float detectRadius = 5f;

    // Projectile settings
    public GameObject projectilePrefab;
    public Transform shootingPoint; // Added shooting point transform
    public float shootInterval = 1f;
    public float projectileSpeed = 10f;

    // Audio settings
    public AudioClip shootingAudioClip; // Added shooting audio clip
    private AudioSource audioSource;

    private State currentState;
    private Transform target;
    private Vector3 currentDestination;
    private SphereCollider triggerCollider;
    private float lastShootTime;

    private void Start()
    {
        currentState = State.Patrol;
        currentDestination = new Vector3(pointA.position.x, transform.position.y, pointA.position.z);

        // Set up the trigger collider
        triggerCollider = gameObject.AddComponent<SphereCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = detectRadius;

        // Set up the audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = shootingAudioClip;
    }

    private void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Chase:
                Chase();
                break;
        }
    }

    private void Patrol()
    {
        Vector3 nextPosition = Vector3.MoveTowards(
            new Vector3(transform.position.x, transform.position.y, transform.position.z),
            new Vector3(currentDestination.x, transform.position.y, currentDestination.z),
            patrolSpeed * Time.deltaTime
        );

        transform.position = nextPosition;

        if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z),
                             new Vector2(currentDestination.x, currentDestination.z)) < 0.1f)
        {
            if (currentDestination.x == pointA.position.x && currentDestination.z == pointA.position.z)
                currentDestination = new Vector3(pointB.position.x, transform.position.y, pointB.position.z);
            else
                currentDestination = new Vector3(pointA.position.x, transform.position.y, pointA.position.z);
        }
    }

    private void Chase()
    {
        if (target != null)
        {
            Vector3 nextPosition = Vector3.MoveTowards(
                new Vector3(transform.position.x, transform.position.y, transform.position.z),
                new Vector3(target.position.x, transform.position.y, target.position.z),
                chaseSpeed * Time.deltaTime
            );

            transform.position = nextPosition;

            // Face the target
            Vector3 directionToTarget = target.position - transform.position;
            directionToTarget.y = 0; // Keep the y-rotation level
            transform.rotation = Quaternion.LookRotation(directionToTarget);

            // Shoot at the target
            if (Time.time - lastShootTime > shootInterval)
            {
                ShootProjectile();
                lastShootTime = Time.time;
            }

            if (Vector3.Distance(transform.position, target.position) > detectRadius)
            {
                target = null;
                currentState = State.Patrol;
            }
        }
        else
        {
            currentState = State.Patrol;
        }
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null && shootingPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position, Quaternion.identity);
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

            if (projectileRb != null)
            {
                Vector3 shootDirection = (target.position - transform.position).normalized;
                projectileRb.velocity = shootDirection * projectileSpeed;
            }

            // Play shooting audio
            if (audioSource != null && shootingAudioClip != null)
            {
                audioSource.PlayOneShot(shootingAudioClip);
            }

            // Destroy the projectile after 5 seconds to prevent clutter
            Destroy(projectile, 5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentState == State.Patrol && other.gameObject.CompareTag("Player"))
        {
            target = other.transform;
            currentState = State.Chase;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);

        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
