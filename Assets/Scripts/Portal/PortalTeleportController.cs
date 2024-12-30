using UnityEngine;

public class PortalTeleportController : MonoBehaviour
{
    [SerializeField] private float _suctionVelocity = 10f; // Kecepatan benda saat masuk/keluar portal supaya tidak sengaja clone 2x
    private Collider2D _thisCollider; // Collider portal ini
    public PortalTeleportController otherPortal; // Portal lain sebagai tujuan teleport
    [HideInInspector] public LayerMask otherPortalMask; // Layer mask dari portal lain
    private Transform _otherPortalTransform; // Transform portal lain
    private Collider2D _otherPortalCollider; // Collider portal lain
    [HideInInspector] public Vector2 inVector; // Vektor arah masuk keluar
    void Start()
    {
        _thisCollider = GetComponent<Collider2D>();
        _otherPortalTransform = otherPortal.GetComponent<Transform>();
        _otherPortalCollider = otherPortal.GetComponent<Collider2D>();
        otherPortalMask = otherPortal.GetComponentInChildren<SpriteMask>().frontSortingLayerID;

        UpdateInVector();
        Debug.Log(name + " inVector: " + inVector.x + " " + inVector.y);
    }
    public void SetColliderActive(bool active)
    {
        // if portal aktif, maka isTrigger = true
        // else jadi collider biasa
        _thisCollider.isTrigger = active;
    }
    private void UpdateInVector()
    {
        // Default arah masuk keluar portal
        inVector = Vector2.left;
        if (transform.localScale.x < 0) // Kalo skala x terbalik
        {
            inVector.x *= -1;
        }
        if (transform.localScale.y < 0) // Kalo skala y terbalik
        {
            inVector.y *= -1;
        }
        // Sesuaikan arah dengan rotasi portal
        inVector = VectorHelper.RotateVector(inVector, transform.rotation.eulerAngles.z);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // Update inVector supaya tidak salah arah
        UpdateInVector();

        // Kalau portal lainnya tidak ada atau sedang tidak aktif, jangan lakukan teleportasi
        if (otherPortal == null || _otherPortalCollider.isTrigger == false) return;

        // Kalau objectnya bukan object yang bisa diteleportasikan, jangan lakukan teleportasi
        // Kalau sedang di portal, jangan lakukan teleportasi lagi
        TeleportData otherData = other.gameObject.GetComponent<TeleportData>();
        if (otherData == null || otherData.isTeleporting || otherData.clone != null) return;
        // Tandai lagi di portal, supaya clone di portal lain tidak membuat clone lagi
        otherData.isTeleporting = true;

        // Copy gameObjectnya ke portal lain
        otherData.clone = Instantiate(other.gameObject);
        otherData.clone.name = otherData.name; // Biar nama clonenya nggak kepanjangan

        // Clonenya clone dari diri sendiri adalah diri sendiri
        TeleportData cloneData = otherData.clone.GetComponent<TeleportData>();
        cloneData.clone = other.gameObject;

        // Supaya clonenya kena masknya portal lain
        cloneData.spriteRenderer.sortingLayerID = otherPortalMask;

        // Untuk transformasi posisi clone
        Vector2 toCenter = transform.position - other.transform.position;

        // Flip posisi game object tergantung perubahan arah
        bool isFlippedX = Mathf.Sign(transform.localScale.x) == Mathf.Sign(_otherPortalTransform.localScale.x);
        bool isFlippedY = Mathf.Sign(transform.localScale.y) != Mathf.Sign(_otherPortalTransform.localScale.y);
        if (isFlippedX)
        {
            toCenter -= 2 * VectorHelper.VectorProjection(toCenter, inVector);
        }
        if (isFlippedY)
        {
            toCenter -= 2 * VectorHelper.VectorProjection(toCenter, Vector2.Perpendicular(inVector));
        }

        // Rotasikan posisi dan rotasi sesuai arah keluar
        float angle = _otherPortalTransform.eulerAngles.z - transform.eulerAngles.z;
        toCenter = VectorHelper.RotateVector(toCenter, angle);
        otherData.clone.transform.rotation = Quaternion.Euler(0, 0, other.transform.eulerAngles.z + angle);

        // Sesuaikan posisi
        float initialZ = otherData.clone.transform.position.z;
        Vector3 finalPosition = _otherPortalTransform.position;
        finalPosition.x -= toCenter.x;
        finalPosition.y -= toCenter.y;
        finalPosition.z = initialZ;
        otherData.clone.transform.position = finalPosition;

        // Atur kecepatan clone sesuai rotasi dan localScale portal tujuan
        var cloneRigidBody = otherData.clone.GetComponent<Rigidbody2D>();
        cloneRigidBody.linearVelocity = VectorHelper.RotateVector(cloneRigidBody.linearVelocity, -transform.rotation.eulerAngles.z);
        if (isFlippedX)
        {
            otherData.clone.transform.localScale *= new Vector2(-1, 1);
            cloneRigidBody.linearVelocity *= new Vector2(-1, 1);
        }
        if (isFlippedY)
        {
            otherData.clone.transform.localScale *= new Vector2(-1, 1);
            cloneRigidBody.linearVelocity *= new Vector2(1, -1);
        }
        otherData.clone.transform.rotation = Quaternion.Euler(0, 0, otherData.clone.transform.rotation.eulerAngles.z + 180);

        cloneRigidBody.linearVelocity = VectorHelper.RotateVector(cloneRigidBody.linearVelocity, _otherPortalTransform.rotation.eulerAngles.z);

        // Untuk arah penyedotan oleh portal
        otherData.teleportDirection = -inVector;
        cloneData.teleportDirection = otherPortal.inVector;
        otherData.suctionVelocity = _suctionVelocity;
        cloneData.suctionVelocity = _suctionVelocity;
    }
    void OnTriggerExit2D(Collider2D other)
    {
        TeleportData otherData = other.gameObject.GetComponent<TeleportData>();
        if (otherData == null) return;

        // Sudah keluar dari portal
        otherData.isTeleporting = false;

        if (otherData.clone == null) return; // Kalo nggak ada clone, sebaiknya jangan dihancurkan, nanti hilang total
        // Kalau arah vektor keluar searah dengan arah vektor masuk, berarti berhasil masuk portal dan pindah
        float exitAngle = VectorHelper.AngleBetweenTwoVector(other.transform.position - transform.position, inVector);
        if (Mathf.Abs(exitAngle) >= 90)
        {
            Destroy(other.gameObject);
        }
        else
        {
            Destroy(otherData.clone);
        }
    }
}
