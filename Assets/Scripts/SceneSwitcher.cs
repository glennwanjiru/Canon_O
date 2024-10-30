using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneSwitcher : MonoBehaviour
{
    // Public variable to assign the button from the Inspector
    public Button switchSceneButton;

    // The name of the scene to load
    public string sceneToLoad = "NextScene";

    void Start()
    {
        // Add a listener to the button to call the SwitchScene method when clicked
        if (switchSceneButton != null)
        {
            switchSceneButton.onClick.AddListener(SwitchScene);
        }
    }

    // Method to switch the scene
    void SwitchScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
