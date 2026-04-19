using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;

// Updates the UI power bar based on throw charge

public class UIPowerBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private PlayerGrab playerGrab;

    private bool isActive = false;

    void OnEnable()
    {
        // Reset bar when enabled
        if (slider != null)
        {
            slider.value = 0f;
        }
    }

    void Update()
    {
        if (!isActive) return;
        if (slider == null || playerGrab == null) return;

        float value = playerGrab.GetChargeNormalized();

        slider.value = Mathf.Lerp(
            slider.value,
            value,
            10f * Time.deltaTime
        );
    }

    public void StartTracking()
    {
        isActive = true;
    }

    public void StopTracking()
    {
        isActive = false;

        if (slider != null)
        {
            slider.value = 0f;
        }
    }
}