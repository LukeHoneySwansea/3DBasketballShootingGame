using UnityEngine;
using UnityEngine.Events;

public class UIMenuContoller : MonoBehaviour
{
    [SerializeField] private CanvasGroup menuCanvasGroup;
    [SerializeField] private GameObject menuRoot; // parent object of the menu

    [SerializeField] private GameStateController gameStateController;

    // Called when Start button is pressed
    public void StartGame()
    {
        if (gameStateController != null)
        {
            gameStateController.StartGame();
        }
    }

    // Called when Settings button is pressed
    public void OpenSettings()
    {
        // TODO: Implement settings menu in future development
    }

    // Called when Credits button is pressed
    public void OpenCredits()
    {
        // TODO: Implement credits screen in future development
    }

    // Called when Quit button is pressed
    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}