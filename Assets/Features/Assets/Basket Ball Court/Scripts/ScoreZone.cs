using UnityEngine;
using UnityEngine.Events;

// Detects when a ball passes through the hoop and triggers scoring

public class ScoreZone : MonoBehaviour
{
    #region EVENTS
    public UnityEvent OnScore;
    #endregion

    #region SERIALIZED FIELDS
    [Header("VFX")]
    [SerializeField] private GameObject scoreEffect;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip scoreClip;
    [SerializeField] private float volume = 1f;
    #endregion

    #region UNITY METHODS
    private void OnTriggerEnter(Collider other)
    {
        BallController ball = other.GetComponent<BallController>();

        // Only score if this is a valid ball and it hasn't already scored
        if (ball == null || ball.HasScored()) return;

        HandleScore(ball);
    }
    #endregion

    #region PRIVATE METHODS

    /// <summary>
    /// Handles all scoring feedback and logic when a valid ball enters the zone
    /// </summary>
    private void HandleScore(BallController ball)
    {
        // Prevent this ball from scoring multiple times
        ball.MarkScored();

        PlayScoreAudio();
        SpawnScoreEffect();

        // Notify other systems (round controller, UI, etc.)
        OnScore?.Invoke();
    }

    /// <summary>
    /// Plays the scoring sound with slight variation
    /// </summary>
    private void PlayScoreAudio()
    {
        if (audioSource == null || scoreClip == null) return;

        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.PlayOneShot(scoreClip, volume);
    }

    /// <summary>
    /// Spawns the scoring particle effect
    /// </summary>
    private void SpawnScoreEffect()
    {
        if (scoreEffect == null) return;

        GameObject fx = Instantiate(scoreEffect, transform.position, Quaternion.identity);
        Destroy(fx, 2f);
    }

    #endregion
}