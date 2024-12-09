using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    private void Start()
    {
        GameStateManager.Instance.OnPointsAdded += UpdateScore;
    }

    private void UpdateScore(int amount)
    {
        text.text = "Score: " + amount;
    }
}
