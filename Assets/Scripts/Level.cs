using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Level
{
    public int LevelNumber;
    public int GridSize;
    public int NumberOfColors;
    public List<DotData> Dots;
}