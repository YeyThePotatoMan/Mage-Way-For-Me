using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the behavior of the layer selection button in the level editor.
/// </summary>
public class LayerSelectButtonBehaviour : MonoBehaviour
{    
    /// The index of the tilemap associated with this button.    
    [SerializeField] private int _tilemapIndex;
        
    private Button _button;
        
    private void Start()
    {
        _button = GetComponent<Button>();
        if (_button == null) Debug.LogError("Button component not found");
        _button.interactable = LevelEditorManager.Instance.SelectedTilemapIndex != _tilemapIndex;

        _button.onClick.AddListener(OnClick);
        LevelEditorManager.Instance.TilemapChangeEvent.AddListener(OnTilemapChange);
    }
    
    /// <summary>
    /// Called when the button is clicked. Changes the selected tilemap in the level editor.
    /// </summary>
    private void OnClick() { LevelEditorManager.Instance.ChangeTilemap(_tilemapIndex); }
    
    /// <summary>
    /// Called when the selected tilemap changes. Updates the button's interactable state.
    /// </summary>
    /// <param name="index">The index of the newly selected tilemap.</param>    
    private void OnTilemapChange(int index) { _button.interactable = index != _tilemapIndex; }
}
