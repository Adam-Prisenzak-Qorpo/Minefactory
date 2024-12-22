using System;
using Minefactory.Game;
using Minefactory.Storage;
using UnityEngine;

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

        private void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.CompareTag("Solid"))
            {
                isGrounded = true;
            }
            if (collider.CompareTag("Item"))
            {
                var collidedItem = collider.gameObject.GetComponent<ItemBehaviour>().item;
                if (collidedItem is null)
                {
                    Debug.LogError("Item on ground is null");
                    return;
                }
                WorldManager.activeBaseWorld.playerInventory.AddItem(collidedItem);
                Destroy(collider.gameObject);
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
            horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            var move = new Vector2(horizontal * speed, vertical * speed);

            if (horizontal < 0)
            {
                if (topWorld){
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                else{
                    transform.localScale = new Vector3(-1, 1, 1);
                }
            }
            else if (horizontal > 0)
            {
                if (topWorld){
                    transform.rotation = Quaternion.Euler(0, 0, -90);
                }
                else{
                    transform.localScale = new Vector3(1, 1, 1);
                }
            }
            if (topWorld)
            {
                rb.velocity = move;
                if (vertical > 0)
                {
                   transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (vertical < 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                }
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
            if (anim)
            {
                anim.SetFloat("horizontal", horizontal);
            }

        }

        public void IncreaseJumpHeight(float multiplier)
        {
            jumpForce *= multiplier;
            Debug.Log("Jump height increased! New jump force: " + jumpForce);
        }

    }
}
