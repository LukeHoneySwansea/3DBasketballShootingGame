using UnityEngine;
using UnityEngine.InputSystem;

// Handles grabbing, charging, aiming, and throwing the ball

public class PlayerGrab : MonoBehaviour
{
    #region SERIALIZED FIELDS

    [Header("Grab Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float grabRange = 3f;
    [SerializeField] private float grabRadius = 0.5f;

    [Header("Throw Settings")]
    [SerializeField] private float maxThrowForce = 5f;
    [SerializeField] private float chargeSpeed = 6f;

    [Header("Assist Settings")]
    [SerializeField] private float arcHeight = 2.5f;
    [SerializeField] private Transform aimTarget;

    [Header("References")]
    [SerializeField] private BallSpawner ballSpawner;

    #endregion

    #region PRIVATE FIELDS

    private BallController _heldBall;

    private float _currentCharge = 0f;

    private bool _justPickedUp = false;
    private bool _isActive = false;

    #endregion

    #region UNITY METHODS

    private void Update()
    {
        if (!_isActive) return;

        if (_heldBall == null)
        {
            HandleGrabInput();
            return;
        }

        HandleHeldState();
    }

    #endregion

    #region INPUT HANDLING

    private void HandleGrabInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryGrab();
        }
    }

    private void HandleHeldState()
    {
        // Prevent instant charge right after pickup
        if (_justPickedUp)
        {
            _justPickedUp = false;
            return;
        }

        HandleCharging();

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ThrowBall();
        }
    }

    private void HandleCharging()
    {
        if (!Mouse.current.leftButton.isPressed) return;

        _currentCharge += chargeSpeed * Time.deltaTime;
        _currentCharge = Mathf.Clamp(_currentCharge, 0f, maxThrowForce);
    }

    #endregion

    #region GRAB LOGIC

    private void TryGrab()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Physics.SphereCast(ray, grabRadius, out RaycastHit hit, grabRange))
        {
            BallController ball = hit.collider.GetComponent<BallController>();

            if (ball != null && ball.CanBePickedUp())
            {
                _heldBall = ball;
                _heldBall.PickUp(holdPoint);

                _justPickedUp = true;
                _currentCharge = 0f;
            }
        }
    }

    #endregion

    #region THROW LOGIC

    private void ThrowBall()
    {
        Vector3 start = holdPoint.position;

        if (aimTarget != null)
        {
            ThrowWithAssist(start);
        }
        else
        {
            ThrowForward();
        }

        ClearHeldBall();

        SpawnNextBall();
    }

    private void ThrowWithAssist(Vector3 start)
    {
        Vector3 target = aimTarget.position;

        Vector3 velocity = CalculateArcVelocity(start, target, arcHeight);

        float powerMultiplier = Mathf.Lerp(
            0.95f,
            1.05f,
            _currentCharge / maxThrowForce
        );

        velocity *= powerMultiplier;

        _heldBall.Throw(velocity.normalized, velocity.magnitude);
    }

    private void ThrowForward()
    {
        Vector3 direction = playerCamera.transform.forward;
        _heldBall.Throw(direction, maxThrowForce);
    }

    private void SpawnNextBall()
    {
        if (ballSpawner != null)
        {
            ballSpawner.SpawnBall();
        }
    }

    private void ClearHeldBall()
    {
        _heldBall = null;
        _currentCharge = 0f;
    }

    #endregion

    #region ARC CALCULATION

    // Calculates a clean arc from start → target
    private Vector3 CalculateArcVelocity(Vector3 start, Vector3 target, float height)
    {
        float gravity = Physics.gravity.y;

        Vector3 displacement = target - start;
        Vector3 displacementXZ = new Vector3(displacement.x, 0f, displacement.z);

        float timeUp = Mathf.Sqrt(-2f * height / gravity);
        float timeDown = Mathf.Sqrt(2f * (displacement.y - height) / gravity);
        float totalTime = timeUp + timeDown;

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2f * gravity * height);
        Vector3 velocityXZ = displacementXZ / totalTime;

        return velocityXZ + velocityY;
    }

    #endregion

    #region PUBLIC METHODS

    public float GetChargeNormalized()
    {
        return _currentCharge / maxThrowForce;
    }

    public void EnableInput()
    {
        _isActive = true;
    }

    public void DisableInput()
    {
        _isActive = false;

        if (_heldBall != null)
        {
            _heldBall.Release();
            _heldBall = null;
        }

        _currentCharge = 0f;
        _justPickedUp = false;
    }

    #endregion
}