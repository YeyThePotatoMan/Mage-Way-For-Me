using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

/// <summary>
/// Controls the toolbar tiles in the level editor.
/// </summary>
public class ToolbarTileController : MonoBehaviour
{
    [Header("Prefabs")]
    // The prefab for the toolbar tile.
    [SerializeField] private GameObject _toolbarTilePrefab;

    [Header("Layouting")]
    // The gap between toolbar tiles. it is also used as the horizontal padding.
    [SerializeField] private float _contentGap = 20f;

    // The list of button components in the toolbar content.
    private List<Button> _buttons = new();
    // The list of image components in the toolbar content.
    private List<Image> _images = new();
    // The list of tiles acquired from the resources folder.
    private List<Tile> _tileResources = new();
    // Selected tile index, -1 means no tile selected.
    private int _selectedTileIndex = -1;

    private void Start()
    {
        if (_toolbarTilePrefab == null) Debug.LogError("Toolbar tile prefab not set");

        // Get all tile resources
        _tileResources.AddRange(Resources.LoadAll<Tile>("Tiles"));

        // Create toolbar tiles
        _buttons = new(_tileResources.Count);
        _images = new(_tileResources.Count);
        for (int i = 0; i < _tileResources.Count; i++)
        {
            // Instantiate
            GameObject toolbarTile = Instantiate(_toolbarTilePrefab, transform);
            Image toolbarTileImage = toolbarTile.GetComponent<Image>();
            toolbarTileImage.sprite = _tileResources[i].sprite;
            toolbarTile.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                _contentGap                                                                         // Padding
                + i * (_toolbarTilePrefab.GetComponent<RectTransform>().sizeDelta.x + _contentGap)  // Gap between tiles
            , 0);
            toolbarTile.name = _tileResources[i].name;

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
        if (index == _selectedTileIndex) _selectedTileIndex = -1;
        else _selectedTileIndex = index;

        for (int i = 0; i < _images.Count; i++)
        {
            _images[i].color = i == _selectedTileIndex
                ? new Color(_images[i].color.r, _images[i].color.g, _images[i].color.b, 0.5f)
                : new Color(_images[i].color.r, _images[i].color.g, _images[i].color.b, 1f);
        }

        LevelEditorManager.Instance.selectedTileToPlace = _selectedTileIndex == -1 ? null : _tileResources[_selectedTileIndex];
    }
}
