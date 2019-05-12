using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class PlatformerCharacter2D : MonoBehaviour
    {
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Transform m_CeilingCheck;   // A position marking where to check for ceilings
        const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
        private Animator m_Anim;            // Reference to the player's animator component.
        private Rigidbody2D m_Rigidbody2D;
        private bool m_FacingRight = true;  // For determining which way the player is currently facing.

        public bool isPlayer = false;

        public GameObject particlePrefab;
        bool canLand = true;

        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_CeilingCheck = transform.Find("CeilingCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            // Time.timeScale = 0.2f;
        }

        bool prevGrounded = false;
        private void FixedUpdate()
        {
            m_Grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    m_Grounded = true;
                }
            }

            m_Anim.SetBool("Ground", m_Grounded);

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);

            if (m_Grounded && !prevGrounded)
            {
                // Landed

            }
            else if (!m_Grounded && prevGrounded)
            {
                // Jumped
                canLand = true;
            }
            prevGrounded = m_Grounded;
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if (Lives.manager.isDead()) return;

            if (col.gameObject.layer == 11)
            {
                if (canLand)
                {
                    GameObject jumpParticles = Instantiate(particlePrefab);
                    jumpParticles.transform.position = new Vector2(col.contacts[0].point.x, col.contacts[0].point.y + 0.1f);
                    jumpParticles.GetComponent<Animator>().SetTrigger("Land");

                    canLand = false;
                }
                else
                {
                    GameObject jumpParticles = Instantiate(particlePrefab);
                    jumpParticles.transform.position = new Vector2(col.contacts[0].point.x, col.contacts[0].point.y + 0.1f);
                    jumpParticles.GetComponent<Animator>().SetTrigger("Run");
                }
            }
        }

        public void Move(float move, bool jump)
        {

            //only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
                m_Rigidbody2D.velocity = new Vector2(move * m_MaxSpeed, m_Rigidbody2D.velocity.y);

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }
            // If the player should jump...
            if (m_Grounded && jump && m_Anim.GetBool("Ground"))
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Anim.SetBool("Ground", false);
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));

                GameObject jumpParticles = Instantiate(particlePrefab);
                jumpParticles.transform.position = new Vector2(transform.position.x, transform.position.y - 0.26f);
                jumpParticles.GetComponent<Animator>().SetTrigger("Jump");
            }
        }

        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        public void Hit()
        {
            m_Anim.SetTrigger("Hit");
 
            if(isPlayer)
                Lives.manager.LoseLife();
        }

        public void Die()
        {
            if (isPlayer)
                GetComponent<Platformer2DUserControl>().enabled = false;
            else
                GetComponent<Enemy>().enabled = false;
            m_Anim.SetBool("Dead", true);
            m_Anim.SetTrigger("Die");
            GetComponent<Collider2D>().sharedMaterial = null;
            this.enabled = false;
        }

        public void Attack()
        {
            m_Anim.SetTrigger("Attack");
            Invoke("CheckHitPlayer", 0.1f);
        }

        void CheckHitPlayer()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.9f);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].tag == "Player")
                    colliders[i].gameObject.SendMessage("Hit");
            }
        }
    }
}
