using UnityEngine;

class CollisionLayerController : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other) {
        // Kalau objectnya bukan object yang bisa diteleportasikan, tidak perlu exclude collisionnya dengan "Ground"
        // Kalau sedang di portal, tidak perlu exclude lagi collisionnya dengan "Ground"
        TeleportData otherData = other.gameObject.GetComponent<TeleportData>();
        if (otherData == null) return;
        other.excludeLayers |= 1 << LayerMask.NameToLayer("Ground"); // Jadikan bisa tembus tanah
    }
    void OnTriggerExit2D(Collider2D other) {
        // Kalau objectnya bukan object yang bisa diteleportasikan, tidak perlu mencoba include collisionnya dengan "Ground"
        TeleportData otherData = other.gameObject.GetComponent<TeleportData>();
        if (otherData == null) return;
        other.excludeLayers = 0; // Kembali tidak bisa tembus tanah
    }
}