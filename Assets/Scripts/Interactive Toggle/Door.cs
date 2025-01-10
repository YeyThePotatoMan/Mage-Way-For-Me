using UnityEngine;

public class Door : MonoBehaviour
{
    // Open() -> opens the door up-way
    // Close() -> close the door down-way
    public float moveDistance = 3f; // How far the door moves to open/close
    public float moveSpeed = 2f;   // How fast the door moves
    private Vector3 initialPosition;
    private Vector3 openPosition;

    private void Start()
    {
        initialPosition = transform.position;
        openPosition = initialPosition + Vector3.up * moveDistance;
    }

    public void Open()
    {
        StopAllCoroutines();
        StartCoroutine(MoveDoor(openPosition));
    }

    public void Close()
    {
        StopAllCoroutines();
        StartCoroutine(MoveDoor(initialPosition));
    }

    private System.Collections.IEnumerator MoveDoor(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
