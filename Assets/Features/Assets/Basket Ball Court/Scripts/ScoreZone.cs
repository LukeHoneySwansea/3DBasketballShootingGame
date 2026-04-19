using UnityEngine;
using UnityEngine.Events;

// Detects ball passing through hoop

public class ScoreZone : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent OnScore;

    [Header("VFX")]
    [SerializeField] private GameObject scoreEffect;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip scoreClip;
    [SerializeField] private float volume = 1f;

    private void OnTriggerEnter(Collider other)
    {
        BallController ball = other.GetComponent<BallController>();

        // ✅ Ensure it's a ball AND hasn't already scored
        if (ball != null && !ball.HasScored())
        {
            // Mark as scored (prevents duplicate triggers)
            ball.MarkScored();

            // 🔊 Play sound
            if (audioSource != null && scoreClip != null)
            {
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.PlayOneShot(scoreClip, volume);
            }

            // ✨ Spawn VFX
            if (scoreEffect != null)
            {
                GameObject fx = Instantiate(
                    scoreEffect,
                    transform.position,
                    Quaternion.identity
                );

                Destroy(fx, 2f);
            }

            // 🎯 Trigger scoring logic
            OnScore?.Invoke();
        }
    }
}