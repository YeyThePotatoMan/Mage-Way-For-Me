using System;
using UnityEngine;

class TeleportData : MonoBehaviour
{
    [HideInInspector] public Renderer spriteRenderer;
    [HideInInspector] public int originalLayer;
    [HideInInspector] public bool isTeleporting = false;
    [HideInInspector] public Vector2 teleportDirection;
    [HideInInspector] public float suctionVelocity;
    private Rigidbody2D _rigidbody;
    public GameObject clone;
    void Awake()
    {
        spriteRenderer = GetComponent<Renderer>();
        originalLayer = spriteRenderer.sortingLayerID;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (isTeleporting)
        {
            Debug.Log(name + " sedang tersedot dengan arah " + teleportDirection.x + " " + teleportDirection.y);
            // Membuat efek portal menyedot objek
            Vector2 velocity = _rigidbody.linearVelocity;
            Vector2 projection = VectorHelper.VectorProjection(velocity, teleportDirection);
            if (projection.magnitude < suctionVelocity) {
                // Mengenolkan vektor ke arah masuk/keluar portal
                velocity -= projection;
                // Memberi kecepatan untuk masuk/keluar portak
                velocity += teleportDirection * suctionVelocity;
                // Kasih ke rigidbody
                _rigidbody.linearVelocity = velocity;
            }
        }
    }
}