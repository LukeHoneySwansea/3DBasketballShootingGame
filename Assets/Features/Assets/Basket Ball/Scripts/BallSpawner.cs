using UnityEngine;

// Handles spawning balls with a max limit

public class BallSpawner : MonoBehaviour
{
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Settings")]
    [SerializeField] private int maxBalls = 5;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip appearClip;

    private int activeBallCount = 0;
    private bool isSpawningEnabled = false;

    public void StartSpawning()
    {
        isSpawningEnabled = true;
        EnsureBallExists();
    }

    public void StopSpawning()
    {
        isSpawningEnabled = false;
    }

    public void EnsureBallExists()
    {
        if (!isSpawningEnabled) return;

        if (activeBallCount <= 0)
        {
            SpawnBall();
        }
    }

    public void SpawnBall()
    {
        if (!isSpawningEnabled) return;

        if (activeBallCount >= maxBalls)
        {
            return;
        }

        if (ballPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("BallSpawner: Missing prefab or spawn point.");
            return;
        }

        if (audioSource != null && appearClip != null)
        {
            audioSource.PlayOneShot(appearClip);
        }

        GameObject ballObj = Instantiate(ballPrefab, spawnPoint.position, spawnPoint.rotation);

        BallController ball = ballObj.GetComponent<BallController>();

        if (ball != null)
        {
            ball.SetSpawner(this);
            activeBallCount++;
        }
    }

    public void OnBallDestroyed()
    {
        activeBallCount--;

        if (activeBallCount < 0)
            activeBallCount = 0;

        // Only respawn if allowed AND under max
        if (isSpawningEnabled && activeBallCount == 0)
        {
            SpawnBall();
        }
    }
    public void ClearAllBalls()
    {
        var balls = FindObjectsByType<BallController>(FindObjectsSortMode.None);

        foreach (var ball in balls)
        {
            Destroy(ball.gameObject);
        }

        activeBallCount = 0;
    }
}