using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject winText;
    public GameObject nextLevelButton;
    public Button replayButton;

    private GridManager gridManager;

    void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();

        replayButton.onClick.AddListener(OnReplayClicked);

        winText.SetActive(false);
        nextLevelButton.SetActive(false);
    }

    public void OnLevelComplete()
    {
        winText.SetActive(true);
        nextLevelButton.SetActive(true);
    }

    public void OnNextLevelClicked()
    {
        if (gridManager != null)
        {
            gridManager.LoadNextLevel();
        }

        winText.SetActive(false);
        nextLevelButton.SetActive(false);
    }

    public void OnReplayClicked()
    {
        if (gridManager != null)
        {
            gridManager.ReplayCurrentLevel();
        }

        winText.SetActive(false);
        nextLevelButton.SetActive(false);
    }
    public void HideWinUI()
{
    winText.SetActive(false);
    nextLevelButton.SetActive(false);
}
}