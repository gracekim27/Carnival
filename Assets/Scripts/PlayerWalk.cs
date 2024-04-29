using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalk : MonoBehaviour
{
    public float speed = 5.0f;
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

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.MovePosition(transform.position + movement * speed * Time.deltaTime);

        // Update the animator parameters
        if (movement != Vector3.zero)
        {
            animator.SetFloat("Speed", 1.0f);  // Change 1.0 to vary animation speed
        }
        else
        {
            animator.SetFloat("Speed", 0.0f);
        }
    }
}


