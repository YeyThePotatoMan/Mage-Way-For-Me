using UnityEngine;
using UnityEngine.Events;

public class InteractableToggle : MonoBehaviour
{
    public UnityEvent onActivate; // Event triggered when the interaction occurs
    public UnityEvent onDeactivate; // Optional

    private bool _isActivated = false;

    public void Toggle()
    {
        _isActivated = !_isActivated;

        if (_isActivated)
        {
            onActivate.Invoke();
        }
        else
        {
            onDeactivate.Invoke();
        }
    }
}
