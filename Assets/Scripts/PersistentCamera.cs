using UnityEngine;
using UnityEngine.SceneManagement;


public class PersistentCamera : MonoBehaviour
{
    public static PersistentCamera Instance; // Singleton pattern to easily access the instance of ShopManager

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make this gameObject persistent across scenes
        }
        else
        {
            Destroy(gameObject); // Ensures there is only one instance of ShopManager
        }
        SceneManager.LoadScene("Menu");
    }
}