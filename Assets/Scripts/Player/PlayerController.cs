using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering.Universal;
using Minefactory.Game;
using Minefactory.Storage;

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
        private Light2D light2D;
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



        private void ApplyMovementSpeedUpgrade(float newMovementSpeed)
        {
            speed = newMovementSpeed;
        }
        
        private void OnEnable()
        {
            StartCoroutine(WaitForSkillTreeManager());
            StartCoroutine(WaitForGameStateManager());
        }

        private void OnDisable()
        {
            SkillTreeManager.Instance.OnMovementSpeedPurchased -= ApplyMovementSpeedUpgrade;
        }

        private void JumpForceUpgrade(){
            float newJumpForce = GameStateManager.Instance.GetSharedState("JumpForce", jumpForce); //either we get the upgrade, or just stick to the current jumpForce
            jumpForce = newJumpForce;
        }

        private void FlashlightUpgrade(){
            light2D = GetComponentInChildren<Light2D>();

            if (light2D == null)
            {
                return;
            }

            float newFlashlightRadius = GameStateManager.Instance.GetSharedState("Flashlight", light2D.pointLightOuterRadius);
            light2D.pointLightOuterRadius = newFlashlightRadius;
        }

        private void MovementSpeedUpgrade(){
            float newMovementSpeed = GameStateManager.Instance.GetSharedState("Speed", speed); 
            speed = newMovementSpeed;
        }
        
        private IEnumerator WaitForGameStateManager()
        {
            while (GameStateManager.Instance == null)
            {
                yield return null; // Wait until the next frame
            }

            JumpForceUpgrade();
            FlashlightUpgrade();
            MovementSpeedUpgrade();
        }

        private IEnumerator WaitForSkillTreeManager()
        {
            while (SkillTreeManager.Instance == null)
            {
                yield return null; // Wait until the next frame
            }
            SkillTreeManager.Instance.OnMovementSpeedPurchased += ApplyMovementSpeedUpgrade;
        }
    }
}