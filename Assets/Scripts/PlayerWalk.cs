using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalk : MonoBehaviour
{
    [SerializeField] private float speed;
    private Animator animator;
    private Rigidbody2D rb;
    private float moveLimiter = 0.7f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        
        Animate();

        FlipCharacterBasedOnMousePosition();
    }

    void Move()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");

        if (moveHorizontal != 0 && moveVertical != 0) {// Check for diagonal movement
            // limit movement speed diagonally, so you move at 70% speed
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
}