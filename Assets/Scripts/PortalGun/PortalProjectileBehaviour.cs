using System;
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
  
  [CloneReference]
  [HideInInspector]
  public Action<GameObject> onTrigger;

  private Rigidbody2D _rigidbody;

  private void Start()
  {
    _rigidbody = GetComponent<Rigidbody2D>();

    _rigidbody.linearVelocity = transform.right * initialSpeed;

    StartCoroutine(StartLifetime());
  }

  // When the projectile collides with a solid object, trigger it.
  private void OnCollisionEnter2D(Collision2D coll)
  {
    if (((1 << coll.gameObject.layer) & solidLayer) != 0) Trigger();
  }  

  // When the projectile doesn't land on anything after the lifetime duration, trigger it.
  private IEnumerator StartLifetime()
  {
    yield return new WaitForSeconds(lifetimeDuration);
    Trigger();
  }

  private void Trigger()
  {        
    onTrigger?.Invoke(gameObject);
    Destroy(gameObject);
  }
}