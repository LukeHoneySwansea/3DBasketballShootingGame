using UnityEngine;

// Handles spawning balls with a max limit and safe lifecycle management

public class BallSpawner : MonoBehaviour
{
    #region SERIALIZED FIELDS

    [Header("Spawn Settings")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int maxBalls = 5;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip appearClip;

    #endregion

    #region PRIVATE FIELDS

    private int _activeBallCount = 0;
    private bool _isSpawningEnabled = false;

    #endregion

    #region SPAWN CONTROL

    public void StartSpawning()
    {
        _isSpawningEnabled = true;
        EnsureBallExists();
    }

    public void StopSpawning()
    {
        _isSpawningEnabled = false;
    }

    #endregion

    #region SPAWNING

    public void EnsureBallExists()
    {
        if (!_isSpawningEnabled) return;

        // Guarantee at least one playable ball
        if (_activeBallCount <= 0)
        {
            SpawnBall();
        }
    }

    public void SpawnBall()
    {
        if (!_isSpawningEnabled) return;

        // Respect max ball limit
        if (_activeBallCount >= maxBalls) return;

        if (ballPrefab == null || spawnPoint == null)
        {
            Debug.LogWarning("BallSpawner: Missing prefab or spawn point.");
            return;
        }

        PlaySpawnAudio();

        GameObject ballObj = Instantiate(
            ballPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        BallController ball = ballObj.GetComponent<BallController>();

        if (ball != null)
        {
            ball.SetSpawner(this);
            _activeBallCount++;
        }
    }

    private void PlaySpawnAudio()
    {
        if (audioSource != null && appearClip != null)
        {
            audioSource.PlayOneShot(appearClip);
        }
    }

    #endregion

    #region BALL LIFECYCLE

    public void OnBallDestroyed()
    {
        _activeBallCount--;

        if (_activeBallCount < 0)
            _activeBallCount = 0;

        // Safety: ensure at least one ball exists while playing
        if (_isSpawningEnabled && _activeBallCount == 0)
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

        _activeBallCount = 0;
    }

    #endregion
}