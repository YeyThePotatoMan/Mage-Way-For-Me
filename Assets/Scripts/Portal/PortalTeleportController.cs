using UnityEngine;

public class PortalTeleportController : MonoBehaviour
{
    private const float _suctionVelocity = 4f;
    private Collider2D _thisCollider;
    public PortalTeleportController otherPortal; // Other portal to teleport to
    private Transform _otherPortalTransform; // Other portal to teleport to
    private Collider2D _otherPortalCollider; // Other portal to teleport to
    private Vector2 _inVector; // Vektor arah masuk keluar
    void Start()
    {
        _thisCollider = GetComponent<Collider2D>();
        _otherPortalTransform = otherPortal.GetComponent<Transform>();
        _otherPortalCollider = otherPortal.GetComponent<Collider2D>();

        // Default arah masuk keluar portal
        _inVector = Vector2.left;
        if (transform.localScale.x < 0) // Kalo skala x terbalik
        {
            _inVector.x *= -1;
        }
        if (transform.localScale.y < 0) // Kalo skala y terbalik
        {
            _inVector.y *= -1;
        }
        // Sesuaikan arah dengan rotasi portal
        _inVector = RotateVector(_inVector, transform.rotation.eulerAngles.z);
        Debug.Log(name + ": " + _inVector.x + ", " + _inVector.y);
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
        // Tandai lagi di portal, supaya clone di portal lain tidak membuat clone lagi
        otherData.isTeleporting = true;

        Vector3 toCenter = transform.position - other.transform.position;

        // Flip game object tergantung perubahan arah
        bool flipX = Mathf.Sign(transform.localScale.x) == Mathf.Sign(_otherPortalTransform.localScale.x);
        bool flipY = Mathf.Sign(transform.localScale.y) != Mathf.Sign(_otherPortalTransform.localScale.y);
        if (flipX)
        {
            toCenter.x = -toCenter.x;
        }
        if (flipY)
        {
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
        otherData.clone.name = otherData.name;

        // Samakan kecepatan
        var originalRigidBody = otherData.gameObject.GetComponent<Rigidbody2D>();
        var cloneRigidBody = otherData.clone.gameObject.GetComponent<Rigidbody2D>();
        if (originalRigidBody.linearVelocity.magnitude < _suctionVelocity)
        {
            originalRigidBody.linearVelocity = originalRigidBody.linearVelocity.normalized * _suctionVelocity;
        }
        cloneRigidBody.linearVelocity = RotateVector(originalRigidBody.linearVelocity, angle);

        if (flipX)
        {
            otherData.clone.transform.localScale *= new Vector2(-1, 1);
            cloneRigidBody.linearVelocity *= new Vector2(-1, 1);
        }
        if (flipY)
        {
            otherData.clone.transform.localScale *= new Vector2(1, -1);
            cloneRigidBody.linearVelocity *= new Vector2(1, -1);
        }

    }
    void OnTriggerExit2D(Collider2D other)
    {
        TeleportData otherData = other.gameObject.GetComponent<TeleportData>();
        if (otherData == null) return;

        // Sudah keluar dari portal
        otherData.isTeleporting = false;
        // Kalau arah vektor keluar searah dengan arah vektor masuk, berarti berhasil masuk portal dan pindah
        float exitAngle = AngleBetweenTwoVector(other.transform.position - transform.position, _inVector);
        if (Mathf.Abs(exitAngle) >= 90)
        {
            Destroy(other.gameObject);
        }
    }

    private Vector2 RotateVector(Vector2 a, float angle)
    {
        /*
        Matrix rotasi
        |x'| = |cos -sin||x|
        |y'|   |sin cos ||y|
        */
        // Ubah jadi radian
        angle *= Mathf.Deg2Rad;
        float x = a.x * Mathf.Cos(angle) - a.y * Mathf.Sin(angle);
        float y = a.x * Mathf.Sin(angle) + a.y * Mathf.Cos(angle);
        return new Vector2(x, y);
    }
    private Vector2 VectorProjection(Vector2 a, Vector2 b)
    {
        /*
        Misalkan c adalah proyeksi vektor a pada b
        c = a•b / |b|² × b
        c = a•b / |b| × b.normalized
        */
        return Vector2.Dot(a, b) / b.magnitude * b.normalized;
    }
    // Mengembalikan sudut rotasi yang dibutuhkan untuk merotasi a menjadi searah dengan b
    // Kembalian berupa angka pada interval (-180, 180)
    private float AngleBetweenTwoVector(Vector2 a, Vector2 b)
    {
        return ((Mathf.Atan2(b.y, b.x) - Mathf.Atan2(a.y, a.x)) * Mathf.Rad2Deg + 180) % 360 - 180;
    }
}
