using UnityEngine;
using UnityEngine.UI;

public class SettingsButtonSetter : MonoBehaviour
{
    private Button _thisButton;
    private Button _backButton;
    private Canvas _settingsCanvas; // Reference to the settings canvas
    public Canvas _backToThisCanvas; // Reference to the canvas that should be returned to when the back button is clicked
    void Start()
    {
        _thisButton = GetComponent<Button>(); // Get the button component attached to this GameObject
        _settingsCanvas = SettingsManager.Instance.GetComponent<Canvas>(); // Access the settings canvas from a singleton manager (SettingsManager)
        _backButton = SettingsManager.Instance.GetComponentInChildren<Button>().gameObject.GetComponentInChildren<Button>(); // Find the back button within the SettingsManager's hierarchy
        SetCanvas(); // Set up the button interactions to enable the appropriate canvases
    }
    void SetCanvas()
    {
        _thisButton.onClick.AddListener(() => EnableCanvas(_settingsCanvas)); // Add a listener to this button to enable the settings canvas when clicked
        _backButton.onClick.AddListener(() => EnableCanvas(_backToThisCanvas)); // Add a listener to the back button to return to the previous canvas when clicked
    }
    void EnableCanvas(Canvas canvas)
    {
        canvas.enabled = true; // Set the canvas as enabled to make it visible
    }
}
