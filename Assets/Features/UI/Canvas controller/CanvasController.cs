using UnityEngine;

// Controls switching between menu and player canvases with smooth fading

public class CanvasController : MonoBehaviour
{
    #region SERIALIZED FIELDS

    [SerializeField] private CanvasGroup menuCanvas;
    [SerializeField] private CanvasGroup playerCanvas;

    [SerializeField] private float fadeSpeed = 5f;

    #endregion

    #region PRIVATE FIELDS

    private bool _isMenuActive = true;

    #endregion

    #region UNITY METHODS

    private void Start()
    {
        // Start on menu instantly (no fade)
        SetMenuActive(true, true);
    }

    private void Update()
    {
        UpdateCanvas(menuCanvas, _isMenuActive, true);
        UpdateCanvas(playerCanvas, !_isMenuActive, false);
    }

    #endregion

    #region CANVAS CONTROL

    /// <summary>
    /// Smoothly updates canvas alpha and interaction state
    /// </summary>
    private void UpdateCanvas(CanvasGroup canvas, bool isActive, bool isMenu)
    {
        if (canvas == null) return;

        float targetAlpha = isActive ? 1f : 0f;

        // Smooth fade towards target alpha
        canvas.alpha = Mathf.MoveTowards(
            canvas.alpha,
            targetAlpha,
            fadeSpeed * Time.deltaTime
        );

        // Interaction logic
        // Menu is clickable when active
        // Player UI is also clickable when active (fixed)
        canvas.interactable = isActive;

        // Only block raycasts when visible
        canvas.blocksRaycasts = isActive;
    }

    #endregion

    #region PUBLIC METHODS

    /// <summary>
    /// Switch to gameplay UI
    /// </summary>
    public void ShowPlayerUI()
    {
        _isMenuActive = false;
    }

    /// <summary>
    /// Switch to menu UI
    /// </summary>
    public void ShowMenuUI()
    {
        _isMenuActive = true;
    }

    /// <summary>
    /// Instantly set UI state (used on startup or hard transitions)
    /// </summary>
    public void SetMenuActive(bool active, bool instant = false)
    {
        _isMenuActive = active;

        if (!instant) return;

        // Menu canvas
        if (menuCanvas != null)
        {
            menuCanvas.alpha = active ? 1f : 0f;
            menuCanvas.interactable = active;
            menuCanvas.blocksRaycasts = active;
        }

        // Player canvas
        if (playerCanvas != null)
        {
            playerCanvas.alpha = active ? 0f : 1f;
            playerCanvas.interactable = !active; // FIXED
            playerCanvas.blocksRaycasts = !active;
        }
    }

    #endregion
}