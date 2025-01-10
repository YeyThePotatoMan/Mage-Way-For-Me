using UnityEngine;
using UnityEngine.Events;

public class InteractableToggle : MonoBehaviour
{
    public UnityEvent onActivate; // Event triggered when the interaction occurs
    public UnityEvent onDeactivate; // Optional

    private bool isActivated = false;

    public void Toggle()
    {
        isActivated = !isActivated;

        if (isActivated)
        {
            onActivate.Invoke();
        }
        else
        {
            onDeactivate.Invoke();
        }
    }
}
