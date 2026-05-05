using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public float CellSize;
    public GameObject gridPrefab;
    public Levels allLevels;
    public int currentLevelIndex = 0;
    private float Offset;
    public UIManager uiManager;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        if (allLevels == null || allLevels.levels.Count == 0) return;
        if (currentLevelIndex >= allLevels.levels.Count) currentLevelIndex = 0;
        
        Level currentLevel = allLevels.levels[currentLevelIndex];
        int gridSize = currentLevel.GridSize;
        
        Offset = (gridSize - 1) * CellSize / 2f;

        GameObject cellsContainer = new GameObject("Cells");
        cellsContainer.transform.parent = transform;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 position = new Vector3(
                    x * CellSize - Offset,
                    y * CellSize - Offset,
                    0
                );
                GameObject newCell = Instantiate(gridPrefab, position, Quaternion.identity, cellsContainer.transform);
                newCell.name = "Cell_" + x + "_" + y;

                Cells cellScript = newCell.GetComponent<Cells>();
                if (cellScript != null)
                {
                    cellScript.Cellx = x;
                    cellScript.Celly = y;

                    DotData dot = GetDotAt(currentLevel, x, y);
                    if (dot != null)
                    {
                        cellScript.Dot = true;
                        cellScript.SetColor(dot.Dot_Color);
                    }
                    else
                    {
                        cellScript.OriginalColor();
                    }
                }
            }
        }
    }

    DotData GetDotAt(Level level, int x, int y)
    {
        foreach (DotData dot in level.Dots)
        {
            if (dot.Dot_X == x && dot.Dot_Y == y)
            {
                return dot;
            }
        }
        return null;
    }

    public bool CheckWin()
    {
        if (allLevels == null || allLevels.levels.Count == 0) return false;
        if (currentLevelIndex >= allLevels.levels.Count) return false;
        
        Level currentLevel = allLevels.levels[currentLevelIndex];

        HashSet<Color> uniqueColors = new HashSet<Color>();
        foreach (DotData dot in currentLevel.Dots)
        {
            uniqueColors.Add(dot.Dot_Color);
        }

        foreach (Color color in uniqueColors)
        {
            if (!DrawManager.IsColorConnected(color)) return false;
        }

        return true;
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex >= allLevels.levels.Count)
        {
            currentLevelIndex = 0;
            Debug.Log("Та бүх level-ийг давлаа!");
        }

        ReloadGrid();
    }

    public void ReplayCurrentLevel()
    {
        ReloadGrid();
    }

    void ReloadGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        DrawManager.ClearAllData();
        GenerateGrid();
        
        if (uiManager != null)
        {
            uiManager.HideWinUI();
        }
    }
}