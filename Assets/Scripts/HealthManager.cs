using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class HealthManager : MonoBehaviour
{
    public int collisionThreshold = 3;
    private int collisionCount = 0;
    public Slider healthSlider;
    public string gameOverSceneName = "GameOverScene";

    private string gameId = "5722429";  // Replace with your actual Game ID
    private string adUnitId = "Interstitial_Android";  // Replace with your Ad Unit ID
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

        // Load an ad in advance
        Advertisement.Load(adUnitId, new UnityAdsLoadListener(this));
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the player collides with the specific object (e.g., tagged "LethalObject")
        if (collision.gameObject.CompareTag("DeathZone"))
        {
            // Immediately set health to zero
            collisionCount = collisionThreshold;
            if (healthSlider != null)
            {
                healthSlider.value = 0;
            }

            // Show ad or destroy the player object depending on its tag
            if (gameObject.CompareTag("Player"))
            {
                ShowAd();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (collision.gameObject.CompareTag("Bullets"))
        {
            // Increment collision count and reduce health normally
            collisionCount++;

            if (healthSlider != null)
            {
                healthSlider.value = collisionThreshold - collisionCount;
            }

            // Check if health has reached zero
            if (collisionCount >= collisionThreshold)
            {
                if (gameObject.CompareTag("Player"))
                {
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
        if (adLoaded)
        {
            Advertisement.Show(adUnitId, new UnityAdsShowListener(this));
        }
        else
        {
            LoadGameOverScene();
        }
    }

    public void LoadGameOverScene()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }

    private class UnityAdsLoadListener : IUnityAdsLoadListener
    {
        private HealthManager healthManager;
        public UnityAdsLoadListener(HealthManager healthManager)
        {
            this.healthManager = healthManager;
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            healthManager.adLoaded = true;
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            healthManager.adLoaded = false;
            Debug.Log($"Error loading ad: {message}");
        }
    }

    private class UnityAdsShowListener : IUnityAdsShowListener
    {
        private HealthManager healthManager;
        public UnityAdsShowListener(HealthManager healthManager)
        {
            this.healthManager = healthManager;
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            healthManager.LoadGameOverScene();
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            healthManager.LoadGameOverScene();
            Debug.Log($"Error showing ad: {message}");
        }

        public void OnUnityAdsShowStart(string placementId) { }

        public void OnUnityAdsShowClick(string placementId) { }
    }
}
