using UnityEngine;

/// <summary>
/// Behaviour of the gun movement, following the player's mouse position.
/// </summary>
public class GunMovement : MonoBehaviour
{
    [Tooltip("Player transform to follow.")]
    public Transform player;

    private Vector3 mousePos;
    private Vector3 mousePosRelative;
    private Vector3 portalGunPos;
    private float portalGunRotation;

    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosRelative = mousePos - player.transform.position;
        portalGunPos = new Vector3(mousePosRelative.x, mousePosRelative.y, 0f).normalized * 0.5f + player.transform.position;

        portalGunRotation = Mathf.Atan2(mousePosRelative.y, mousePosRelative.x) * Mathf.Rad2Deg;

        transform.position = portalGunPos;        
        transform.rotation = Quaternion.AngleAxis(portalGunRotation, Vector3.forward);

    }
}
