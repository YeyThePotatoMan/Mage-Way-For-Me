using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This script is responsible for controlling the camera in the level editor.
/// </summary>
public class LevelEditorCameraController : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _zoomSpeed = 5f;
    [SerializeField] private float _minZoom = 5f;
    [SerializeField] private float _maxZoom = 50f;

    private bool _isDragging = false;
    private Vector3 _startDraggingScreenPos;
    private Vector3 _startDraggingTransformPos;

    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if (_camera == null) Debug.LogError("Camera not found");
    }

    void Update()
    {
        if (_isDragging) // Move camera with Cursor.
        {
            if (Input.GetMouseButtonUp(2) || (LevelEditorManager.Instance.CurrentCursorState == LevelEditorManager.CursorState.Default && Input.GetMouseButtonUp(0))) // End cursor mvoement.
            {
                _isDragging = false;
                return;
            }

            Vector3 dragOffset = Input.mousePosition - _startDraggingScreenPos;
            Vector3 worldDragOffset = _camera.ScreenToWorldPoint(new Vector3(dragOffset.x, dragOffset.y, _camera.nearClipPlane)) - _camera.ScreenToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
            transform.position = _startDraggingTransformPos - worldDragOffset;
        }
        else
        {
            if (!UserInterfaceHelper.IsPointerOverUIElement()
                && (
                    Input.GetMouseButtonDown(2)
                    || (LevelEditorManager.Instance.CurrentCursorState == LevelEditorManager.CursorState.Default && Input.GetMouseButtonDown(0)))) // Start cursor movement.
            {
                _isDragging = true;
                _startDraggingScreenPos = Input.mousePosition;
                _startDraggingTransformPos = transform.position;
                return;
            }

            float horizontalAxis = Input.GetAxis("Horizontal");
            float verticalAxis = Input.GetAxis("Vertical");

            transform.position +=
                Time.deltaTime
                * _movementSpeed
                * _camera.orthographicSize
                * new Vector3(horizontalAxis, verticalAxis, 0);
        }

        float mouseScrollAxis = Input.GetAxis("Mouse ScrollWheel");
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - mouseScrollAxis
            * _movementSpeed
            * _zoomSpeed,
        _minZoom, _maxZoom);
    }
}
