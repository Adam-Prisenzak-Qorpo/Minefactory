using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public bool isGrounded;
    public bool topWorld;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        var move = new Vector2(horizontal * speed, vertical * speed);

        if (horizontal < 0)
        {
            sr.flipX = true;
        }
        else if (horizontal > 0)
        {
            sr.flipX = false;
        }
        if (topWorld)
        {
            rb.velocity = move;
            return;
        }
        move *= new Vector2(0, rb.velocity.y);

        float jump = Input.GetAxisRaw("Jump");

        if (vertical > 0.1f || jump > 0.1f)
        {
            if (isGrounded)
            {
                move.y = jumpForce;
            }
        }

        rb.velocity = move;
    }
}
