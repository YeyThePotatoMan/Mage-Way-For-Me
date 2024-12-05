using System;
using System.Collections.Generic;
using System.IO;
using MessagePack;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

/// <summary>
/// Manages the level editor functionality, including tile placement, saving, and loading levels.
/// <summary>
public class LevelEditorManager : MonoBehaviour
{
    public struct EditAction
    {
        public enum ActionType { Tile }

        public ActionType actionType;
        // The tiles that was placed or deleted. Tuple consists of tilemap index, x, and y.
        public Dictionary<Tuple<int, int, int>, Tile> pastTiles;
        public Dictionary<Tuple<int, int, int>, Tile> newTiles;
    }
    public enum CursorState { Default, Brush, Delete }

    // Singleton instance of the LevelEditorManager.
    public static LevelEditorManager Instance { get; private set; }

    // The current level being edited.
    public Level level = new()
    {
        name = "New Level",
        tiles = new string[0],
        tilemapData = new Dictionary<Tuple<int, int, int>, int>()
    };
    // The history of edit actions.
    public Stack<EditAction> editHistory = new();
    // The history of redo actions.
    public Stack<EditAction> redoHistory = new();
    // The cursor state.
    public CursorState CurrentCursorState { get; private set; } = CursorState.Default;
    // The selected tilemap index.
    public int SelectedTilemapIndex { get; private set; } = 0;

    [Header("Events")]
    // The event invoked when an edit action is recorded.
    public UnityEvent EditEvent = new();
    // The event invoked when the cursor state is changed.
    public UnityEvent<CursorState> CursorChangeEvent = new();
    // The event invoked when the tilemap is changed.
    public UnityEvent<int> TilemapChangeEvent = new();

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
    // Indicates whether the user is filling tiles.
    private bool _isFilling = false;
    // Indicates whether the user is deleting tiles.
    private bool _isDeleting = false;
    // The start cell position for filling or deleting tiles.
    private Vector3Int _fillStartCellPos;

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
        int startX = _fillStartCellPos.x > cellPos.x ? cellPos.x : _fillStartCellPos.x;
        int startY = _fillStartCellPos.y > cellPos.y ? cellPos.y : _fillStartCellPos.y;
        int endX = _fillStartCellPos.x > cellPos.x ? _fillStartCellPos.x : cellPos.x;
        int endY = _fillStartCellPos.y > cellPos.y ? _fillStartCellPos.y : cellPos.y;
        if (selectedTileToPlace != null)
        {
            if (_isFilling)
            {
                for (int x = startX; x <= endX; x++)
                    for (int y = startY; y <= endY; y++)
                        _ghostTilemap.SetTile(new Vector3Int(x, y, 0), selectedTileToPlace);
            }
            else
            {
                _ghostTilemap.SetTile(cellPos, selectedTileToPlace);
            }
        }

        // Tile placement input controller        
        if (CurrentCursorState == CursorState.Brush || CurrentCursorState == CursorState.Delete)
        {
            if (selectedTileToPlace != null && !_isDeleting)
            {
                if (!_isFilling && Input.GetMouseButtonDown(0))
                {
                    _isFilling = true;
                    _fillStartCellPos = cellPos;
                }
                else if (_isFilling && Input.GetMouseButtonUp(0))
                {
                    _isFilling = false;
                    SetTileRange(selectedTileToPlace, 0, _fillStartCellPos, cellPos);
                }
            }
            if (!_isFilling)
            {
                if (!_isDeleting && Input.GetMouseButtonDown(1))
                {
                    _isDeleting = true;
                    ChangeCursorState(CursorState.Delete);
                    _fillStartCellPos = cellPos;
                }
                else if (_isDeleting && Input.GetMouseButtonUp(1))
                {
                    _isDeleting = false;
                    ChangeCursorState(CursorState.Brush);
                    SetTileRange(null, 0, _fillStartCellPos, cellPos);
                }
            }
        }


        // Undo and Redo
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.Z)) Undo();
            if (Input.GetKeyDown(KeyCode.Y)) Redo();
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
                if (_isFilling)
                {
                    _tileMaskInstance.transform.localScale = new Vector3(endX - startX + 1, endY - startY + 1, 1);
                    _tileMaskInstance.transform.position = _grid.CellToWorld(new Vector3Int(startX, startY, 0))
                        + _ghostTilemap.tileAnchor
                        + new Vector3((endX - startX) / 2f, (endY - startY) / 2f, 0);
                }
                else
                {
                    _tileMaskInstance.transform.localScale = Vector3.one;
                    _tileMaskInstance.transform.position = _grid.CellToWorld(cellPos) + _ghostTilemap.tileAnchor;
                }
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
    /// Sets a tile at the specified range in the specified tilemap.
    /// </summary>
    /// <param name="tile">The tile to set.</param>
    /// <param name="tilemapIndex">The index of the tilemap.</param>
    /// <param name="startPosition">The start position to set the tile.</param>
    /// <param name="endPosition">The end position to set the tile.</param>
    public void SetTileRange(Tile tile, int tilemapIndex, Vector3Int startPosition, Vector3Int endPosition)
    {
        // Add edit action to history
        EditAction editAction = new()
        {
            actionType = EditAction.ActionType.Tile,
            pastTiles = new(),
            newTiles = new()
        };
        bool didSomethingChanged = false;

        if (startPosition.x > endPosition.x) (endPosition.x, startPosition.x) = (startPosition.x, endPosition.x);
        if (startPosition.y > endPosition.y) (endPosition.y, startPosition.y) = (startPosition.y, endPosition.y);

        for (int x = startPosition.x; x <= endPosition.x; x++)
            for (int y = startPosition.y; y <= endPosition.y; y++)
            {
                // Store the past tile
                Tile pastTile = _tilemaps[tilemapIndex].GetTile<Tile>(new Vector3Int(x, y, 0));
                editAction.pastTiles.Add(new Tuple<int, int, int>(tilemapIndex, x, y), pastTile);

                if (pastTile != tile)
                {
                    didSomethingChanged = true;
                    _tilemaps[tilemapIndex].SetTile(new Vector3Int(x, y, 0), tile);
                }
                editAction.newTiles.Add(new Tuple<int, int, int>(tilemapIndex, x, y), tile);
            }

        if (didSomethingChanged) RecordEdit(editAction);
    }

    /// <summary>
    /// Changes the cursor state.
    /// </summary>
    /// <param name="cursorState">The cursor state to change to.</param>
    public void ChangeCursorState(CursorState cursorState)
    {
        if (CurrentCursorState == cursorState) return;
        CurrentCursorState = cursorState;
        CursorChangeEvent.Invoke(cursorState);
    }

    /// <summary>
    /// Changes the selected tilemap.
    /// </summary>
    /// <param name="index">Index of the tilemap.</param>
    public void ChangeTilemap(int index)
    {
        if (SelectedTilemapIndex == index) return;
        SelectedTilemapIndex = index;
        TilemapChangeEvent.Invoke(index);
    }

    /// <summary>
    /// Records an edit action to the edit history. Clears the redo history.
    /// </summary>
    /// <param name="editAction">The edit action to record.</param>
    public void RecordEdit(EditAction editAction)
    {
        editHistory.Push(editAction);
        if (redoHistory.Count > 0) redoHistory.Clear();
        EditEvent.Invoke();
    }

    /// <summary>
    /// Undoes the last action.
    /// </summary>
    public void Undo()
    {
        if (editHistory.Count == 0) return;

        EditAction editAction = editHistory.Pop();
        redoHistory.Push(editAction);

        switch (editAction.actionType)
        {
            case EditAction.ActionType.Tile:
                foreach (var tile in editAction.pastTiles)
                    _tilemaps[tile.Key.Item1].SetTile(new Vector3Int(tile.Key.Item2, tile.Key.Item3, 0), tile.Value);
                break;
            default:
                Debug.LogError("Invalid edit action type");
                break;
        }

        EditEvent.Invoke();
    }

    /// <summary>
    /// Redoes the last undone action.
    /// </summary>
    public void Redo()
    {
        if (redoHistory.Count == 0) return;

        EditAction editAction = redoHistory.Pop();
        editHistory.Push(editAction);

        switch (editAction.actionType)
        {
            case EditAction.ActionType.Tile:
                foreach (var tile in editAction.newTiles)
                    _tilemaps[tile.Key.Item1].SetTile(new Vector3Int(tile.Key.Item2, tile.Key.Item3, 0), tile.Value);
                break;
            default:
                Debug.LogError("Invalid edit action type");
                break;
        }

        EditEvent.Invoke();
    }
}
