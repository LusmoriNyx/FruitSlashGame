// LifeManager.cs
using System.Collections.Generic;
using UnityEngine;
using TMPro; // thêm dòng này

public class LifeManager : MonoBehaviour
{
    public static LifeManager Instance; // thêm instance

    [SerializeField] private List<GameObject> lives = new List<GameObject>();
    [SerializeField] private int lifeCount = 3;

    [Header("Score")]
    public TMP_Text scoreText; // kéo text vào inspector
    private int currentScore = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    public void UpdateLives()
    {
        if (lifeCount <= 0) return;
        lives[lifeCount - 1].gameObject.SetActive(false);
        lifeCount--;
        // TODO: nếu lifeCount == 0 thì game over
    }

    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreUI();
        Debug.Log("+ " + points + " điểm. Tổng: " + currentScore);
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text =  currentScore.ToString();
    }
}