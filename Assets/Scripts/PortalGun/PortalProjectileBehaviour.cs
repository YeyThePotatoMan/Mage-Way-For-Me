using System.Collections;
using UnityEngine;

class PortalProjectileBehaviour : MonoBehaviour
{
  public float initialSpeed = 10f;
  public float lifetimeDuration = 5f;
  public LayerMask solidLayer;

  private Rigidbody2D _rigidbody;

  private void Start()
  {
    _rigidbody = GetComponent<Rigidbody2D>();

    _rigidbody.linearVelocity = transform.right * initialSpeed;

    StartCoroutine(StartLifetime());
  }

  private void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.gameObject.layer == solidLayer)
    {
      Trigger();
    }
  }

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