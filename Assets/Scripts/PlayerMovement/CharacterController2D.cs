using UnityEngine;
using UnityEngine.Events;


//this scrip control the character such jump, walking, and detecting ground/collision
//attach this script to a 2d character GameObject in unity
public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float _jumpForce = 400f;                            // Force added when the player jumps.
    [Range(0, .3f)] [SerializeField] private float _movementSmoothing = .05f;   // Smoothing for movement.
    [SerializeField] private bool _airControl = false;                          // whether the player Can steer while mid air or not.
    [SerializeField] private LayerMask _whatIsGround;                           // What is considered ground.
    [SerializeField] private Transform _groundCheck;                            // Position to check if the player is grounded.

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
        CheckGroundStatus();//calling the function CheckGroundStatus
    }
    
    
    private void CheckGroundStatus()//Detecting whether the character is on the ground or not
    {
        bool wasGrounded = _grounded;
        _grounded = false;

        // Check if the player is grounded.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_groundCheck.position, _groundedRadius, _whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                _grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }
    }

    public void Move(float move, bool jump)//these included feature that whether you can control the character mid-air or not
    //and making GameObject facing right or left upon pressing  'A' or 'D' button
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

        // Handle jumping.
        if (_grounded && jump) //making it's not possible to jump midair
        {
            _grounded = false;
            _rigidbody2D.AddForce(new Vector2(0f, _jumpForce));
        }
    }

    private void Flip()
    {
        // Switch the direction the player is facing by multiplying player x scale by -1
        _facingRight = !_facingRight;

        
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
