using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float timeLimit = 120f;
    [SerializeField] private TextMeshProUGUI timerText;

    private float timeRemaining;
    private bool timerRunning = true;

    void Start()
    {
        timeRemaining = timeLimit;
    }

    void Update()
    {
        if (timerRunning)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                timerRunning = false;
                TimerEnded();
            }

            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    void TimerEnded()
    {
        ScoreManager.Instance.SaveScore();
        SceneManager.LoadScene("MainMenuExample");
    }
}