namespace FrogGame.Gameplay
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Triggers a scene transition when the player enters the level exit zone.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class LevelTransitionTrigger : MonoBehaviour
    {
        [Header("Scene Settings")]
        [Tooltip("Exact name of the scene to load in Build Settings")]
        [SerializeField] private string targetSceneName = "GameplayTest_Artist_Manglar";

        private bool hasTriggered = false;

        private void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (hasTriggered) return;

            if (other.CompareTag("Player"))
            {
                hasTriggered = true;
                LoadNextLevel();
            }
        }

        private void LoadNextLevel()
        {
            Time.timeScale = 1f;

            if (!string.IsNullOrEmpty(targetSceneName))
            {
                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                Debug.LogError("[LevelTransitionTrigger] Target Scene Name is not set!");
            }
        }
    }
}