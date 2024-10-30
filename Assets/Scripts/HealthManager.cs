using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;  // Add this for Unity Ads

public class HealthManager : MonoBehaviour, IUnityAdsListener
{
    public int collisionThreshold = 3;
    private int collisionCount = 0;
    public Slider healthSlider;
    public string gameOverSceneName = "GameOverScene";


    // Unity Ads
    private string gameId = "YOUR_GAME_ID";  // Replace with your actual Game ID
    private string adUnitId = "Interstitial_Android"; // Replace with your actual Ad Unit ID
    private bool adLoaded = false;

    void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = collisionThreshold;
            healthSlider.value = collisionThreshold;
        }

        // Initialize Unity Ads
        Advertisement.Initialize(gameId, true);
        Advertisement.AddListener(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullets"))
        {
            collisionCount++;

            if (healthSlider != null)
            {
                healthSlider.value = collisionThreshold - collisionCount;
            }

            if (collisionCount >= collisionThreshold)
            {
                if (gameObject.CompareTag("Player"))
                {
                    // Show an ad before loading the game over scene
                    ShowAd();
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    void ShowAd()
    {
        if (Advertisement.IsReady(adUnitId))
        {
            Advertisement.Show(adUnitId);
        }
        else
        {
            // If the ad is not ready, proceed to game over
            LoadGameOverScene();
        }
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        if (placementId == adUnitId)
        {
            // After the ad finishes, load the game over scene
            LoadGameOverScene();
        }
    }

    public void LoadGameOverScene()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }

    public void OnUnityAdsReady(string placementId) { adLoaded = true; }
    public void OnUnityAdsDidError(string message) { /* Log any errors here */ }
    public void OnUnityAdsDidStart(string placementId) { }
}
