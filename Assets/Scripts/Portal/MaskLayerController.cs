using UnityEngine;

class MaskLayerController : MonoBehaviour
{
    private SpriteMask _mask;
    private int _maskLayer;
    void Start() {
        _mask = GetComponent<SpriteMask>();
        _maskLayer = _mask.frontSortingLayerID;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        TeleportData otherData = other.gameObject.GetComponent<TeleportData>();
        if (otherData == null) return;
        
        // Ubah layerMask supaya bisa tertutupi oleh mask nya saat lagi teleport
        if (otherData.spriteRenderer.sortingLayerID == otherData.originalLayer && otherData.isTeleporting)
        {
            otherData.spriteRenderer.sortingLayerID = _maskLayer;
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        TeleportData otherData = other.gameObject.GetComponent<TeleportData>();
        if (otherData == null) return;

        // Kembalikan ke layer aslinya karena sudah tidak perlu ditutupi oleh mask saat tidak teleport
        if (otherData.spriteRenderer.sortingLayerID == _maskLayer)
        {
            otherData.spriteRenderer.sortingLayerID = otherData.originalLayer;
        }
    }
}