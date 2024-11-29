using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GunMovement : MonoBehaviour
{
    public Transform player;
    public Vector3 mousePos;
    public Vector3 mousePosRelative;
    public Vector3 portalGunPos;
    public float portalGunRotation;

    void Update ()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosRelative = mousePos - player.transform.position;
        portalGunPos = new Vector3(mousePosRelative.x, mousePosRelative.y, 0f).normalized * 0.5f + player.transform.position;

        portalGunRotation = Mathf.Atan(mousePosRelative.y/mousePosRelative.x) * Mathf.Rad2Deg;

        transform.position = portalGunPos;
        transform.localEulerAngles = new Vector3 (0f, 0f, portalGunRotation);
        
    }
}
