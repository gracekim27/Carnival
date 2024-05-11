using UnityEngine;

public class SwordScript : MonoBehaviour
{
    [SerializeField] private int baseDamage = 10; // Base damage without upgrades
    [SerializeField] private float baseScale = 1.0f; // Base scale of the sword
    private int damage;

    private void Start()
    {
        UpdateSwordStats();
    }

    private void UpdateSwordStats()
    {
        if (ShopManager.Instance != null)
        {
            int swordLevel = ShopManager.Instance.swordLevel; // Get the current level of sword upgrade
            float sizeMultiplier = 1.0f + 0.2f * swordLevel; // Increase size by 20% per level
            int additionalDamage = 1 * swordLevel; // Each level adds 5 more damage

            transform.localScale = new Vector3(baseScale * sizeMultiplier, baseScale * sizeMultiplier, baseScale * sizeMultiplier);
            GetComponent<Collider2D>().transform.localScale = transform.localScale; // Update collider size if needed
            damage = baseDamage + additionalDamage;
        }
        else
        {
            Debug.LogError("ShopManager instance not found. Make sure ShopManager is initialized and set as Singleton.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyScript enemy = collision.GetComponent<EnemyScript>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}