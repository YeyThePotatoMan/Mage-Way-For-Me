using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the redo button in the level editor. It checks if the redo history is empty and disables the button if it is.
/// </summary>
public class RedoButtonBehaviour : MonoBehaviour
{
    private Button _button;
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClick);
    }

    private void Start()
    {
        OnEdit();
        LevelEditorManager.Instance.EditEvent.AddListener(OnEdit);
    }

    private void OnClick()
    {
        LevelEditorManager.Instance.Redo();
        _button.interactable = LevelEditorManager.Instance.redoHistory.Count != 0;
    }

    // Enable or disable the redo button based on the redo history.
    private void OnEdit() { _button.interactable = LevelEditorManager.Instance.redoHistory.Count != 0; }
}
