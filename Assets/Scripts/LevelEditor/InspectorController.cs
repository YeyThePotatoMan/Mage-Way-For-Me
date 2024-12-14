using UnityEngine;

public class InspectorController : MonoBehaviour
{
    private Canvas _canvas;

    private void Start() {
        _canvas = GetComponent<Canvas>();
        if (_canvas == null) Debug.LogError("Canvas component not found.");
    }

    public void Close() {
        _canvas.enabled = false;
    }
}
