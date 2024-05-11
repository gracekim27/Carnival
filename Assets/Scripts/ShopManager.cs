using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;


public class ShopManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI swordPriceText;
    [SerializeField] private TextMeshProUGUI armorPriceText;
    [SerializeField] private TextMeshProUGUI moneyPriceText;
    public static ShopManager Instance; // Singleton pattern to easily access the instance of ShopManager

    [SerializeField] int[] upgradeCosts = {10, 20, 40, 60, 120}; // Cost for each level of upgrades
    public int swordLevel = 0;
    public int armorLevel = 0;
    public int moneyLevel = 0;
    public GameObject shopUI;  // Drag the ShopUI GameObject here in the inspector
    public GameObject menuUI;
    public GameObject gameUI;
    public string shopSceneName = "Shop";  // Set this to the exact name of your shop scene
    public string menuSceneName = "Menu";  // Set this to the exact name of your shop scene
    [SerializeField] private TextMeshProUGUI buyTextPrefab;
    [SerializeField] private Canvas buyTextCanvas;
    [SerializeField] private GameObject deathScreen;


    void Start()
    {
        UpdatePriceTexts();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("Menu");
        SceneManager.LoadScene("Bootstrap");
    }
    
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

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        deathScreen.SetActive(false);
        // Check if the loaded scene is the shop scene and activate/deactivate ShopUI accordingly
        if (scene.name == shopSceneName)
        {
            gameUI.SetActive(true);
            shopUI.SetActive(true);
            menuUI.SetActive(false);
        }
        else if (scene.name == menuSceneName)
        {
            gameUI.SetActive(false);
            shopUI.SetActive(false);
            menuUI.SetActive(true);
        }
        else
        {
            gameUI.SetActive(true);
            shopUI.SetActive(false);
            menuUI.SetActive(false);
        }
    }

    public int GetCurrentLevel(string upgradeType)
    {
        switch (upgradeType)
        {
            case "sword":
                return swordLevel;
            case "armor":
                return armorLevel;
            case "money":
                return moneyLevel;
            default:
                Debug.LogError("Invalid upgrade type provided: " + upgradeType);
                return -1;  // Return -1 to indicate an error
        }
    }

    public bool PurchaseUpgrade(string upgradeType)
    {
        int currentLevel = GetCurrentLevel(upgradeType);
        if (currentLevel == -1) return false;  // Error checking if invalid type was provided

        if (currentLevel < 5)
        {
            int cost = upgradeCosts[currentLevel];
            PlayerCoins playerCoins = GetComponent<PlayerCoins>();
            if (playerCoins.coins >= cost)
            {
                playerCoins.ChangeCoins(-cost); // Deduct the cost from player's coins
                IncreaseLevel(upgradeType);
                UpdatePriceTexts();
                CreateBuyText(-cost);  // Instantiate and animate the buy text
                return true;
            }
            else
            {
                Debug.Log("Not enough coins to purchase upgrade.");
                return false;
            }
        }
        else
        {
            Debug.Log("Maximum upgrade level reached.");
            return false;
        }
    }

    private void CreateBuyText(int cost)
    {
        TextMeshProUGUI buyTextInstance = Instantiate(buyTextPrefab, buyTextCanvas.transform);
        buyTextInstance.text = cost.ToString();  // Set the text to show the cost
        StartCoroutine(AnimateBuyText(buyTextInstance));  // Start the coroutine to animate the text
    }

    private IEnumerator AnimateBuyText(TextMeshProUGUI buyText)
    {
        float duration = 1.0f;  // Duration of the animation in seconds
        float time = 0;
        Vector3 startPosition = buyText.transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, -1, 0);  // Move down by 50 units

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            buyText.color = new Color(buyText.color.r, buyText.color.g, buyText.color.b, 1 - t);  // Fade out
            buyText.transform.position = Vector3.Lerp(startPosition, endPosition, t);  // Move down
            yield return null;
        }

        Destroy(buyText.gameObject);  // Destroy the text object after animation
    }

    public void UpdatePriceTexts()
    {
        // Update price texts based on the current levels, ensuring not to exceed array bounds
        if (swordLevel < upgradeCosts.Length)
            swordPriceText.text = $"${upgradeCosts[swordLevel]}";
        else
            swordPriceText.text = "Fully Upgraded";

        if (armorLevel < upgradeCosts.Length)
            armorPriceText.text = $"${upgradeCosts[armorLevel]}";
        else
            armorPriceText.text = "Fully Upgraded";

        if (moneyLevel < upgradeCosts.Length)
            moneyPriceText.text = $"${upgradeCosts[moneyLevel]}";
        else
            moneyPriceText.text = "Fully Upgraded";
    }

    private void IncreaseLevel(string upgradeType)
    {
        switch (upgradeType)
        {
            case "sword":
                swordLevel++;
                break;
            case "armor":
                armorLevel++;
                break;
            case "money":
                moneyLevel++;
                break;
        }
    }

    public int GetDifficultyScore()
    {
        return swordLevel + armorLevel + moneyLevel; // Each level ranges from 1 to 5
    }

    public void DeathScreen()
    {
        deathScreen.SetActive(true);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;  // Clean up delegate subscription
    }
}