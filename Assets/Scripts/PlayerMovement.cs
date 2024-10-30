using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 10f;
    public float projectileLifeTime = 2f;

    // Audio settings
    public AudioClip shootingAudioClip;
    public AudioClip movingAudioClip;
    public AudioClip jumpingAudioClip;
    private AudioSource audioSource;

    private Rigidbody rb;
    private bool isGrounded = false;
    private int jumpCount = 0;
    private int maxJumps = 2;

    // Joystick and Buttons
    public FixedJoystick fixedJoystick;
    public Button shootButton;
    public Button jumpButton;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = gameObject.AddComponent<AudioSource>();

        // Set up button listeners
        shootButton.onClick.AddListener(ShootProjectile);
        jumpButton.onClick.AddListener(Jump);
    }

    void Update()
    {
        // Handle Movement using FixedJoystick
        Vector3 moveDirection = new Vector3(fixedJoystick.Horizontal, 0, fixedJoystick.Vertical).normalized;

        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);

            // Play moving audio clip if not already playing
            if (!audioSource.isPlaying || audioSource.clip != movingAudioClip)
            {
                audioSource.clip = movingAudioClip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            // Stop moving audio clip
            if (audioSource.clip == movingAudioClip)
            {
                audioSource.Stop();
            }
        }

        rb.velocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void ShootProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
        projectileRb.velocity = projectileSpawnPoint.forward * projectileSpeed;
        Destroy(projectile, projectileLifeTime);

        // Play shooting audio clip
        audioSource.PlayOneShot(shootingAudioClip);
    }

    void Jump()
    {
        // Handle Jump
        if (isGrounded || jumpCount < maxJumps)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            jumpCount++;

            // Play jumping audio clip
            audioSource.PlayOneShot(jumpingAudioClip);
        }
    }
}
