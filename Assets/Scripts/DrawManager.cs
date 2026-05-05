using UnityEngine;
using System.Collections.Generic;

public class DrawManager : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Cells cellScript;

    public static bool isDrawing = false;
    public static Color DrawingColor;

    public float lineWidth = 0.3f;

    private static DrawManager lastCell;
    private static List<DrawManager> draggedCells = new List<DrawManager>();
    private static Dictionary<Color, List<DrawManager>> colorPaths = new Dictionary<Color, List<DrawManager>>();
    private static Dictionary<Color, LineRenderer> lineRenderers = new Dictionary<Color, LineRenderer>();

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        cellScript = GetComponent<Cells>();
    }

    void OnMouseDown()
    {
        if (cellScript.Dot)
        {
            StartNewPath();
        }
        else if (cellScript.isFilled)
        {
            ContinuePath();
        }
        else
        {
            isDrawing = false;
        }
    }

    void OnMouseEnter()
    {
        if (!isDrawing) return;
        if (lastCell == null) return;

        int diffX = Mathf.Abs(cellScript.Cellx - lastCell.cellScript.Cellx);
        int diffY = Mathf.Abs(cellScript.Celly - lastCell.cellScript.Celly);

        if (diffX + diffY != 1) return;
        if (cellScript.Dot && cellScript.CellColor != DrawingColor) return;

        if (cellScript.isFilled && cellScript.CellColor != DrawingColor)
        {
            ClearPath(cellScript.CellColor);
        }

        int index = draggedCells.IndexOf(this);

        if (index >= 0)
        {
            RemoveCellsAfter(index);
        }
        else
        {
            cellScript.SetColor(DrawingColor);
            draggedCells.Add(this);
        }

        lastCell = this;
        UpdateLine(DrawingColor, draggedCells);
    }

    void OnMouseUp()
    {
        isDrawing = false;
        lastCell = null;
        colorPaths[DrawingColor] = new List<DrawManager>(draggedCells);
        UpdateLine(DrawingColor, draggedCells);

        GridManager gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager != null && gridManager.CheckWin())
        {
            Debug.Log("Та ялаа! Congratulations");

            UIManager uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null)
            {
                uiManager.OnLevelComplete();
            }
        }
    }

    void StartNewPath()
    {
        isDrawing = true;
        DrawingColor = cellScript.CellColor;

        if (colorPaths.ContainsKey(DrawingColor))
        {
            foreach (DrawManager cell in colorPaths[DrawingColor])
            {
                if (!cell.cellScript.Dot)
                {
                    cell.cellScript.OriginalColor();
                }
            }
            colorPaths[DrawingColor].Clear();
        }

        draggedCells.Clear();
        draggedCells.Add(this);
        lastCell = this;
        UpdateLine(DrawingColor, draggedCells);
    }

    void ContinuePath()
    {
        if (!colorPaths.ContainsKey(cellScript.CellColor)) return;

        List<DrawManager> path = colorPaths[cellScript.CellColor];
        if (path.Count == 0) return;

        int index = path.IndexOf(this);
        if (index < 0) return;

        for (int i = path.Count - 1; i > index; i--)
        {
            if (!path[i].cellScript.Dot)
            {
                path[i].cellScript.OriginalColor();
            }
            path.RemoveAt(i);
        }

        isDrawing = true;
        DrawingColor = cellScript.CellColor;
        draggedCells = new List<DrawManager>(path);
        lastCell = this;
        UpdateLine(DrawingColor, draggedCells);
    }

    void RemoveCellsAfter(int index)
    {
        for (int i = draggedCells.Count - 1; i > index; i--)
        {
            if (!draggedCells[i].cellScript.Dot)
            {
                draggedCells[i].cellScript.OriginalColor();
            }
            draggedCells.RemoveAt(i);
        }
    }

    void ClearPath(Color color)
    {
        if (!colorPaths.ContainsKey(color)) return;

        foreach (DrawManager cell in colorPaths[color])
        {
            if (!cell.cellScript.Dot)
            {
                cell.cellScript.OriginalColor();
            }
        }

        colorPaths[color].Clear();

        if (lineRenderers.ContainsKey(color))
        {
            lineRenderers[color].positionCount = 0;
        }
    }

    void UpdateLine(Color color, List<DrawManager> path)
    {
        if (!lineRenderers.ContainsKey(color))
        {
            GameObject lineObj = new GameObject("Line_" + color);
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.sortingOrder = -1;
            lineRenderers[color] = lr;
        }

        LineRenderer line = lineRenderers[color];
        line.startColor = color;
        line.endColor = color;
        line.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            Vector3 pos = path[i].transform.position;
            pos.z = -2f;
            line.SetPosition(i, pos);
        }
    }

    public static bool IsColorConnected(Color color)
    {
        if (!colorPaths.ContainsKey(color)) return false;

        List<DrawManager> path = colorPaths[color];
        if (path.Count < 2) return false;

        return path[0].cellScript.Dot && path[path.Count - 1].cellScript.Dot;
    }
    public static void ClearAllData()
    {
        isDrawing = false;
        DrawingColor = Color.white;
        lastCell = null;
        draggedCells.Clear();
        colorPaths.Clear();

        foreach (var line in lineRenderers.Values)
        {
            if (line != null)
                Destroy(line.gameObject);
        }
        lineRenderers.Clear();
    }
}