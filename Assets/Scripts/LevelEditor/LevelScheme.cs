
using System;
using System.Collections.Generic;
using MessagePack;

/// <summary>
/// MessagePack serializable class for storing level data.
/// </summary>
[MessagePackObject]
public class Level
{
  [Key(0)] public string name;
  [Key(1)] public string[] tiles;
  [Key(2)] public Dictionary<Tuple<int, int, int>, int> tilemapData;
}