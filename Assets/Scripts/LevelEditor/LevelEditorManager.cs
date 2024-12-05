using System;
using System.Collections.Generic;
using System.IO;
using MessagePack;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Manages the level editor functionality, including tile placement, saving, and loading levels.
/// <summary>
public class LevelEditorManager : MonoBehaviour
{
    // Singleton instance of the LevelEditorManager.
    public static LevelEditorManager Instance { get; private set; }

    // Indicates whether tile editing is enabled.
    public bool tileEditingEnabled = true;

    // The current level being edited.
    public Level level = new()
    {
        name = "New Level",
        tiles = new string[0],
        tilemapData = new Dictionary<Tuple<int, int, int>, int>()
    };

    // The tile currently selected to be placed.
    [HideInInspector] public Tile selectedTileToPlace;

    // The ghost tilemap used for previewing the selected tile.
    [Header("Tilemaps")]
    [SerializeField] private Tilemap _ghostTilemap;

    // The array of tilemaps used in the level editor.
    [SerializeField] private Tilemap[] _tilemaps;

    // The prefab for the tile mask.
    [SerializeField] private GameObject _tileMaskPrefab;

    // The grid component used for tile placement.
    private Grid _grid;

    // The instance of the tile mask.
    private GameObject _tileMaskInstance;

    // The path to save user levels.
    private readonly string _userLevelsPath = "UserData/Levels/";

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance != null && Instance != this) Destroy(Instance.gameObject);
        Instance = this;

        if (_tileMaskPrefab == null) Debug.LogError("Tile mask prefab not set");
    }

    private void Start()
    {
        _grid = FindFirstObjectByType<Grid>();
        if (_grid == null) Debug.LogError("Grid not found");

        // Temporary load level
        Load("New Level");
    }

    private void Update()
    {
        // Save level
        if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl)) Save();

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = _grid.WorldToCell(mousePos);

        // Place tile when selected tile is not null
        _ghostTilemap.ClearAllTiles();
        if (selectedTileToPlace != null) _ghostTilemap.SetTile(cellPos, selectedTileToPlace);

        // Tile placement input controller
        if (tileEditingEnabled)
        {
            if (selectedTileToPlace != null && Input.GetMouseButton(0)) SetTile(selectedTileToPlace, 0, cellPos);
            else if (Input.GetMouseButton(1)) DeleteTile(0, cellPos);
        }

        // Position tile mask
        if (selectedTileToPlace != null)
        {
            if (_tileMaskInstance == null)
            {
                _tileMaskInstance = Instantiate(_tileMaskPrefab, _grid.CellToWorld(cellPos) + _ghostTilemap.tileAnchor, Quaternion.identity);
                _tileMaskInstance.transform.SetParent(_grid.transform);
            }
            else
            {
                _tileMaskInstance.transform.position = _grid.CellToWorld(cellPos) + _ghostTilemap.tileAnchor;
            }
        }
        else if (_tileMaskInstance != null) Destroy(_tileMaskInstance);
    }

    /// <summary>
    /// Saves the current level to a MessagePack file saved in UserData/Levels/.
    /// </summary>
    public void Save()
    {
        int tileIndex;
        List<string> tiles = new();
        level.tilemapData = new();

        // Get tilemap data
        for (int i = 0; i < _tilemaps.Length; i++)
        {
            foreach (var pos in _tilemaps[i].cellBounds.allPositionsWithin)
            {
                var tile = _tilemaps[i].GetTile(new Vector3Int(pos.x, pos.y, 0));
                if (tile != null)
                {
                    // Store each unique tile to a tile list, in which the index will be used to store tilemap data.
                    if (!tiles.Contains(tile.name)) tiles.Add(tile.name);

                    tileIndex = tiles.IndexOf(tile.name);
                    level.tilemapData.Add(new Tuple<int, int, int>(i, pos.x, pos.y), tileIndex);
                }
            }
        }
        level.tiles = tiles.ToArray();

        // Serialize to message pack and save to file
        byte[] bytes = MessagePackSerializer.Serialize(level);
        Directory.CreateDirectory(_userLevelsPath);
        File.WriteAllBytes(Path.Join(_userLevelsPath, $"{level.name}.level"), bytes);
    }

    /// <summary>
    /// Loads a level from a MessagePack file.
    /// </summary>
    /// <param name="leveLFileToLoad">The name of the level file to load.</param>
    public void Load(string leveLFileToLoad)
    {
        // Load level file
        byte[] bytes = File.ReadAllBytes(Path.Join(_userLevelsPath, $"{leveLFileToLoad}.level"));
        level = MessagePackSerializer.Deserialize<Level>(bytes);

        // Clear tilemaps
        foreach (var tilemap in _tilemaps) tilemap.ClearAllTiles();

        // Load tilemap data
        foreach (var tile in level.tilemapData)
        {
            if (_tilemaps.Length <= tile.Key.Item1 || level.tiles.Length <= tile.Value) continue;
            _tilemaps[tile.Key.Item1].SetTile(
                new Vector3Int(tile.Key.Item2, tile.Key.Item3, 0),
                Resources.Load<Tile>($"Tiles/{level.tiles[tile.Value]}")
            );
        }
    }

    /// <summary>
    /// Sets a tile at the specified position in the specified tilemap.
    /// </summary>
    /// <param name="tile">The tile to set.</param>
    /// <param name="tilemapIndex">The index of the tilemap.</param>
    /// <param name="position">The position to set the tile.</param>
    public void SetTile(Tile tile, int tilemapIndex, Vector3Int position) { _tilemaps[tilemapIndex].SetTile(position, tile); }

    /// <summary>
    /// Deletes a tile at the specified position in the specified tilemap.
    /// </summary>
    /// <param name="tilemapIndex">The index of the tilemap.</param>
    /// <param name="position">The position to delete the tile.</param>
    public void DeleteTile(int tilemapIndex, Vector3Int position) { _tilemaps[tilemapIndex].SetTile(position, null); }

    /// <summary>
    /// Enables or disables tile placement.
    /// </summary>
    /// <param name="enabled">True to enable tile placement, false to disable.</param>
    public void SetTilePlacement(bool enabled) { tileEditingEnabled = enabled; }
}
