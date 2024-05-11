using UnityEngine;
using TMPro; // Make sure to include this namespace for TextMeshPro elements
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public TextMeshProUGUI healthText; // Reference to the TextMeshPro UI element
    private PlayerWalk playerWalk; // Reference to the PlayerWalk script which manages health

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        UpdateHealthDisplay(); // Update health display every frame
    }

    private void UpdateHealthDisplay()
    {
        if (playerWalk != null && healthText != null)
        {
            healthText.text = "Health: " + playerWalk.currentHealth; // Update the health display with current health from PlayerWalk
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerWalk = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerWalk>(); // Find the PlayerWalk component
    }
}