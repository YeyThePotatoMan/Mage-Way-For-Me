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
    private Camera _camera;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        if (_camera == null) Debug.LogError("Camera not found");
    }

    void Update()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");
        float mouseScrollAxis = Input.GetAxis("Mouse ScrollWheel");

        transform.position += 
            Time.deltaTime
            * _movementSpeed
            * _camera.orthographicSize
            * new Vector3(horizontalAxis, verticalAxis, 0);

        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize - mouseScrollAxis
            * _movementSpeed
            * _zoomSpeed,
        _minZoom, _maxZoom);
    }
}
