using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int score = 0;

    void Awake()
    {
        // make sure only one ScoreManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score: " + score);
    }

    public int GetScore()
    {
        return score;
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt("LastScore", score);
        PlayerPrefs.Save();
    }

    public void ResetScore()
    {
        score = 0;
    }
}