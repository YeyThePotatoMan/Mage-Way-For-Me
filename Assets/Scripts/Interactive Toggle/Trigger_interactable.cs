using UnityEngine;

public class TriggerInteractable : MonoBehaviour
{
    public InteractableToggle interactable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            interactable.Toggle();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            interactable.Toggle();
        }
    }
}
