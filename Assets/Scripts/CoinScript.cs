using UnityEngine;
using System.Collections;

public class CoinScript : MonoBehaviour
{
    [SerializeField] private float minVelocity;
    [SerializeField] private float baseMaxVelocity; // Base maximum velocity
    private float maxVelocity; // This will now be dynamic
    [SerializeField] private float fadeDuration; // Duration of the fade-out effect
    [SerializeField] private float riseSpeed; // Speed at which the coin moves upwards during the fade-out
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioClip[] coinPickup;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateMaxVelocity(); // Update the maxVelocity based on the money level
        ApplyInitialForce();
    }

    private void UpdateMaxVelocity()
    {
        if (ShopManager.Instance != null)
        {
            int moneyLevel = ShopManager.Instance.moneyLevel; // Get the current level of money upgrade
            maxVelocity = baseMaxVelocity + moneyLevel * 1.5f; // Increase max velocity by 1.5 for each level of money upgrade
        }
    }

    private void ApplyInitialForce()
    {
        float velocity = Random.Range(minVelocity, maxVelocity);
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        rb.AddForce(direction * velocity, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PickupCoin();
        }
    }

    private void PickupCoin()
    {
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.GetComponent<PlayerCoins>().AddCoin(); // Call AddCoin on the PlayerCoins script
        }
        AudioClip coinSound = SoundFXManager.Instance.ChooseSoundFromList(coinPickup);
        SoundFXManager.Instance.PlaySound(coinSound);
        StartCoroutine(FadeOutAndRise()); // Start the fade-out and rise coroutine
    }

    private IEnumerator FadeOutAndRise()
    {
        float elapsedTime = 0;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        Vector3 initialPosition = transform.position;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            transform.position = new Vector3(initialPosition.x, initialPosition.y + (riseSpeed * elapsedTime), initialPosition.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject); // Destroy the coin after the effect is complete
    }
}