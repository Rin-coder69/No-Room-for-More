using TMPro;
using UnityEngine;

public class ScoreManagerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    void Update()
    {
        if (ScoreManager.Instance != null)
        {
            scoreText.text = "Score: " + ScoreManager.Instance.GetScore();
        }
    }
}
