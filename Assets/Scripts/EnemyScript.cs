using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public enum State
    {
        Chasing,
        Attacking,
        Retreating
    }

    [SerializeField] private float speed;
    [SerializeField] private float attackRange;
    [SerializeField] private float baseRetreatDistance;
    [SerializeField] private float retreatVariance; // Variance in the retreat distance
    private Transform player;
    private Animator animator;
    private Vector3 retreatTarget;
    private State currentState;

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
    }

    void Update()
    {
        CheckAndFlip();

        switch (currentState)
        {
            case State.Chasing:
                ChasePlayer();
                break;
            case State.Attacking:
                // Attack logic is handled by Animator's events or timer
                break;
            case State.Retreating:
                Retreat();
                break;
        }
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

    void ChasePlayer()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            currentState = State.Attacking;
            animator.SetTrigger("Fight");
        }
        else if (player != null)
        {
            MoveTowardsPlayer();
        }
    }

    void OnAttackComplete()
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

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    public void AttackAnimationEnded()
    {
        OnAttackComplete();
    }
}
