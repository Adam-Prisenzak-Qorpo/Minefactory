using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Minefactory.Player
{
    public class PlayerController : MonoBehaviour
    {
        public float speed;
        public float jumpForce;
        public bool isGrounded;
        public bool topWorld;
        private Rigidbody2D rb;
        private SpriteRenderer[] srs;
        private Animator anim;
        private float horizontal;


        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            srs = GetComponentsInChildren<SpriteRenderer>();
            anim = GetComponent<Animator>();
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
            horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            var move = new Vector2(horizontal * speed, vertical * speed);

            if (horizontal < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (horizontal > 0)
            {
               transform.localScale = new Vector3(1, 1, 1);
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

        private void Update()
        {
            if(anim){
                anim.SetFloat("horizontal", horizontal);
            }
            
        }
        
    }
}
