using UnityEngine;
using UnityEngine.SceneManagement;

public class WinSwitch : MonoBehaviour
{
    // Tag for the triggering object
    public string triggeringObjectTag = "Player";

    // Tag for the target object
    public string targetObjectTag = "Flag";

    // The name of the scene to load
    public string sceneToLoad = "WinScene";

    // OnTriggerEnter is called when the Collider other enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter called with: " + other.gameObject.name);

        // Check if the collision is with the target object
        if (other.CompareTag(targetObjectTag))
        {
            Debug.Log("Collided with target object: " + other.gameObject.name);

            // Check if the collider is the triggering object
            if (gameObject.CompareTag(triggeringObjectTag))
            {
                Debug.Log("Triggering object detected: " + gameObject.name);

                // Load the specified scene
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
