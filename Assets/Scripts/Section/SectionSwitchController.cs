using System.Collections;
using UnityEngine;

class SectionSwitchController : MonoBehaviour
{
    public Camera sectionCamera; // Kamera yang dipake di ruangan ini
    public SectionSwitchController otherSectionDoor; // Pintu di ruangan lain
    [HideInInspector] public bool canSwitch = true; // Bisa ganti ruangan atau tidak
    private bool _toggleOnExit = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        // Deteksi apakah player yang masuk trigger atau sedang tidak bisa ganti ruangan
        if (!other.CompareTag("Player")) return;
        if (!canSwitch)
        {
            _toggleOnExit = true;
            return;
        }
        sectionCamera.enabled = false;
        otherSectionDoor.sectionCamera.enabled = true;

        // Tidak boleh langsung ganti ruangan, ini dilakukan supaya OnTriggerEnter2D tidak langsung ke-trigger
        canSwitch = false;
        StartCoroutine(CanSwitchDelay());
        otherSectionDoor.canSwitch = false;
        StartCoroutine(otherSectionDoor.CanSwitchDelay());

        Vector3 initialPosition = other.transform.position; // Posisi awal player
        Vector3 deltaPosition = otherSectionDoor.transform.position - transform.position; // Perubahan posisi
        deltaPosition.z = 0; // Posisi z jangan diubah
        other.transform.position = initialPosition + deltaPosition; // Pindah posisi
    }
    void OnTriggerExit2D(Collider2D other)
    {
        // Deteksi apakah player yang keluar trigger
        if (!other.CompareTag("Player")) return;
        if (!canSwitch && _toggleOnExit)
        {
            Debug.Log("Toggled on Exit!");
            canSwitch = true;
            _toggleOnExit = false;
        }
    }


    public IEnumerator CanSwitchDelay()
    {
        // Tunggu 3 frame baru boleh ganti ruangan
        yield return null;
        yield return null;
        yield return null;
        if (!canSwitch && !_toggleOnExit)
        {
            Debug.Log("Toggled normally!");
            canSwitch = true;
        }
    }
}