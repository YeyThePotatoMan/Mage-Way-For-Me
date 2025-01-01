using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

class SectionSwitchController : MonoBehaviour
{
    public Camera sectionCamera;
    public SectionSwitchController otherSection;
    [HideInInspector] public bool canSwitch = true; // Bisa ganti ruangan
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
        otherSection.sectionCamera.enabled = true;

        // Tidak boleh langsung ganti ruangan, ini dilakukan supaya OnTriggerEnter2D tidak langsung ke-trigger
        canSwitch = false;
        StartCoroutine(CanSwitchDelay());
        otherSection.canSwitch = false;
        StartCoroutine(otherSection.CanSwitchDelay());

        Vector3 initialPosition = other.transform.position; // Posisi awal player
        Vector3 deltaPosition = otherSection.transform.position - transform.position; // Perubahan posisi
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