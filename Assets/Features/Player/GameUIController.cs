using UnityEngine;
using TMPro;
using System.Collections;

// Handles gameplay UI + round flow timing

public class GameUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameRoundController roundController;
    [SerializeField] private BallSpawner ballSpawner;

    [Header("Main UI")]
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text scoreText;

    [Header("Overlay Text")]
    [SerializeField] private TMP_Text roundPopupText;
    [SerializeField] private TMP_Text gameOverText;

    void Start()
    {
        if (roundPopupText != null)
            roundPopupText.gameObject.SetActive(false);

        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (roundController == null) return;

        UpdateUI();
    }

    void UpdateUI()
    {
        // Round
        if (roundText != null)
        {
            roundText.text = $"Round: {roundController.GetRound()}";
        }

        // Timer
        if (timerText != null)
        {
            float time = Mathf.Max(0f, roundController.GetTimeRemaining());
            timerText.text = $"Time: {Mathf.CeilToInt(time)}";
        }

        // Score
        if (scoreText != null)
        {
            scoreText.text = $"{roundController.GetScore()} / {roundController.GetTargetScore()}";
        }
    }

    // =========================
    // ROUND FLOW
    // =========================

    public void HandleRoundStart()
    {
        StartCoroutine(RoundStartSequence());
    }

    IEnumerator RoundStartSequence()
    {
        // Stop gameplay
        if (ballSpawner != null)
            ballSpawner.StopSpawning();

        // 🧹 CLEAR ALL BALLS
        if (ballSpawner != null)
            ballSpawner.ClearAllBalls();

        // Show ROUND text
        if (roundPopupText != null)
        {
            roundPopupText.text = $"ROUND {roundController.GetRound()}";
            roundPopupText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        // Hide text
        if (roundPopupText != null)
            roundPopupText.gameObject.SetActive(false);

        // Resume gameplay
        if (ballSpawner != null)
            ballSpawner.StartSpawning();
    }

    // =========================
    // GAME OVER
    // =========================

    public void HandleGameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    IEnumerator GameOverSequence()
    {
        // Stop gameplay
        if (ballSpawner != null)
            ballSpawner.StopSpawning();

        // Stop gameplay
        if (ballSpawner != null)
        {
            ballSpawner.StopSpawning();
            ballSpawner.ClearAllBalls();
        }

        // Show GAME OVER
        if (gameOverText != null)
        {
            gameOverText.text = "GAME OVER";
            gameOverText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(3f);

        // Hide text
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);

        // Return to menu
        FindObjectOfType<GameStateController>().ReturnToMenu();
    }
}