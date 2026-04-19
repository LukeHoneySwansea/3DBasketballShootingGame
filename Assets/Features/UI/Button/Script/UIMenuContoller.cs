using UnityEngine;

// Handles main menu button actions

public class UIMenuController : MonoBehaviour
{
    #region SERIALIZED FIELDS

    [SerializeField] private GameStateController gameStateController;

    // Optional future references (kept for expansion)
    [SerializeField] private CanvasGroup menuCanvasGroup;
    [SerializeField] private GameObject menuRoot;

    #endregion

    #region PUBLIC BUTTON METHODS

    /// <summary>
    /// Starts the game (called by Start button)
    /// </summary>
    public void StartGame()
    {
        if (gameStateController == null) return;

        gameStateController.StartGame();
    }

    /// <summary>
    /// Opens settings menu (not yet implemented)
    /// </summary>
    public void OpenSettings()
    {
        // TODO: Hook into settings UI when implemented
        Debug.Log("Settings menu not implemented yet.");
    }

    /// <summary>
    /// Opens credits screen (not yet implemented)
    /// </summary>
    public void OpenCredits()
    {
        // TODO: Hook into credits UI when implemented
        Debug.Log("Credits not implemented yet.");
    }

    /// <summary>
    /// Quits the application
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        // Allows quitting while testing in editor
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    #endregion
}