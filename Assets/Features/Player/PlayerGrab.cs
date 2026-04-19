using UnityEngine;
using UnityEngine.InputSystem;

// Handles grabbing, charging, and throwing the ball

public class PlayerGrab : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float grabRange = 3f;
    [SerializeField] private float grabRadius = 0.5f;

    [Header("Throw Settings")]
    [SerializeField] private float maxThrowForce = 5f;
    [SerializeField] private float chargeSpeed = 6f;

    [Header("Assist Settings")]
    [SerializeField] private float arcHeight = 2.5f;

    [Header("References")]
    [SerializeField] private BallSpawner ballSpawner;

    [SerializeField] private Transform aimTarget;

    private BallController heldBall;
    private float currentCharge = 0f;

    private bool justPickedUp = false;
    private bool isActive = false;

    void Update()
    {
        if (!isActive) return;

        if (heldBall == null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                TryGrab();
            }
            return;
        }

        if (justPickedUp)
        {
            justPickedUp = false;
            return;
        }

        if (Mouse.current.leftButton.isPressed)
        {
            currentCharge += chargeSpeed * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0f, maxThrowForce);
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ThrowBall();
        }
    }

    void TryGrab()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.SphereCast(ray, grabRadius, out hit, grabRange))
        {
            BallController ball = hit.collider.GetComponent<BallController>();

            if (ball != null && ball.CanBePickedUp())
            {
                heldBall = ball;
                ball.PickUp(holdPoint);

                justPickedUp = true;
                currentCharge = 0f;
            }
        }
    }

    void ThrowBall()
    {
        Vector3 start = holdPoint.position;
        Vector3 direction = playerCamera.transform.forward;

        if (aimTarget != null)
        {
            Vector3 target = aimTarget.position;

            Vector3 velocity = CalculateArcVelocity(start, target, arcHeight);

            float powerMultiplier = Mathf.Lerp(0.95f, 1.05f, currentCharge / maxThrowForce);
            velocity *= powerMultiplier;

            heldBall.Throw(velocity.normalized, velocity.magnitude);
        }
        else
        {
            // fallback
            heldBall.Throw(direction, maxThrowForce);
        }

        heldBall = null;
        currentCharge = 0f;

        // Spawn next ball
        if (ballSpawner != null)
        {
            ballSpawner.SpawnBall();
        }
    }

    Vector3 CalculateArcVelocity(Vector3 start, Vector3 target, float height)
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

    public float GetChargeNormalized()
    {
        return currentCharge / maxThrowForce;
    }

    public void EnableInput()
    {
        isActive = true;
    }

    public void DisableInput()
    {
        isActive = false;

        if (heldBall != null)
        {
            heldBall.Release();
            heldBall = null;
        }

        currentCharge = 0f;
        justPickedUp = false;
    }
}