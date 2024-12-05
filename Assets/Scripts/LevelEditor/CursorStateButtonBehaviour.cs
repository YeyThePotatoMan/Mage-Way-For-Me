using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls the cursor state buttons in the level editor. It checks if the current cursor state is the same as the button's cursor state and disables the button if it is.
/// </summary>
public class CursorStateButtonBehaviour : MonoBehaviour
{
    [SerializeField] private LevelEditorManager.CursorState _cursorState;

    private Button _button;
    
    private void Start()
    {
        _button = GetComponent<Button>();
        if (_button == null) Debug.LogError("Button component not found");
        _button.interactable = LevelEditorManager.Instance.CurrentCursorState != _cursorState;

        _button.onClick.AddListener(OnClick);
        LevelEditorManager.Instance.CursorChangeEvent.AddListener(OnCursorStateChange);
    }

    private void OnClick() { LevelEditorManager.Instance.ChangeCursorState(_cursorState); }

    private void OnCursorStateChange(LevelEditorManager.CursorState cursorState) { _button.interactable = cursorState != _cursorState; }
}
