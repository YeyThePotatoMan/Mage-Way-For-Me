using System;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// MessagePack serializable class for storing level data.
/// </summary>
[MessagePackObject]
public class Level
{
  public struct PropData
  {
    // The name of the entity. It is also used as an id.
    public string name;
    // The index of the entity in the prop list.
    public int propIndex;    
    // Values of the entity component's public variables. If it is float or int, it must be serializable.
    public string[] parameters;    
  }

  [Key(0)] public string name;
  // The list of tile paths in the resources folder.
  [Key(1)] public List<string> tiles;
  // The list of loaded tiles.
  [IgnoreMember] public List<Tile> tileResources;
  // The list of prop paths in the resources folder.
  [Key(2)] public List<string> props;
  // The list of loaded props.
  [IgnoreMember] public List<GameObject> propResources;
  // Collection of tiles with its key as a tuple of tilemap index, x, and y, and its value as the tile index.
  [Key(3)] public Dictionary<Tuple<int, int, int>, int> tilemapData;
  // Collection of entities with its key as its position.
  [Key(4)] public Dictionary<Vector2Int, PropData> propData;
  // Collection of prop instances with its key as its position.
  [IgnoreMember] public Dictionary<Vector2Int, GameObject> propInstances;
}