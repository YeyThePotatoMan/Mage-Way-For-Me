using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float _jumpForce = 400f;                            // Force added when the player jumps.
    [Range(0, .3f)][SerializeField] private float _movementSmoothing = .05f;   // Smoothing for movement.
    [SerializeField] private bool _airControl = false;                          // Can the player steer while jumping.
    [SerializeField] private LayerMask _whatIsGround;                           // What is considered ground.
    [SerializeField] private Collider2D _groundCheck;                            // Position to check if the player is grounded.

    private const float _groundedRadius = .2f; // Radius of the ground check circle.
    private bool _grounded;                   // Is the player grounded.
    private Rigidbody2D _rigidbody2D;
    private bool _facingRight = true;         // For determining the player's facing direction.
    private Vector3 _velocity = Vector3.zero;

    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = _grounded;
        _grounded = false;

        // Check if the player is grounded.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_groundCheck.transform.position, _groundedRadius, _whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                _grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
        // Debug.Log("Player Speed: " + _rigidbody2D.linearVelocityX);
    }

    public void Move(float move)
    {
        // Only control the player if grounded or airControl is enabled.
        if (_grounded || _airControl)//if airControl is unabled you can't control the player mid air
        {
            // Calculate the target velocity and smooth movement.
            Vector3 targetVelocity = new Vector2(move * 10f, _rigidbody2D.linearVelocityY);
            _rigidbody2D.linearVelocity = Vector3.SmoothDamp(_rigidbody2D.linearVelocity, targetVelocity, ref _velocity, _movementSmoothing);

            // Flip the player if needed.
            if (move > 0 && !_facingRight)
            {
                Flip();
            }
            else if (move < 0 && _facingRight)
            {
                Flip();
            }
        }
    }

    // Return apakah berhasil lompat
    public bool Jump()
    {
        // Handle jumping.
        if (_grounded)
        {
            _grounded = false;
            _rigidbody2D.AddForce(new Vector2(0f, _jumpForce));

            return true; // Berhasil lompat
        }
        else
        {
            return false;  // Tidak bisa lompat
        }
    }

    private void Flip()
    {
        // Switch the direction the player is facing.
        _facingRight = !_facingRight;

        // Invert the player's x scale.
        Vector3 theScale = transform.localScale;
        // Lagi hadap kanan dan keflip ke kiri
        if (_facingRight && Mathf.Sign(theScale.x) == -1)
        {
            theScale.x *= -1;
        }
        // Lagi hadap kiri dan belum diflip ke kiri
        if (!_facingRight && Mathf.Sign(theScale.x) == 1)
        {
            theScale.x *= -1;
        }
        transform.localScale = theScale;
    }
}
