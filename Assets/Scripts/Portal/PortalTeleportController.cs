using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class PortalTeleportController : MonoBehaviour
{
    private Collider2D _thisCollider;
    public PortalTeleportController otherPortal; // Other portal to teleport to
    private Transform _otherPortalTransform; // Other portal to teleport to
    private Collider2D _otherPortalCollider; // Other portal to teleport to
    void Start()
    {
        _thisCollider = GetComponent<Collider2D>();
        _otherPortalTransform = otherPortal.GetComponent<Transform>();
        _otherPortalCollider = otherPortal.GetComponent<Collider2D>();
    }
    public void SetActive(bool active)
    {
        // if portal aktif, maka isTrigger = true
        // else jadi collider biasa
        _thisCollider.isTrigger = active;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // Kalau portal lainnya tidak ada atau sedang tidak aktif, jangan lakukan teleportasi
        if (otherPortal == null || _otherPortalCollider.isTrigger == false) return;

        // Kalau objectnya bukan object yang bisa diteleportasikan, jangan lakukan teleportasi
        // Kalau sedang di portal, jangan lakukan teleportasi lagi
        TeleportData otherData = other.gameObject.GetComponent<TeleportData>();
        if (otherData == null || otherData.isTeleporting) return;
        // Tandai lagi di portal
        otherData.isTeleporting = true;

        Vector3 toCenter = transform.position - other.transform.position;

        // Flip game object tergantung perubahan arah
        bool flipX = Mathf.Sign(transform.localScale.x) != Mathf.Sign(_otherPortalTransform.localScale.x);
        bool flipY = Mathf.Sign(transform.localScale.y) != Mathf.Sign(_otherPortalTransform.localScale.y);
        if (flipX)
        {
            other.gameObject.transform.localScale *= new Vector2(-1, 1);
            toCenter.x = -toCenter.x;
        }
        if (flipY)
        {
            other.gameObject.transform.localScale *= new Vector2(1, -1);
            toCenter.y = -toCenter.y;
        }

        // Rotasikan sesuai arah keluar
        float angle = _otherPortalTransform.eulerAngles.z - transform.eulerAngles.z;
        toCenter = RotateVector(toCenter, angle);
        other.transform.rotation = Quaternion.Euler(0, 0, other.transform.eulerAngles.z + angle);

        // Copy gameObjectnya ke portal lain
        otherData.clone = Instantiate(other.gameObject);
        float initialZ = otherData.clone.transform.position.z;
        Vector3 finalPosition = _otherPortalTransform.position - toCenter;
        finalPosition.z = initialZ;
        otherData.clone.transform.position = finalPosition;

        // Samakan kecepatan
        var originalRigidBody = otherData.gameObject.GetComponent<Rigidbody2D>();
        var cloneRigidBody = otherData.clone.gameObject.GetComponent<Rigidbody2D>();
        cloneRigidBody.linearVelocity = RotateVector(originalRigidBody.linearVelocity, angle);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        TeleportData otherData = other.gameObject.GetComponent<TeleportData>();
        if (otherData == null) return;

        // Sudah keluar dari portal
        otherData.isTeleporting = false;
        if (otherData.clone != null) {
            Destroy(otherData.gameObject);
        }
    }

    // Rotate vector with angle in degree
    private Vector2 RotateVector(Vector2 a, float angle)
    {
        float x = a.x * Mathf.Cos(angle) - a.y * Mathf.Sin(angle);
        float y = a.x * Mathf.Sin(angle) + a.y * Mathf.Cos(angle);
        return new Vector2(x, y);
    }
}
