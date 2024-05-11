using UnityEngine;
using TMPro;

public class PlayerCoins : MonoBehaviour
{
    public int coins = 0;  // Number of coins the player has collected
    public TextMeshProUGUI coinText;  // Reference to the TextMeshPro GUI component

    void Start()
    {
        UpdateCoinText();  // Update the UI text on start
    }

    public void AddCoin()
    {
        coins++;  // Increment the coin count
        UpdateCoinText();  // Update the UI text
    }

    public void ChangeCoins(int amount)
    {
        coins += amount;  // Modify coin count by a specific amount
        UpdateCoinText();  // Update the UI text whenever coins change
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
            coinText.text = "Coins: " + coins;  // Update the text to show the current coin count
    }
}