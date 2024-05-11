using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSceneTransition : MonoBehaviour
{
    // Name of the shop scene to load
    [SerializeField] private string shopSceneName;
    [SerializeField] private AudioClip tentSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ShopTent"))
        {
            SoundFXManager.Instance.PlaySound(tentSound, 0.6f);
            SceneManager.LoadScene(shopSceneName);
        }
    }
}