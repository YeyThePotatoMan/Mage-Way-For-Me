using System.ComponentModel.Design;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public float runSpeed = 40f;
    private float _horizontalMove = 0f;
    [SerializeField] private float _jumpBufferTime = 0.2f;
    private float _jumpBufferCounter;
    private bool _active = true;
    private BoxCollider2D _playerCollider;
    private Rigidbody2D _playerRigidbody;
    void Start()
    {
        _playerCollider = GetComponent<BoxCollider2D>();
        _playerRigidbody = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        _horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (Input.GetButtonDown("Jump"))
        {
            _jumpBufferCounter = _jumpBufferTime;
        }
        else
        {
            _jumpBufferCounter -= Time.deltaTime;
        }
    }
    void FixedUpdate()
    {
        if(!_active) // Ketika sudah mati, karakter tidak bisa bergerak
        {
            return;
        }
        // Jalan horizontal setiap saat
        controller.Move(_horizontalMove * Time.fixedDeltaTime);

        if (_jumpBufferCounter > 0)
        {
            // Coba lompat
            if (controller.Jump()) // Berhasil lompat
            {
                _jumpBufferCounter = 0f; // Hilangkan buffer input
            }
        }
    }
    public void Die()
    {
        _active = false;
        _playerCollider.enabled = false;
        if(_playerRigidbody.linearVelocityX < 0f){
            _playerRigidbody.AddForce(new Vector2(100f, 200f));
        }
        else{
            _playerRigidbody.AddForce(new Vector2(-100f, 200f));
        }
    }
}