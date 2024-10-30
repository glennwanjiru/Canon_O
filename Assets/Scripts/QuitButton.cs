using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    // Public variable to assign the button from the Inspector
    public Button quitButton;

    void Start()
    {
        // Add a listener to the button to call the QuitGame method when clicked
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    // Method to quit the game
    void QuitGame()
    {
        // If we are running in the editor
#if UNITY_EDITOR
        // Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // If we are running in a standalone build, quit the application
        Application.Quit();
#endif
    }
}
