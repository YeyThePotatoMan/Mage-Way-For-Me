using UnityEngine;
using UnityEngine.Events;

class FinishLevelController : MonoBehaviour
{
    public UnityEvent WinEvent;
    void Start() {
        if (WinEvent == null) WinEvent = new UnityEvent();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        WinEvent.Invoke(); // Bilang kalo sedang win
    }
}