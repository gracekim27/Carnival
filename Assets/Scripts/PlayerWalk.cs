using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalk : MonoBehaviour
{
    [SerializeField] private float speed;
    private Animator animator;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f);
        rb.MovePosition(transform.position + movement * speed * Time.deltaTime);

        // Update the animator parameters based on movement
        if (movement != Vector3.zero)
        {
            animator.SetBool("IsWalking", true);
            animator.SetFloat("Speed", movement.magnitude);
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
}