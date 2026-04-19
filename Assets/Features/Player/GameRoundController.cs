using UnityEngine;
using UnityEngine.Events;

// Handles scoring, timer, and round progression

public class GameRoundController : MonoBehaviour
{
    [Header("Round Settings")]
    [SerializeField] private float roundTime = 10f;
    [SerializeField] private int baseTargetScore = 3;

    [Header("Difficulty Scaling")]
    [SerializeField] private int scoreIncreasePerRound = 2;

    [Header("References")]
    [SerializeField] private HoopMover hoopMover;

    [Header("Events")]
    public UnityEvent OnRoundStart;
    public UnityEvent OnRoundEnd;
    public UnityEvent OnGameOver;

    private float timer;
    private int currentScore = 0;
    private int currentRound = 1;
    private int targetScore;

    private bool isPlaying = false;
    private bool isTransitioning = false;

    public void StartRound()
    {
        // Reset transition lock
        isTransitioning = false;

        currentScore = 0;
        timer = roundTime;

        targetScore = baseTargetScore + (currentRound - 1) * scoreIncreasePerRound;

        isPlaying = true;

        OnRoundStart?.Invoke();

        // Only increase difficulty AFTER round 1
        if (currentRound > 1 && hoopMover != null)
        {
            hoopMover.IncreaseDifficulty();
        }

        Debug.Log($"Round {currentRound} started. Target: {targetScore}");
    }

    void Update()
    {
        if (!isPlaying) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            EndRound();
        }
    }

    public void AddScore()
    {
        if (!isPlaying || isTransitioning) return;

        currentScore++;

        Debug.Log($"Score: {currentScore}");

        // Early round completion
        if (currentScore >= targetScore)
        {
            isTransitioning = true;

            currentRound++;
            StartRound();
        }
    }

    void EndRound()
    {
        if (isTransitioning) return;

        isTransitioning = true;
        isPlaying = false;

        OnRoundEnd?.Invoke();

        if (currentScore >= targetScore)
        {
            currentRound++;
            StartRound();
        }
        else
        {
            Debug.Log("Game Over");
            OnGameOver?.Invoke();
        }
    }
    public void ResetGame()
    {
        isPlaying = false;
        isTransitioning = false;

        currentRound = 1;
        currentScore = 0;
        timer = roundTime;

        // Reset hoop too
        if (hoopMover != null)
        {
            hoopMover.ResetHoop();
        }
    }

    // ----------------------------
    // UI HELPERS (very useful later)
    // ----------------------------

    public float GetTimeRemaining()
    {
        return timer;
    }

    public int GetScore()
    {
        return currentScore;
    }

    public int GetTargetScore()
    {
        return targetScore;
    }

    public int GetRound()
    {
        return currentRound;
    }
}