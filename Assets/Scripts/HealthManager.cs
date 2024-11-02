using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class HealthManager : MonoBehaviour, IUnityAdsInitializationListener
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

        // Initialize Unity Ads with a listener
        Debug.Log("Initializing Unity Ads...");
        Advertisement.Initialize(gameId, true, this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DeathZone"))
        {
            collisionCount = collisionThreshold;
            if (healthSlider != null)
            {
                healthSlider.value = 0;
            }

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
            collisionCount++;
            if (healthSlider != null)
            {
                healthSlider.value = collisionThreshold - collisionCount;
            }

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
        Debug.Log($"Attempting to show ad. Ad loaded status: {adLoaded}");
        if (adLoaded)
        {
            Advertisement.Show(adUnitId, new UnityAdsShowListener(this));
        }
        else
        {
            Debug.Log("Ad not loaded, loading Game Over Scene directly.");
            LoadGameOverScene();
        }
    }

    public void LoadGameOverScene()
    {
        Debug.Log("Loading Game Over Scene...");
        SceneManager.LoadScene(gameOverSceneName);
    }

    // IUnityAdsInitializationListener methods
    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");

        // Load an ad after successful initialization
        Advertisement.Load(adUnitId, new UnityAdsLoadListener(this));
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads initialization failed: {error.ToString()} - {message}");
    }

    // Nested class for loading ads
    private class UnityAdsLoadListener : IUnityAdsLoadListener
    {
        private HealthManager healthManager;

        public UnityAdsLoadListener(HealthManager healthManager)
        {
            this.healthManager = healthManager;
        }

        public void OnUnityAdsAdLoaded(string placementId)
        {
            Debug.Log("Ad loaded successfully.");
            healthManager.adLoaded = true;
        }

        public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
        {
            healthManager.adLoaded = false;
            Debug.Log($"Error loading ad: {error.ToString()} - {message}");
        }
    }

    // Nested class for showing ads
    private class UnityAdsShowListener : IUnityAdsShowListener
    {
        private HealthManager healthManager;

        public UnityAdsShowListener(HealthManager healthManager)
        {
            this.healthManager = healthManager;
        }

        public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
        {
            Debug.Log("Ad finished showing.");
            healthManager.LoadGameOverScene();
        }

        public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Error showing ad: {error.ToString()} - {message}");
            healthManager.LoadGameOverScene();
        }

        public void OnUnityAdsShowStart(string placementId)
        {
            Debug.Log("Ad started showing.");
        }

        public void OnUnityAdsShowClick(string placementId)
        {
            Debug.Log("Ad was clicked.");
        }
    }
}
