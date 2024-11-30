using UnityEngine;
using UnityEngine.UI;

public class SettingsButtonSetter : MonoBehaviour
{
    private Button _thisButton;
    private Button _backButton;
    private Canvas _settingsCanvas;
    public Canvas _backToThisCanvas;
    void Start()
    {
        _thisButton = GetComponent<Button>();
        _settingsCanvas = SettingsManager.Instance.GetComponent<Canvas>();
        _backButton = SettingsManager.Instance.GetComponentInChildren<Button>().gameObject.GetComponentInChildren<Button>();
        SetCanvas();
    }
    void SetCanvas()
    {
        _thisButton.onClick.AddListener(() => EnableCanvas(_settingsCanvas));
        _backButton.onClick.AddListener(() => EnableCanvas(_backToThisCanvas));
    }
    void EnableCanvas(Canvas canvas)
    {
        canvas.enabled = true;
    }
}
