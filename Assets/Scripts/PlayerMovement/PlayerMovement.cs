using System.Collections;
using System.ComponentModel.Design;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private float _rotationSpeed = 180f;
    public float restartTime = 1.5f; // Waktu untuk delay reload scene ketika mati
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
        var _playerRotationController = GetComponent<PlayerRotationController>();
        var _animator = GetComponent<Animator>();
        _playerRotationController.enabled = false;
        _animator.enabled = false;

        _active = false;
        _playerCollider.enabled = false;
        _playerRigidbody.freezeRotation = false;
        float _impulse = _rotationSpeed * Mathf.Deg2Rad;


        if(_playerRigidbody.linearVelocityX < 0f){
            _playerRigidbody.AddForce(new UnityEngine.Vector2(100f, 200f));
            // _playerRigidbody.AddTorque(_impulse, ForceMode2D.Impulse);
            _playerRigidbody.angularVelocity = -_rotationSpeed;
        }
        else{
            _playerRigidbody.AddForce(new UnityEngine.Vector2(-100f, 200f));
            // _playerRigidbody.AddTorque(_impulse, ForceMode2D.Impulse);
            _playerRigidbody.angularVelocity = _rotationSpeed;
        }
        StartCoroutine(Delay(restartTime));
    }

    IEnumerator Delay(float seconds) {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

