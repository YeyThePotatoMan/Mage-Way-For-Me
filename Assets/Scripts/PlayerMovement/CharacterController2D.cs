using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float _JumpForce = 400f;                            // Force added when the player jumps.
    [Range(0, .3f)] [SerializeField] private float _MovementSmoothing = .05f;   // Smoothing for movement.
    [SerializeField] private bool _AirControl = false;                          // whether the player Can steer while mid air or not.
    [SerializeField] private LayerMask _WhatIsGround;                           // What is considered ground.
    [SerializeField] private Transform _GroundCheck;                            // Position to check if the player is grounded.

    private const float _GroundedRadius = .2f; // Radius of the ground check circle.
    private bool _Grounded;                   // Is the player grounded.
    private Rigidbody2D _Rigidbody2D;
    private bool _FacingRight = true;         // For determining the player's facing direction.
    private Vector3 _Velocity = Vector3.zero;

    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent;

    private void Awake()
    {
        _Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    private void FixedUpdate()
    {
        CheckGroundStatus();
    }
    private void CheckGroundStatus()
    {
        bool wasGrounded = _Grounded;
        _Grounded = false;

        // Check if the player is grounded.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_GroundCheck.position, _GroundedRadius, _WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                _Grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }

    public void Move(float move, bool jump)
    {
        // Only control the player if grounded or airControl is enabled.
        if (_Grounded || _AirControl)//if airControl is unabled you can't control the player mid air
        {
            // Calculate the target velocity and smooth movement.
            Vector3 targetVelocity = new Vector2(move * 10f, _Rigidbody2D.linearVelocityY);
            _Rigidbody2D.linearVelocity = Vector3.SmoothDamp(_Rigidbody2D.linearVelocity, targetVelocity, ref _Velocity, _MovementSmoothing);

            // Flip the player if needed.
            if (move > 0 && !_FacingRight)
            {
                Flip();
            }
            else if (move < 0 && _FacingRight)
            {
                Flip();
            }
        }

        // Handle jumping.
        if (_Grounded && jump)
        {
            _Grounded = false;
            _Rigidbody2D.AddForce(new Vector2(0f, _JumpForce));
        }
    }

    private void Flip()
    {
        // Switch the direction the player is facing by multiplying player x scale by -1
        _FacingRight = !_FacingRight;

        
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
