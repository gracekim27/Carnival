using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalk : MonoBehaviour
{
    [SerializeField] private float baseSpeed;
    private float speed;
    [SerializeField] private Collider2D swordCollider;
    private Animator animator;
    private Rigidbody2D rb;
    private float moveLimiter = 0.7f;
    private bool isKnockbackActive = false;

    [Header("Player Health")]
    [SerializeField] private int baseMaxHealth; // Set the player's maximum health
    public int maxHealth;
    public int currentHealth;
    [Header("Knockback and Flash")]
    private Transform lastAttacker;

    [SerializeField] private float knockbackStrength;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private float flashDuration;
    [SerializeField] private float attackMoveForce;
    [SerializeField] private float attackMoveDuration;
    [Header("Audio")]
    [SerializeField] private AudioClip[] woosh;
    [SerializeField] private AudioClip[] footstep;
    [SerializeField] private AudioClip playerHit;
    public bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        UpdatePlayerStats();
    }

    void Update()
    {
        if (isDead) return;
        Move();
        
        Animate();

        FlipCharacterBasedOnMousePosition();
    }

    private void UpdatePlayerStats()
    {
        if (ShopManager.Instance != null)
        {
            int armorLevel = ShopManager.Instance.armorLevel;
            speed = baseSpeed + armorLevel * 0.5f; // Increase speed by 0.5 units per armor level
            maxHealth = baseMaxHealth + armorLevel * 10; // Increase max health by 20 units per armor level
            currentHealth = maxHealth; // Reset current health to new max
        }
    }

    void Move()
    {
        if (isKnockbackActive) return; // Skip movement if taking knockback
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        if (moveHorizontal != 0 && moveVertical != 0)
        {
            moveHorizontal *= moveLimiter;
            moveVertical *= moveLimiter;
        }
        rb.velocity = new Vector2(moveHorizontal * speed, moveVertical * speed);
    }

    void Animate()
    {
        // Update the animator parameters based on movement
        if (rb.velocity != Vector2.zero)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
        // Check for mouse click to trigger fight animation
        if (Input.GetMouseButtonDown(0)) // 0 is the left mouse button
        {
            animator.SetTrigger("Fight");
        }
        
    }

    void PlayFootstep()
    {
        AudioClip stepSound = SoundFXManager.Instance.ChooseSoundFromList(footstep);
        SoundFXManager.Instance.PlaySound(stepSound, 0.4f);
    }

    void PlayWoosh()
    {
        AudioClip wooshSound = SoundFXManager.Instance.ChooseSoundFromList(woosh);
        SoundFXManager.Instance.PlaySound(wooshSound, 0.4f);
    }

    void FlipCharacterBasedOnMousePosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition; // Get mouse position in screen space
        Vector3 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position); // Convert player position to screen space

        // Flip the sprite based on mouse position relative to the player position
        if (mouseScreenPosition.x < playerScreenPosition.x)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
    }

    public void EnableSword()
    {
        swordCollider.enabled = true;
    }

    public void DisableSword()
    {
        swordCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("EnemySword"))
        {
            lastAttacker = other.transform.parent;
            TakeDamage(1); // Assuming each hit takes 10 health points
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(KnockbackAndFlash());
        SoundFXManager.Instance.PlaySound(playerHit, 0.3f, 0.7f);

        if (currentHealth <= 0)
        {
            Die(); // Call the Die function if health is 0 or less
        }
    }

    private IEnumerator KnockbackAndFlash()
    {
        isKnockbackActive = true;

        // Knockback
        Vector2 knockbackDirection = (transform.position - lastAttacker.position).normalized;
        rb.AddForce(knockbackDirection * knockbackStrength, ForceMode2D.Impulse);

        // Flash red
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        Color originalColor = Color.white;  // Assuming all sprites initially have the same color
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
    }

    public void ActivateAttackMove()
    {
        StartCoroutine(AttackMove(attackMoveDuration));
    }

    public IEnumerator AttackMove(float duration)
    {
        Vector3 mousePosition = Input.mousePosition; // Get the mouse position in screen coordinates
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition); // Convert it to world coordinates

        Vector2 forceDirection = new Vector2(mousePosition.x - transform.position.x, mousePosition.y - transform.position.y);
        forceDirection.Normalize(); // Normalize to get direction only

        isKnockbackActive = true; // Disable regular movement
        rb.AddForce(forceDirection * attackMoveForce, ForceMode2D.Impulse); // Apply the force as an impulse

        yield return new WaitForSeconds(duration); // Wait for the duration of the knockback

        isKnockbackActive = false; // Re-enable regular movement
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger("Die"); // Play the death animation
        // Keep the script active to allow for visual effects like flashing
        rb.velocity = Vector2.zero; // Stop all movement
        ShopManager.Instance.DeathScreen();
    }
}