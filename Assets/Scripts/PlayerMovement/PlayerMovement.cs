using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public float runSpeed = 40f;
    private float _horizontalMove = 0f;
    [SerializeField] private float _jumpBufferTime = 0.2f;
    private float _jumpBufferCounter;

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
        // Jalan setiap saat
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
}