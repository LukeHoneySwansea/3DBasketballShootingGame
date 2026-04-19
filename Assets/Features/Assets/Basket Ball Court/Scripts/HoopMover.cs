using UnityEngine;

// Controls hoop movement and difficulty scaling over rounds

public class HoopMover : MonoBehaviour
{
    #region SERIALIZED FIELDS

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

    #endregion

    #region PRIVATE FIELDS

    private Vector3 _startPos;

    private float _movementTime = 0f;
    private float _moveSpeed = 0f;

    private float _currentXRange;
    private float _currentYRange;

    private bool _isMoving = false;

    #endregion

    #region UNITY METHODS

    private void Awake()
    {
        _startPos = transform.position;

        // Ensure exact starting position (prevents drift from editor tweaks)
        transform.position = _startPos;

        // Initialise with small movement range (easy early rounds)
        _currentXRange = startXRange;
        _currentYRange = startYRange;
    }

    private void Update()
    {
        if (!_isMoving) return;

        UpdateMovement();
    }

    #endregion

    #region PUBLIC METHODS

    /// <summary>
    /// Increases hoop difficulty by enabling movement and scaling speed/range
    /// </summary>
    public void IncreaseDifficulty()
    {
        // First call → enable movement
        if (!_isMoving)
        {
            _isMoving = true;
            _movementTime = 0f;
            _moveSpeed = startMoveSpeed;
            return;
        }

        // Increase movement speed
        _moveSpeed += speedIncrease;

        // Gradually expand movement range (clamped to max values)
        _currentXRange = Mathf.Min(_currentXRange + rangeIncrease, maxXRange);
        _currentYRange = Mathf.Min(_currentYRange + rangeIncrease, maxYRange);
    }

    /// <summary>
    /// Resets hoop to its initial state (used when restarting the game)
    /// </summary>
    public void ResetHoop()
    {
        _isMoving = false;
        _moveSpeed = 0f;
        _movementTime = 0f;

        // Reset movement bounds
        _currentXRange = startXRange;
        _currentYRange = startYRange;

        transform.position = _startPos;
    }

    #endregion

    #region PRIVATE METHODS

    /// <summary>
    /// Handles sinusoidal movement within defined bounds
    /// </summary>
    private void UpdateMovement()
    {
        _movementTime += Time.deltaTime * _moveSpeed;

        // Horizontal movement
        float x = Mathf.Sin(_movementTime) * _currentXRange;

        // Vertical movement (only upwards from start position)
        float yRaw = Mathf.Sin(_movementTime * 0.8f) * _currentYRange;
        float y = Mathf.Max(0f, yRaw);

        transform.position = new Vector3(
            _startPos.x + x,
            _startPos.y + y,
            _startPos.z
        );
    }

    #endregion
}