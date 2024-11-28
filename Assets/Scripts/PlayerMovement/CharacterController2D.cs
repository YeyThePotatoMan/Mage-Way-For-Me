using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                            // Force added when the player jumps.
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;   // Smoothing for movement.
    [SerializeField] private bool m_AirControl = false;                          // Can the player steer while jumping.
    [SerializeField] private LayerMask m_WhatIsGround;                           // What is considered ground.
    [SerializeField] private Transform m_GroundCheck;                            // Position to check if the player is grounded.

    private const float k_GroundedRadius = .2f; // Radius of the ground check circle.
    private bool m_Grounded;                   // Is the player grounded.
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;         // For determining the player's facing direction.
    private Vector3 m_Velocity = Vector3.zero;

    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // Check if the player is grounded.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }

    public void Move(float move, bool jump)
    {
        // Only control the player if grounded or airControl is enabled.
        if (m_Grounded || m_AirControl)
        {
            // Calculate the target velocity and smooth movement.
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.linearVelocityY);
            m_Rigidbody2D.linearVelocity = Vector3.SmoothDamp(m_Rigidbody2D.linearVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // Flip the player if needed.
            if (move > 0 && !m_FacingRight)
            {
                Flip();
            }
            else if (move < 0 && m_FacingRight)
            {
                Flip();
            }
        }

        // Handle jumping.
        if (m_Grounded && jump)
        {
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    private void Flip()
    {
        // Switch the direction the player is facing.
        m_FacingRight = !m_FacingRight;

        // Invert the player's x scale.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
