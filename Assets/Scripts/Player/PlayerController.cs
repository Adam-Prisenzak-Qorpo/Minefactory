using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (collision.CompareTag("Solid"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Solid"))
        {
            isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        bool worldChange = Input.GetKey(KeyCode.N);
        if (worldChange)
        {
            SceneManager.LoadScene(topWorld ? "Underground" : "TopWorld");
            return;
        }
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
        move.y = rb.velocity.y;

        float jump = Input.GetAxisRaw("Jump");
        Console.WriteLine(isGrounded);
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
