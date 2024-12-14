using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/// <summary>
/// Controls the toolbar tiles in the level editor.
/// </summary>
public class ToolbarPlacementController : MonoBehaviour
{
    [SerializeField] private string _resourceKey;
    [Header("Prefabs")]
    // The prefab for the toolbar tile UI.
    [SerializeField] private GameObject _toolbarPlacementUIPrefab;

    [Header("Layouting")]
    // The gap between toolbar tiles. it is also used as the horizontal padding.
    [SerializeField] private float _contentGap = 20f;

    // The list of button components in the toolbar content.
    private List<Button> _buttons = new();
    // The list of image components in the toolbar content.
    private List<Image> _images = new();
    // The list of tiles acquired from the resources folder.
    private List<LevelEditorManager.Placement> _placementResources = new();
    // Selected tile index, -1 means no tile selected.
    private int _selectedTileIndex = -1;

    private void Start()
    {
        if (_toolbarPlacementUIPrefab == null) Debug.LogError("Toolbar tile prefab not set");

        // Get all tile resources                        
        _placementResources.AddRange(Array.ConvertAll(Resources.LoadAll(_resourceKey), item =>
        {
            if (item.GetType() == typeof(Tile)) return new LevelEditorManager.Placement
            {
                name = ((Tile)item).name,
                tile = (Tile)item,
                sprite = ((Tile)item).sprite,
            };
            else if (item.GetType() == typeof(GameObject))
            {
                LevelEditorManager.Instance.level.props.Add(Path.Join(_resourceKey, ((GameObject)item).name));
                LevelEditorManager.Instance.level.propResources.Add((GameObject)item);
                return new LevelEditorManager.Placement
                {
                    name = ((GameObject)item).name,
                    propPrefab = (GameObject)item,
                    sprite = ((GameObject)item).GetComponent<SpriteRenderer>().sprite,
                    propIndex = LevelEditorManager.Instance.level.props.Count - 1,
                };
            }
            Debug.LogError($"Invalid resource type in the Resources/{_resourceKey} folder.");
            return new LevelEditorManager.Placement { };
        }).Where(item => item.sprite != null));

        // Create toolbar tiles
        _buttons = new(_placementResources.Count);
        _images = new(_placementResources.Count);
        for (int i = 0; i < _placementResources.Count; i++)
        {
            // Instantiate
            GameObject toolbarTile = Instantiate(_toolbarPlacementUIPrefab, transform);
            Image toolbarTileImage = toolbarTile.GetComponent<Image>();
            RectTransform toolbarTileRectTransform = toolbarTile.GetComponent<RectTransform>();
            toolbarTileImage.sprite = _placementResources[i].sprite;
            toolbarTileRectTransform.anchoredPosition = new Vector2(
                _contentGap                                                 // Padding
                + i * (toolbarTileRectTransform.sizeDelta.x + _contentGap)  // Gap between tiles
            , 0);
            toolbarTile.name = _placementResources[i].name;

            // Fetch components
            _buttons.Add(toolbarTile.GetComponent<Button>());
            _images.Add(toolbarTile.GetComponent<Image>());

            // Add click event
            _buttons[i].onClick.AddListener(OnChildrenClickAction(i));
        }
    }

    // Deselect the tile when the toolbar is disabled.
    private void OnDisable()
    {
        SelectTile(-1);
    }

    /// <summary>
    /// HOF that returns a UnityAction that selects the tile at the given index. It is used for the click event of the toolbar tiles.
    /// </summary>
    /// <param name="index">The index of the tile to select.</param>
    /// <returns>A UnityAction that selects the tile at the given index.</returns>
    private UnityAction OnChildrenClickAction(int index)
    {
        return () => SelectTile(index);
    }

    /// <summary>
    /// Selects the tile of the toolbar at the given index.
    /// </summary>
    /// <param name="index">The index of the tile to select.</param>
    public void SelectTile(int index)
    {
        if (index == _selectedTileIndex)
        {
            _selectedTileIndex = -1;
            LevelEditorManager.Instance.ChangeCursorState(LevelEditorManager.CursorState.Default);
        }
        else
        {
            _selectedTileIndex = index;
            LevelEditorManager.Instance.ChangeCursorState(LevelEditorManager.CursorState.Brush);
        }

        // Change the alpha of the images based on the selected tile
        for (int i = 0; i < _images.Count; i++)
        {
            _images[i].color = i == _selectedTileIndex
                ? new Color(_images[i].color.r, _images[i].color.g, _images[i].color.b, 0.5f)
                : new Color(_images[i].color.r, _images[i].color.g, _images[i].color.b, 1f);
        }

        // Set the selected tile to the level editor manager
        if (_selectedTileIndex != -1)
        {
            LevelEditorManager.Instance.selectedPlacementToPlace = _placementResources[_selectedTileIndex];
        }
        else LevelEditorManager.Instance.selectedPlacementToPlace = null;
    }
}
