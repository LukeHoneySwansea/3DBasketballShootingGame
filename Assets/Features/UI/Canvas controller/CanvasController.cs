using UnityEngine;

// Controls switching between menu and player canvases with smooth fade

public class CanvasController : MonoBehaviour
{
    [SerializeField] private CanvasGroup menuCanvas;
    [SerializeField] private CanvasGroup playerCanvas;

    [SerializeField] private float fadeSpeed = 5f;

    private bool menuActive = true;

    void Start()
    {
        SetMenuActive(true, true); // start on menu instantly
    }

    void Update()
    {
        // Smooth fade between canvases
        UpdateCanvas(menuCanvas, menuActive, true);
        UpdateCanvas(playerCanvas, !menuActive, false);
    }

    void UpdateCanvas(CanvasGroup canvas, bool isActive, bool isMenu)
    {
        if (canvas == null) return;

        float targetAlpha = isActive ? 1f : 0f;

        // Smooth fade (reliable)
        canvas.alpha = Mathf.MoveTowards(
            canvas.alpha,
            targetAlpha,
            fadeSpeed * Time.deltaTime
        );

        // Interactions
        if (isMenu)
        {
            canvas.interactable = isActive;
        }
        else
        {
            canvas.interactable = false;
        }

        canvas.blocksRaycasts = isActive;
    }

    // Call this to switch to gameplay
    public void ShowPlayerUI()
    {
        menuActive = false;
    }

    // Call this to return to menu
    public void ShowMenuUI()
    {
        menuActive = true;
    }

    // Optional: instant set (no fade)
    public void SetMenuActive(bool active, bool instant = false)
    {
        menuActive = active;

        if (!instant) return;

        if (menuCanvas != null)
        {
            menuCanvas.alpha = active ? 1f : 0f;
            menuCanvas.interactable = active;
            menuCanvas.blocksRaycasts = active;
        }

        if (playerCanvas != null)
        {
            playerCanvas.alpha = active ? 0f : 1f;
            playerCanvas.interactable = false;
            playerCanvas.blocksRaycasts = !active;
        }
    }
}