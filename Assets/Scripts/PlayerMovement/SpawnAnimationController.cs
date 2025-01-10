using UnityEngine;

public class SpawnAnimationController : MonoBehaviour
{
    public Animator spawnAnimator;
    private bool _hasSpawned = false;
    void Start()
    {
        // Kalo belum pernah ngespawn
        if (!_hasSpawned) {
            spawnAnimator.enabled = true; // Lakukan animasi ngespawn
            _hasSpawned = true; // Tandai sudah pernah ngespawn
        }
        else {
            spawnAnimator.enabled = false; // Jangan lakukan animasi ngespawn
        }
    }
    void Update()
    {
        // Kalo animasi sudah selesai, hentikan animatornya
        if (spawnAnimator.enabled == true && !AnimatorIsPlaying())
        {
            Debug.Log("Animasi selesai!");
            spawnAnimator.enabled = false;
        }
    }
    bool AnimatorIsPlaying()
    {
        // Return apakah animasinya masih berjalan
        return spawnAnimator.GetCurrentAnimatorStateInfo(0).length > spawnAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
