using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneTransitionTrigger : MonoBehaviour
{
    [SerializeField] private SceneField sceneToLoad;

    private bool _isTransitioning;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only the player triggers
        if (!other.CompareTag("Player"))
        {
            return;
        }
        
        if (_isTransitioning)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(sceneToLoad))
        {
            Debug.LogError("No destination scene assigned!", this);
            return;
        }

        _isTransitioning = true;

        SceneManager.LoadScene(sceneToLoad);
    }
}
