using UnityEngine;
using UnityEngine.UI;

// Updates the UI power bar based on player throw charge

public class UIPowerBar : MonoBehaviour
{
    #region SERIALIZED FIELDS

    [SerializeField] private Slider slider;
    [SerializeField] private PlayerGrab playerGrab;

    [SerializeField] private float smoothSpeed = 10f;

    #endregion

    #region PRIVATE FIELDS

    private bool _isTracking = false;

    #endregion

    #region UNITY METHODS

    private void OnEnable()
    {
        ResetBar();
    }

    private void Update()
    {
        if (!_isTracking) return;
        if (slider == null || playerGrab == null) return;

        UpdateBar();
    }

    #endregion

    #region BAR LOGIC

    /// <summary>
    /// Smoothly updates the slider based on current throw charge
    /// </summary>
    private void UpdateBar()
    {
        float targetValue = playerGrab.GetChargeNormalized();

        slider.value = Mathf.Lerp(
            slider.value,
            targetValue,
            smoothSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// Resets the power bar to zero
    /// </summary>
    private void ResetBar()
    {
        if (slider != null)
        {
            slider.value = 0f;
        }
    }

    #endregion

    #region PUBLIC CONTROL

    /// <summary>
    /// Start tracking player input (when gameplay begins)
    /// </summary>
    public void StartTracking()
    {
        _isTracking = true;
    }

    /// <summary>
    /// Stop tracking and reset UI (when gameplay ends or menu opens)
    /// </summary>
    public void StopTracking()
    {
        _isTracking = false;
        ResetBar();
    }

    #endregion
}