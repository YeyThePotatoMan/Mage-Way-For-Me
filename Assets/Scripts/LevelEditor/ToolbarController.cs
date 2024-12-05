using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Controls the toolbar type switching in the toolbar of the level editor.
/// </summary>
class ToolbarController : MonoBehaviour
{
  [SerializeField] private float _closeDuration = 0.5f;
  [SerializeField] private Canvas[] _toolbarContents;
  [SerializeField] private GameObject[] _toolbarTypeSelectors;
  [SerializeField] private Button _closeButton;

  private RectTransform _toolbarRectTransform;
  private Button[] _toolbarTypeSelectorButtons;
  private Image[] _toolbarTypeSelectorImages;
  private RectTransform _closeButtonRectTransform;
  // The index of the selected toolbar type. -1 means no toolbar type selected.
  private int _selectedToolbarTypeIndex = 0;
  private bool _isToolbarOpen = false;

  private void Start()
  {
    // Hide all toolbar contents except the first one.
    for (int i = 1; i < _toolbarContents.Length; i++)
    {
      if (_toolbarContents[i] == null) Debug.LogError("Toolbar content not set");
      else
      {
        _toolbarContents[i].enabled = false;
        _toolbarContents[i].gameObject.SetActive(false);
      }
    }

    // Get the toolbar type selector buttons and images.
    _toolbarTypeSelectorButtons = new Button[_toolbarTypeSelectors.Length];
    _toolbarTypeSelectorImages = new Image[_toolbarTypeSelectors.Length];
    for (int i = 0; i < _toolbarTypeSelectors.Length; i++)
    {
      if (_toolbarTypeSelectors[i] == null) Debug.LogError("Toolbar type selector not set");
      else
      {
        _toolbarTypeSelectorButtons[i] = _toolbarTypeSelectors[i].GetComponent<Button>();
        _toolbarTypeSelectorImages[i] = _toolbarTypeSelectors[i].GetComponent<Image>();

        // Add click event
        _toolbarTypeSelectorButtons[i].onClick.AddListener(ToolbarTypeButtonAction(i));
      }
    }

    // Handle close button
    _closeButton.onClick.AddListener(ToggleClose);
    _closeButtonRectTransform = _closeButton.GetComponent<RectTransform>();
    if (_closeButtonRectTransform == null) Debug.LogError("Close button rect transform not set");

    // Get the toolbar rect transform.
    _toolbarRectTransform = GetComponent<RectTransform>();
    if (_toolbarRectTransform == null) Debug.LogError("Toolbar rect transform not set");
  }

  /// <summary>
  /// Returns the action for the toolbar type button at the specified index.
  /// </summary>
  /// <param name="toolbarTypeIndex">The index of the toolbar type button.</param>
  /// <returns>The action for the toolbar type button.</returns>
  public UnityAction ToolbarTypeButtonAction(int toolbarTypeIndex)
  {
    return () =>
    {
      // Hide the previously selected toolbar content.
      if (_selectedToolbarTypeIndex != -1)
      {
        _toolbarContents[_selectedToolbarTypeIndex].enabled = false;
        _toolbarContents[_selectedToolbarTypeIndex].gameObject.SetActive(false);
        _toolbarTypeSelectorImages[_selectedToolbarTypeIndex].color = Color.white;
      }
      else if (_selectedToolbarTypeIndex >= _toolbarContents.Length)
      {
        Debug.LogError("Invalid toolbar type index");
        return;
      }

      // Update the selected toolbar type index.
      if (_selectedToolbarTypeIndex == toolbarTypeIndex) _selectedToolbarTypeIndex = -1;
      else
      {
        _selectedToolbarTypeIndex = toolbarTypeIndex;

        // Show the selected toolbar content.        
        _toolbarContents[_selectedToolbarTypeIndex].enabled = true;
        _toolbarContents[_selectedToolbarTypeIndex].gameObject.SetActive(true);
        _toolbarTypeSelectorImages[_selectedToolbarTypeIndex].color = new Color(1f, 1f, 1f, 0.5f);
      }
    };
  }

  public void ToggleClose()
  {
    if (_selectedToolbarTypeIndex != -1) ToolbarTypeButtonAction(_selectedToolbarTypeIndex)();

    _closeButtonRectTransform.localScale = new Vector3(
      _closeButtonRectTransform.localScale.x,
      _closeButtonRectTransform.localScale.y * -1,
      _closeButtonRectTransform.localScale.z
    );

    _isToolbarOpen = !_isToolbarOpen;

    StartCoroutine(ToggleCloseCoroutine());
  }

  private IEnumerator ToggleCloseCoroutine()
  {
    float time = 0;
    Vector3 targetLocalPos = _isToolbarOpen
    ? new Vector3(_toolbarRectTransform.localPosition.x,
      _toolbarRectTransform.localPosition.y + _toolbarRectTransform.sizeDelta.y,
      _toolbarRectTransform.localPosition.z)
    : new Vector3(_toolbarRectTransform.localPosition.x,
      _toolbarRectTransform.localPosition.y - _toolbarRectTransform.sizeDelta.y,
      _toolbarRectTransform.localPosition.z);

    while (time <= _closeDuration)
    {
      _toolbarRectTransform.localPosition = Vector3.Lerp(_toolbarRectTransform.localPosition, targetLocalPos, Mathf.Pow(time / _closeDuration, 2));
      time += Time.unscaledDeltaTime;
      yield return null;
    }
    _toolbarRectTransform.localPosition = targetLocalPos;
  }
}