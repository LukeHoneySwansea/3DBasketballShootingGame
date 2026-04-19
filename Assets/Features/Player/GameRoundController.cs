using UnityEngine;
using UnityEngine.Events;

// Handles scoring, timer, round progression, and win/lose conditions

public class GameRoundController : MonoBehaviour
{
    #region SERIALIZED FIELDS

    [Header("Round Settings")]
    [SerializeField] private float roundTime = 30f;
    [SerializeField] private int baseTargetScore = 3;
    [SerializeField] private int maxTargetScore = 10;

    [Header("Difficulty Scaling")]
    [SerializeField] private int scoreIncreasePerRound = 2;

    [Header("References")]
    [SerializeField] private HoopMover hoopMover;

    [Header("Events")]
    public UnityEvent OnRoundStart;
    public UnityEvent OnRoundEnd;
    public UnityEvent OnGameOver;

    #endregion

    #region PRIVATE FIELDS

    private float _timer;

    private int _currentScore = 0;
    private int _currentRound = 1;
    private int _targetScore;

    private bool _isPlaying = false;
    private bool _isTransitioning = false;

    #endregion

    #region ROUND FLOW

    public void StartRound()
    {
        _isTransitioning = false;

        _currentScore = 0;
        _timer = roundTime;

        CalculateTargetScore();

        _isPlaying = true;

        OnRoundStart?.Invoke();

        // Increase difficulty AFTER round 1
        if (_currentRound > 1 && hoopMover != null)
        {
            hoopMover.IncreaseDifficulty();
        }

        Debug.Log($"Round {_currentRound} started. Target: {_targetScore}");
    }

    private void EndRound()
    {
        if (_isTransitioning) return;

        _isTransitioning = true;
        _isPlaying = false;

        OnRoundEnd?.Invoke();

        if (_currentScore >= _targetScore)
        {
            AdvanceRound();
        }
        else
        {
            TriggerGameOver();
        }
    }

    private void AdvanceRound()
    {
        _currentRound++;
        StartRound();
    }

    private void TriggerGameOver()
    {
        Debug.Log("Game Over");
        OnGameOver?.Invoke();
    }

    #endregion

    #region UPDATE LOOP

    private void Update()
    {
        if (!_isPlaying) return;

        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            EndRound();
        }
    }

    #endregion

    #region SCORING

    public void AddScore()
    {
        if (!_isPlaying || _isTransitioning) return;

        _currentScore++;

        Debug.Log($"Score: {_currentScore}");

        // Early round completion
        if (_currentScore >= _targetScore)
        {
            _isTransitioning = true;
            AdvanceRound();
        }
    }

    #endregion

    #region GAME STATE

    public void ResetGame()
    {
        _isPlaying = false;
        _isTransitioning = false;

        _currentRound = 1;
        _currentScore = 0;
        _timer = roundTime;

        // Reset hoop movement
        if (hoopMover != null)
        {
            hoopMover.ResetHoop();
        }
    }

    #endregion

    #region CALCULATIONS

    private void CalculateTargetScore()
    {
        _targetScore = Mathf.Min(
            baseTargetScore + (_currentRound - 1) * scoreIncreasePerRound,
            maxTargetScore
        );
    }

    #endregion

    #region GETTERS (UI)

    public float GetTimeRemaining() => _timer;

    public int GetScore() => _currentScore;

    public int GetTargetScore() => _targetScore;

    public int GetRound() => _currentRound;

    #endregion
}