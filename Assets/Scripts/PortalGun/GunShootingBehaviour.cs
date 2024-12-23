using UnityEngine;

/// <summary>
/// Behaviour of the shooting mechanism of the gun, including projectile instantiation and cooldown.
/// </summary>
public class GunShootingBehaviour : MonoBehaviour
{
    [Tooltip("Cooldown duration in seconds between each shot.")]
    public float cooldownDuration = 0.5f;

    [SerializeField]
    [Tooltip("Prefab of the projectile to shoot.")]
    private GameObject _projectilePrefab;    
    // Last time the gun shot a projectile
    private float _lastShotTime;

    private void Start() {
        _lastShotTime = Time.unscaledTime - cooldownDuration;
    }

    private void Update()
    {
        // If the player clicks and the cooldown is over, shoot a projectile
        if (Input.GetMouseButtonDown(0) && Time.unscaledTime - _lastShotTime >= cooldownDuration) {
            Instantiate(_projectilePrefab, transform.position, transform.rotation);
            _lastShotTime = Time.unscaledTime;
        };
    }
}
