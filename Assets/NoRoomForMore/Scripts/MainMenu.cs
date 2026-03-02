using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    void Start()
    {
        // display last score
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);
        scoreText.text = "Last Score: " + lastScore;
    }

    public void PlayGame()
    {
        // reset score and load game scene
        PlayerPrefs.SetInt("LastScore", 0);
        SceneManager.LoadScene("Game");
    }
}