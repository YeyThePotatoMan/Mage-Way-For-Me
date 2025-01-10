using System;
using Unity.VisualScripting;
using UnityEngine;

public class IsDeath : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        var player = other.collider.GetComponent<PlayerMovement>();
        if (player != null)
        {
            Debug.Log("death");
            player.Die();
        }
    }
}