using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private float m_jumpForces = 10f; // amount of Jump Forces
    private float m_playerSpeed = 3;
    public Rigidbody2D rb;
    public bool isGrounded;
    
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        isGrounded = false;
    }

    
    void Update()
    {
        PlayerController();

    }
    void PlayerController()
    {
        if (Input.GetKey(KeyCode.D)) //move to right
        {
            transform.localScale  = new Vector3(1, 1, 1); //mage facing to right upon pressing D
            rb.linearVelocity = new Vector2(m_playerSpeed, rb.linearVelocityY);
        }
        if (Input.GetKey(KeyCode.A)) //move to left
        {
            transform.localScale  = new Vector3(-1, 1, 1); //mage facing to left upon pressing A
            rb.linearVelocity = new Vector2(-m_playerSpeed, rb.linearVelocityY);
        }
        if (Input.GetKeyDown(KeyCode.W) && isGrounded) //jumping
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, m_jumpForces);
        }
        
    }
    private void OnCollisionEnter2D(Collision2D other) //when entering ground set isGrounded to true
    {
        if (other.gameObject.CompareTag("Ground")) //checking if player and gameObject with "Ground" tag colliding
        {
            isGrounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D other) //when leaving ground set isGrounded to false
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
