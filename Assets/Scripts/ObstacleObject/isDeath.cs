using TMPro;
using UnityEngine;

public class isDeath : MonoBehaviour
{
    private void OnCollisionEnted2D(Collision2D other)
    {
        var player = other.collider.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.Die();
        }
    }
}
