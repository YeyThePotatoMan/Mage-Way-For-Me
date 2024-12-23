using System.Collections;
using UnityEngine;

public class PlayerRotationController : MonoBehaviour
{
    public float rotateDuration = 0.2f;
    public float rotationDelay = 0.2f;
    private bool _isRotating = false;
    void Update()
    {
        // Kalo rotasinya salah dan tidak sedang berotasi
        if (transform.rotation.eulerAngles.z != 0 && !_isRotating)
        {
            StartCoroutine(RotateOverTime(transform.rotation, Quaternion.Euler(0, 0, 0), rotateDuration, rotationDelay));
        }
    }
    // duration dan rotateAfter dalam detik
    IEnumerator RotateOverTime(Quaternion originalRotation, Quaternion finalRotation, float duration, float rotateAfter)
    {
        _isRotating = true; // Tandain sedang berotasi
        // Delay rotasinya
        while (rotateAfter > 0)
        {
            rotateAfter -= Time.deltaTime;
            yield return null;
        }
        
        float timePassed = 0f;
        transform.rotation = originalRotation; 
        while (timePassed < duration) // Lakukan rotasi
        {
            transform.rotation = Quaternion.Slerp(originalRotation, finalRotation, timePassed / duration);
            timePassed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = finalRotation;
        _isRotating = false; // Tandain selesai berotasi
    }
}
