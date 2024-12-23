using System.Collections;
using UnityEngine;

/// <summary>
/// Behaviour of the portal projectile, from its initial speed to its lifetime duration.
/// When the projectile collides with a solid object, it triggers itself and spawns a portal according to its transform.
/// It is triggered either when it hits a solid object or when its lifetime duration is over.
/// </summary>
class PortalProjectileBehaviour : MonoBehaviour
{
  [Tooltip("Initial speed of the projectile.")]
  public float initialSpeed = 10f;
  [Tooltip("Lifetime duration in seconds of the projectile.")]
  public float lifetimeDuration = 5f;
  [Tooltip("Layer mask of solid objects the projectile should land on.")]
  public LayerMask solidLayer;

  private Rigidbody2D _rigidbody;

  private void Start()
  {
    _rigidbody = GetComponent<Rigidbody2D>();

    _rigidbody.linearVelocity = transform.right * initialSpeed;

    StartCoroutine(StartLifetime());
  }

  // When the projectile collides with a solid object, trigger it.
  private void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.gameObject.layer == solidLayer)
    {
      Trigger();
    }
  }

  // When the projectile doesn't land on anything after the lifetime duration, trigger it.
  private IEnumerator StartLifetime()
  {
    yield return new WaitForSeconds(lifetimeDuration);
    Trigger();
  }

  private void Trigger()
  {
    Destroy(gameObject);
  }
}