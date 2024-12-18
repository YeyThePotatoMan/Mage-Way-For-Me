using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Handling PlayerMovement
public class PlayerMovement : MonoBehaviour
{
    //Assign Character Controller
    //Variable for movement speed, jump, and Jumpbuffer

    public CharacterController2D controller;
    public float runSpeed = 40f; //value of running speed
    float horizontalMove = 0f;
    bool jump = false;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }
    void FixedUpdate()//Activing the JumpBuffer
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump);//moving horizontally and jumping
        jump = false;
        jumpBufferCounter = 0f;
    }
}