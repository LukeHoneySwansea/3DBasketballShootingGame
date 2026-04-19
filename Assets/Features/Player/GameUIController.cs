using UnityEngine;
using TMPro;
using System.Collections;

// Handles gameplay UI + round flow (start transitions + game over)

public class GameUIController : MonoBehaviour
{
    #region SERIALIZED FIELDS

    [Header("References")]
    [SerializeField] private GameRoundController roundController;
    [SerializeField] private BallSpawner ballSpawner;
    [SerializeField] private GameStateController gameStateController;

    [Header("Main UI")]
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text scoreText;

    [Header("Overlay UI")]
    [SerializeField] private TMP_Text roundPopupText;
    [SerializeField] private TMP_Text gameOverText;

    #endregion

    #region UNITY METHODS

    private void Start()
    {
        SetOverlayActive(roundPopupText, false);
        SetOverlayActive(gameOverText, false);
    }

    private void Update()
    {
        if (roundController == null) return;

        UpdateUI();
    }

    #endregion

    #region UI UPDATE

    private void UpdateUI()
    {
        UpdateRoundText();
        UpdateTimerText();
        UpdateScoreText();
    }

    private void UpdateRoundText()
    {
        if (roundText == null) return;

        roundText.text = $"Round: {roundController.GetRound()}";
    }

    private void UpdateTimerText()
    {
        if (timerText == null) return;

        float time = Mathf.Max(0f, roundController.GetTimeRemaining());
        timerText.text = $"Time: {Mathf.CeilToInt(time)}";
    }

    private void UpdateScoreText()
    {
        if (scoreText == null) return;

        scoreText.text = $"{roundController.GetScore()} / {roundController.GetTargetScore()}";
    }

    #endregion

    #region ROUND FLOW

    public void HandleRoundStart()
    {
        StartCoroutine(RoundStartSequence());
    }

    private IEnumerator RoundStartSequence()
    {
        // Stop gameplay + clean scene
        StopGameplay();

        // Show ROUND popup
        if (roundPopupText != null)
        {
            roundPopupText.text = $"ROUND {roundController.GetRound()}";
            roundPopupText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        // Hide popup
        SetOverlayActive(roundPopupText, false);

        // Resume gameplay
        if (ballSpawner != null)
        {
            ballSpawner.StartSpawning();
        }
    }

    #endregion

    #region GAME OVER

    public void HandleGameOver()
    {
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        // Stop gameplay + clean scene
        StopGameplay();

        // Show GAME OVER
        if (gameOverText != null)
        {
            gameOverText.text = "GAME OVER";
            gameOverText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(3f);

        // Hide text
        SetOverlayActive(gameOverText, false);

        // Return to menu (no FindObjectOfType anymore)
        if (gameStateController != null)
        {
            gameStateController.ReturnToMenu();
        }
    }

    #endregion

    #region HELPERS

    private void StopGameplay()
    {
        if (ballSpawner == null) return;

        ballSpawner.StopSpawning();
        ballSpawner.ClearAllBalls();
    }

    private void SetOverlayActive(TMP_Text text, bool state)
    {
        if (text != null)
        {
            text.gameObject.SetActive(state);
        }
    }

    #endregion
}