using UnityEngine;

// Controls the behavior of the ball when picked up, released, thrown, or reset

public class BallController : MonoBehaviour
{
    private Rigidbody rb;

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

    private float lastBounceTime = 0f;

    private bool isHeld = false;
    private Transform holdPoint;
    private bool isResetting = false;
    private bool isProtected = true;

    // prevents re-pickup after throw
    private bool hasBeenThrown = false;

    private BallSpawner spawner;
    private bool hasScored = false;

    public bool HasScored()
    {
        return hasScored;
    }

    public void MarkScored()
    {
        hasScored = true;
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Missing Rigidbody");
        }
    }

    void Start()
    {
        Invoke(nameof(RemoveProtection), protectionTime);

        Collider myCol = GetComponent<Collider>();

        foreach (var other in FindObjectsOfType<BallController>())
        {
            if (other != this)
            {
                Collider otherCol = other.GetComponent<Collider>();

                if (otherCol != null && myCol != null)
                {
                    Physics.IgnoreCollision(myCol, otherCol, true);
                    StartCoroutine(ReenableCollision(myCol, otherCol));
                }
            }
        }
    }

    void RemoveProtection()
    {
        isProtected = false;
    }

    System.Collections.IEnumerator ReenableCollision(Collider a, Collider b)
    {
        yield return new WaitForSeconds(protectionTime);

        if (a != null && b != null)
        {
            Physics.IgnoreCollision(a, b, false);
        }
    }

    public void SetSpawner(BallSpawner ballSpawner)
    {
        spawner = ballSpawner;
    }

    public void PickUp(Transform holdTransform)
    {
        holdPoint = holdTransform;
        isHeld = true;

        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void Release()
    {
        isHeld = false;

        rb.isKinematic = false;
        rb.useGravity = true;

        holdPoint = null;
    }

    public void Throw(Vector3 direction, float force)
    {
        isHeld = false;

        hasBeenThrown = true;

        rb.isKinematic = false;
        rb.useGravity = true;

        holdPoint = null;

        rb.linearVelocity = direction * force;
        rb.angularVelocity = Random.insideUnitSphere * 5f;
    }

    public bool CanBePickedUp()
    {
        return !hasBeenThrown;
    }

    void Update()
    {
        if (!isHeld && transform.position.y < -5f)
        {
            Despawn();
        }

        if (!isHeld || holdPoint == null) return;

        transform.position = Vector3.Lerp(
            transform.position,
            holdPoint.position,
            followSpeed * Time.deltaTime
        );

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            holdPoint.rotation,
            rotateSpeed * Time.deltaTime
        );
    }

    void OnCollisionEnter(Collision collision)
    {
        // Only block if being held (NOT reset / protected)
        if (isHeld) return;

        // BOUNCE AUDIO (always allowed during reset)
        float impact = collision.relativeVelocity.magnitude;

        if (Time.time - lastBounceTime > bounceCooldown)
        {
            if (impact > minBounceVelocity)
            {
                if (audioSource != null && bounceClip != null)
                {
                    audioSource.pitch = Random.Range(0.9f, 1.1f);

                    float volume = Mathf.Clamp01(impact / 8f);
                    audioSource.PlayOneShot(bounceClip, volume);

                    lastBounceTime = Time.time;
                }
            }
        }

        // Ignore hoop/backboard for despawn
        if (collision.gameObject.CompareTag("Hoop") ||
            collision.gameObject.CompareTag("Backboard") ||
            collision.gameObject.CompareTag("Tray"))
        {
            return;
        }

        // Only block despawn during protection (NOT audio)
        if (isProtected) return;

        StartCoroutine(DelayedDespawn());
    }

    System.Collections.IEnumerator DelayedDespawn()
    {
        isResetting = true;

        yield return new WaitForSeconds(resetDelay);

        if (!isHeld)
        {
            Despawn();
        }

        isResetting = false;
    }

    void Despawn()
    {
        if (despawnEffect != null)
        {
            GameObject fx = Instantiate(despawnEffect, transform.position, Quaternion.identity);
            Destroy(fx, 2f);
        }

        if (spawner != null)
        {
            spawner.OnBallDestroyed();
        }

        Destroy(gameObject);
    }
}