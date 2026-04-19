using UnityEngine;

public class HoopMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float startMoveSpeed = 0.4f;
    [SerializeField] private float speedIncrease = 0.1f;

    [Header("Bounds (MAX)")]
    [SerializeField] private float maxXRange = 0.5f;
    [SerializeField] private float maxYRange = 0.75f;

    [Header("Bounds (START)")]
    [SerializeField] private float startXRange = 0.1f;
    [SerializeField] private float startYRange = 0.15f;

    [Header("Growth")]
    [SerializeField] private float rangeIncrease = 0.1f;

    private Vector3 startPos;

    private float movementTime = 0f;
    private float moveSpeed = 0f;

    private float currentXRange;
    private float currentYRange;

    private bool isMoving = false;

    void Awake()
    {
        startPos = transform.position;
        transform.position = startPos;

        // Start small
        currentXRange = startXRange;
        currentYRange = startYRange;
    }

    void Update()
    {
        if (!isMoving) return;

        movementTime += Time.deltaTime * moveSpeed;

        float x = Mathf.Sin(movementTime) * currentXRange;

        float yRaw = Mathf.Sin(movementTime * 0.8f) * currentYRange;
        float y = Mathf.Max(0f, yRaw);

        transform.position = new Vector3(
            startPos.x + x,
            startPos.y + y,
            startPos.z
        );
    }

    public void IncreaseDifficulty()
    {
        // First activation
        if (!isMoving)
        {
            isMoving = true;
            movementTime = 0f;
            moveSpeed = startMoveSpeed;
            return;
        }

        // Increase speed
        moveSpeed += speedIncrease;

        // Grow movement range (clamped to max)
        currentXRange = Mathf.Min(currentXRange + rangeIncrease, maxXRange);
        currentYRange = Mathf.Min(currentYRange + rangeIncrease, maxYRange);
    }

    public void ResetHoop()
    {
        isMoving = false;
        moveSpeed = 0f;
        movementTime = 0f;

        // Reset to small starting movement
        currentXRange = startXRange;
        currentYRange = startYRange;

        transform.position = startPos;
    }
}