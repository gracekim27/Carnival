using UnityEngine;
using System.Collections;


public class EnemyScript : MonoBehaviour
{
    #region Setup code
    public enum State
    {
        Chasing,
        Attacking,
        Retreating,
        Dead // Added state for handling death
    }

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private Collider2D swordCollider; // Make sure to assign this in the Unity inspector
    [Header("Damage/Knockback")]
    [SerializeField] private int maxHealth;
    [SerializeField] private float knockbackStrength;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private float flashDuration;
    private bool isKnockbackActive = false;
    private int currentHealth;
    private Transform player;
    private Animator animator;
    private State currentState;
    [Header("Retreat Properties")]
    [SerializeField] private float retreatChance;
    private Vector3 retreatTarget;
    [SerializeField] private float baseRetreatDistance;
    [SerializeField] private float retreatVariance; // Variance in the retreat distance
    [Header("Attack Properties")]
    [SerializeField] private float attackRange;
    [SerializeField] private float attackCooldown; // Cooldown duration in seconds
    private float lastAttackTime;
    private bool isAttackOnCooldown = false;
    [Header("Coins")]
    [SerializeField] private GameObject coin;
    [SerializeField] private int enemyValue;
    [SerializeField] private int enemyValueVariance;
    public EnemySpawner spawner;
    [Header("Audio")]
    [SerializeField] private AudioClip[] hurt;
    [SerializeField] private AudioClip deathClang;

    #endregion

    void Start()
    {
        GameObject playerGameObject = GameObject.FindWithTag("Player");
        if (playerGameObject != null)
        {
            player = playerGameObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Please ensure the player is tagged correctly.");
        }

        animator = GetComponent<Animator>();
        currentState = State.Chasing; // Start with chasing the player
        currentHealth = maxHealth; // Initialize health
    }

    void Update()
    {
        CheckAndFlip();

        if (currentState == State.Dead) return; // Stop updates if dead
        if (isKnockbackActive) return; // Skip movement if in knockback

        HandleAttackCooldown();

        switch (currentState)
        {
            case State.Chasing:
                ChasePlayer();
                break;
            case State.Attacking:
                break;
            case State.Retreating:
                Retreat();
                break;
        }
    }

    # region Damage, Knockback, and Death

    public void TakeDamage(int damage)
    {
        PlayHurtSound();

        currentHealth -= damage;

        StartCoroutine(KnockbackAndFlash());

        if (currentHealth <= 0)
        {
            SoundFXManager.Instance.PlaySound(deathClang, 0.4f);
            Die();
        }
    }

    void PlayHurtSound()
    {
        AudioClip hurtSound = SoundFXManager.Instance.ChooseSoundFromList(hurt);
        SoundFXManager.Instance.PlaySound(hurtSound, 0.3f);
    }
    private IEnumerator KnockbackAndFlash()
    {
        isKnockbackActive = true;
        // Knockback
        Vector3 knockbackDirection = (transform.position - player.position).normalized;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(knockbackDirection * knockbackStrength, ForceMode2D.Impulse);
        }

        // Flash red
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        Color originalColor = renderers[0].color;  // Assuming all sprites initially have the same color
        foreach (var renderer in renderers)
        {
            renderer.color = Color.red; // Change to red
        }

        yield return new WaitForSeconds(flashDuration);

        // Reset color
        foreach (var renderer in renderers)
        {
            renderer.color = originalColor;
        }

        if (knockbackDuration > flashDuration)
        {
            yield return new WaitForSeconds(knockbackDuration - flashDuration);
        }

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0; 

        isKnockbackActive = false;
        if (currentState != State.Dead) currentState = State.Chasing;
    }

    private void Die()
    {
        currentState = State.Dead;
        animator.SetTrigger("Die");
    }

    public void DisappearOnDeath()
    {
        StartCoroutine(FadeOutAndDestroy(0.5f)); // Set the fade duration to 1.5 seconds, adjust as needed
    }

    private IEnumerator FadeOutAndDestroy(float duration)
    {
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (spriteRenderers.Length == 0)
        {
            Debug.LogError("No SpriteRenderers found on the GameObject or its children.");
            yield break;
        }

        float elapsed = 0;
        float currentAlpha = spriteRenderers[0].color.a;  // Assuming all renderers start with the same alpha

        while (elapsed < duration)
        {
            foreach (SpriteRenderer sr in spriteRenderers)
            {
                float alpha = Mathf.Lerp(currentAlpha, 0, elapsed / duration);
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, alpha);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        foreach (SpriteRenderer sr in spriteRenderers)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0); // Ensure all are fully transparent
        }

        DropCoins();

        Destroy(gameObject); // Remove the GameObject from the scene
    }

    private void DropCoins()
    {
        // Check the current level of the money upgrade
        int moneyLevel = ShopManager.Instance.moneyLevel;

        // Determine the multiplier or additional coins based on the money upgrade
        int bonusCoins = 3 * moneyLevel; // Each level adds 2 extra coins, adjust as needed

        // Determine number of coins to drop based on random variance around enemyValue
        int numCoins = enemyValue + Random.Range(-enemyValueVariance, enemyValueVariance + 1) + bonusCoins;

        // Instantiate coins at the enemy's position
        for (int i = 0; i < numCoins; i++)
        {
            Instantiate(coin, transform.position, Quaternion.identity);
        }
    }

    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.EnemyDestroyed();  // Notify the spawner that this enemy has been destroyed
        }
    }
    #endregion

    #region Movement
    void ChasePlayer()
    {
        if (player != null)
        {
            // Random chance to retreat
            if (Random.value < retreatChance)
            {
                StartRetreat();
                return;
            }
            if (Vector3.Distance(transform.position, player.position) <= attackRange && !isAttackOnCooldown)
            {
                currentState = State.Attacking;
                Attack();
            }
            else
            {
                MoveTowardsPlayer();
            }
        }
    }


    private void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }


    private void CheckAndFlip()
    {
        if (player != null)
        {
            // Flip the enemy's sprite by adjusting the localScale x-value
            if (player.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
        }
    }
    # endregion

    # region Attack and retreat
    private void HandleAttackCooldown()
    {
        if (isAttackOnCooldown && (Time.time - lastAttackTime >= attackCooldown))
        {
            isAttackOnCooldown = false; // Reset cooldown state
        }
    }
    private void Attack()
    {
        animator.SetTrigger("Fight");

        // Calculate retreat target with random variance
        Vector3 direction = (transform.position - player.position).normalized;
        float randomRetreatDistance = baseRetreatDistance + Random.Range(-retreatVariance, retreatVariance);
        retreatTarget = transform.position + direction * randomRetreatDistance;

        lastAttackTime = Time.time; // Update last attack time
        isAttackOnCooldown = true; // Set the cooldown flag
    }
    public void EnableSword()
    {
        swordCollider.enabled = true;
    }

    public void DisableSword()
    {
        swordCollider.enabled = false;
    }
    public void AttackAnimationEnded()
    {
        currentState = State.Retreating;
    }

    void StartRetreat()
    {
        // Calculate retreat target with random variance
        Vector3 direction = (transform.position - player.position).normalized;
        float randomRetreatDistance = baseRetreatDistance + Random.Range(-retreatVariance, retreatVariance);
        retreatTarget = transform.position + direction * randomRetreatDistance;
        currentState = State.Retreating;
    }
    void Retreat()
    {
        if ((transform.position - retreatTarget).sqrMagnitude > 0.1f)
        {
            Vector3 direction = (retreatTarget - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
        else
        {
            currentState = State.Chasing; // Return to chasing after retreating
        }
    }
    # endregion
}
