using UnityEngine;

class SectionSwitchController : MonoBehaviour
{
    public Camera sectionCamera;
    public SectionSwitchController otherSection;
    void OnTriggerEnter2D(Collider2D other)
    {
        // Deteksi apakah player yang masuk trigger
        if (!other.CompareTag("Player")) return;
        sectionCamera.enabled = false;
        otherSection.sectionCamera.enabled = true;

        Vector3 initialPosition = other.transform.position; // Posisi awal player
        Vector3 deltaPosition = otherSection.transform.position - transform.position; // Perubahan posisi
        deltaPosition.z = 0; // Posisi z jangan diubah
        other.transform.position = initialPosition + deltaPosition; // Pindah posisi
    }
}