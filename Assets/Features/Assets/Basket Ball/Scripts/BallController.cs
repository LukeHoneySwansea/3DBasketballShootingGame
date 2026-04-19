using UnityEngine;
using System.Collections;

// Controls the behaviour of the ball (pickup, throw, collisions, despawn)

public class BallController : MonoBehaviour
{
    #region SERIALIZED FIELDS

    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 25f;
    [SerializeField] private float rotateSpeed = 10f;

    [Header("Reset Settings")]
    [SerializeField] private float resetDelay = 2f;

    [Header("VFX")]
    [SerializeField] private GameObject despawnEffect;

    [Header("Spawn Protection")]
    [SerializeField] private float protectionTime = 0.5f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bounceClip;
    [SerializeField] private float minBounceVelocity = 0.1f;
    [SerializeField] private float bounceCooldown = 0.05f;

    #endregion

    #region PRIVATE FIELDS

    private Rigidbody _rb;

    private Transform _holdPoint;

    private BallSpawner _spawner;

    private float _lastBounceTime = 0f;

    private bool _isHeld = false;
    private bool _isResetting = false;
    private bool _isProtected = true;
    private bool _hasBeenThrown = false;
    private bool _hasScored = false;

    #endregion

    #region PUBLIC METHODS

    public void SetSpawner(BallSpawner ballSpawner)
    {
        _spawner = ballSpawner;
    }

    public void PickUp(Transform holdTransform)
    {
        _holdPoint = holdTransform;
        _isHeld = true;

        _rb.isKinematic = true;
        _rb.useGravity = false;
    }

    public void Release()
    {
        _isHeld = false;

        _rb.isKinematic = false;
        _rb.useGravity = true;

        _holdPoint = null;
    }

    public void Throw(Vector3 direction, float force)
    {
        _isHeld = false;
        _hasBeenThrown = true;

        _rb.isKinematic = false;
        _rb.useGravity = true;

        _holdPoint = null;

        _rb.linearVelocity = direction * force;

        // Add slight random spin for realism
        _rb.angularVelocity = Random.insideUnitSphere * 5f;
    }

    public bool CanBePickedUp()
    {
        return !_hasBeenThrown;
    }

    public bool HasScored()
    {
        return _hasScored;
    }

    public void MarkScored()
    {
        _hasScored = true;
    }

    #endregion

    #region UNITY METHODS

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        if (_rb == null)
        {
            Debug.LogError("BallController: Missing Rigidbody", this);
        }
    }

    private void Start()
    {
        // Temporary spawn protection (prevents instant despawn or collisions)
        Invoke(nameof(RemoveProtection), protectionTime);

        IgnoreOtherBallsTemporarily();
    }

    private void Update()
    {
        HandleFallDespawn();
        HandleFollow();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isHeld) return;

        HandleBounceAudio(collision);

        // Ignore scoring surfaces for despawn logic
        if (IsIgnoredCollision(collision)) return;

        // Prevent instant despawn right after spawn
        if (_isProtected) return;

        StartCoroutine(DelayedDespawn());
    }

    #endregion

    #region PRIVATE METHODS

    private void HandleFollow()
    {
        if (!_isHeld || _holdPoint == null) return;

        transform.position = Vector3.Lerp(
            transform.position,
            _holdPoint.position,
            followSpeed * Time.deltaTime
        );

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            _holdPoint.rotation,
            rotateSpeed * Time.deltaTime
        );
    }

    private void HandleFallDespawn()
    {
        if (!_isHeld && transform.position.y < -5f)
        {
            Despawn();
        }
    }

    private void HandleBounceAudio(Collision collision)
    {
        float impact = collision.relativeVelocity.magnitude;

        if (Time.time - _lastBounceTime < bounceCooldown) return;
        if (impact < minBounceVelocity) return;
        if (audioSource == null || bounceClip == null) return;

        audioSource.pitch = Random.Range(0.9f, 1.1f);

        float volume = Mathf.Clamp01(impact / 8f);
        audioSource.PlayOneShot(bounceClip, volume);

        _lastBounceTime = Time.time;
    }

    private bool IsIgnoredCollision(Collision collision)
    {
        return collision.gameObject.CompareTag("Hoop") ||
               collision.gameObject.CompareTag("Backboard") ||
               collision.gameObject.CompareTag("Tray");
    }

    private IEnumerator DelayedDespawn()
    {
        _isResetting = true;

        yield return new WaitForSeconds(resetDelay);

        if (!_isHeld)
        {
            Despawn();
        }

        _isResetting = false;
    }

    private void Despawn()
    {
        SpawnDespawnEffect();

        // Notify spawner so it can maintain ball count
        if (_spawner != null)
        {
            _spawner.OnBallDestroyed();
        }

        Destroy(gameObject);
    }

    private void SpawnDespawnEffect()
    {
        if (despawnEffect == null) return;

        GameObject fx = Instantiate(despawnEffect, transform.position, Quaternion.identity);
        Destroy(fx, 2f);
    }

    private void RemoveProtection()
    {
        _isProtected = false;
    }

    private void IgnoreOtherBallsTemporarily()
    {
        Collider myCol = GetComponent<Collider>();

        foreach (var other in FindObjectsOfType<BallController>())
        {
            if (other == this) continue;

            Collider otherCol = other.GetComponent<Collider>();

            if (otherCol != null && myCol != null)
            {
                Physics.IgnoreCollision(myCol, otherCol, true);
                StartCoroutine(ReenableCollision(myCol, otherCol));
            }
        }
    }

    private IEnumerator ReenableCollision(Collider a, Collider b)
    {
        yield return new WaitForSeconds(protectionTime);

        if (a != null && b != null)
        {
            Physics.IgnoreCollision(a, b, false);
        }
    }

    #endregion
}