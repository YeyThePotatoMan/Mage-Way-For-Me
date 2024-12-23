using UnityEngine;

public class GunShootingBehaviour : MonoBehaviour
{
    public float cooldownDuration = 0.5f;

    [SerializeField]
    private GameObject _projectilePrefab;    
    private float _lastShotTime;

    private void Start() {
        _lastShotTime = Time.unscaledTime - cooldownDuration;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.unscaledTime - _lastShotTime >= cooldownDuration) {
            Instantiate(_projectilePrefab, transform.position, transform.rotation);
            _lastShotTime = Time.unscaledTime;
        };
    }
}
