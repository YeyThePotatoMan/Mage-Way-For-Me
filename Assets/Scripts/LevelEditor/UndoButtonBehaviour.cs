using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the undo button in the level editor. It checks if the undo history is empty and disables the button if it is.
/// </summary>
public class UndoButtonBehaviour : MonoBehaviour
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
        LevelEditorManager.Instance.Undo();
        _button.interactable = LevelEditorManager.Instance.editHistory.Count != 0;
    }

    // Enable or disable the undo button based on the undo history.
    private void OnEdit() { _button.interactable = LevelEditorManager.Instance.editHistory.Count != 0; }
}
