using UnityEngine;

class TeleportData : MonoBehaviour
{
    private int _originalLayer;
    public bool isTeleporting = false;
    public GameObject clone;
    public Vector2 enterVector;

    void Start()
    {
        _originalLayer = gameObject.layer;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Mask") && gameObject.layer == _originalLayer)
        {
            // Ubah layerMask supaya bisa tertutupi oleh mask nya saat lagi teleport
            gameObject.layer = other.gameObject.layer;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Mask") && gameObject.layer == other.gameObject.layer)
        {
            // Kembalikan ke layer aslinya karena sudah tidak perlu ditutupi oleh mask saat tidak teleport
            gameObject.layer = _originalLayer;
        }
    }
}