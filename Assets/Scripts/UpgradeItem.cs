using UnityEngine;
using TMPro; // If using TextMeshPro, otherwise use UnityEngine.UI

public class UpgradeItem : MonoBehaviour
{
    public string upgradeType; // "sword", "armor", "money"
    public TextMeshProUGUI tooltipText; // Drag your TooltipText UI element here in the inspector
    [SerializeField] private AudioClip buySound;
    [SerializeField] private AudioClip errorSound;

    private void OnMouseDown()
    {
        if (ShopManager.Instance != null)
        {
            bool wasPurchased = ShopManager.Instance.PurchaseUpgrade(upgradeType);
            if (wasPurchased)
            {
                SoundFXManager.Instance.PlaySound(buySound);
                Debug.Log("Upgrade purchased: " + upgradeType);
            }
            else
            {
                SoundFXManager.Instance.PlaySound(errorSound, 1.6f);
                Debug.Log("Failed to purchase upgrade: " + upgradeType);
            }
        }
    }

    void OnMouseEnter()
    {
        if (tooltipText != null)
        {
            tooltipText.text = GetTooltipText(upgradeType); // Set the tooltip text
            SetTooltipPosition(upgradeType); // Set the position based on the upgrade type
            tooltipText.gameObject.SetActive(true); // Show the tooltip
        }
    }

    void OnMouseExit()
    {
        if (tooltipText != null)
        {
            tooltipText.gameObject.SetActive(false); // Hide the tooltip
        }
    }

    private string GetTooltipText(string type)
    {
        switch (type)
        {
            case "sword":
                return "+Damage +Range";
            case "armor":
                return "+Speed +Health";
            case "money":
                return "+Loot";
            default:
                return "Unknown Item";
        }
    }

    private void SetTooltipPosition(string type)
    {
        RectTransform tooltipTransform = tooltipText.rectTransform;
        tooltipTransform.position = new Vector3(transform.position.x, -2.3f, 0);
    }
}