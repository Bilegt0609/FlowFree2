using UnityEngine;

public class Cells : MonoBehaviour
{
    public int Cellx;
    public int Celly;
    public Color CellColor;
    public bool Dot = false;
    public bool isFilled = false;

    private SpriteRenderer cellSr;
    private SpriteRenderer circleSr;

    void Awake()
    {
        Transform dotChild = transform.Find("Dot");
        if (dotChild != null)
        {
            circleSr = dotChild.GetComponent<SpriteRenderer>();
        }
        cellSr = GetComponent<SpriteRenderer>();
    }

    public void SetColor(Color color)
    {
        isFilled = true;
        CellColor = color;
        cellSr.color = color;

        if (Dot)
        {
            if (circleSr != null) circleSr.color = color;
        }
    }

    public void OriginalColor()
    {
        isFilled = false;
        CellColor = Color.white;
        cellSr.color = Color.white;

        if (!Dot && circleSr != null)
        {
            circleSr.color = Color.black;
        }
    }
}