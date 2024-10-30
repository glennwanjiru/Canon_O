using UnityEngine;

public class WheelAnimation : MonoBehaviour
{
    public GameObject leftWheel;
    public GameObject rightWheel;
    public float rotationSpeed = 100f; // Adjust as needed
    public Vector3 rotationDirection = Vector3.right; // Direction of wheel rotation (forward should be along the x-axis)
    public AudioClip movingSound; // The audio clip to play when moving

    private Vector3 previousPosition;
    private AudioSource audioSource;

    void Start()
    {
        previousPosition = transform.position;
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = movingSound;
        audioSource.loop = true; // Set to loop the audio clip
        audioSource.pitch = 1.73f; // Set the pitch
        audioSource.volume = 0.45f; // Set the volume
    }

    void Update()
    {
        // Calculate the change in position
        Vector3 positionDelta = transform.position - previousPosition;

        // Calculate the speed based on the position change
        float speed = positionDelta.magnitude / Time.deltaTime;

        // Play or stop the audio based on the speed
        if (speed > 0.1f) // Adjust the threshold as needed
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        RotateWheels(speed);

        // Update the previous position
        previousPosition = transform.position;
    }

    void RotateWheels(float speed)
    {
        float rotationAmount = speed * rotationSpeed * Time.deltaTime;

        // Rotate both wheels equally
        leftWheel.transform.Rotate(rotationDirection, rotationAmount);
        rightWheel.transform.Rotate(rotationDirection, rotationAmount);
    }
}
