using UnityEngine;
using UnityEngine.InputSystem;

// Handles global player input (menu toggle, etc.)

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private GameStateController gameStateController;

    void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            ToggleMenu();
        }
    }

    void ToggleMenu()
    {
        if (gameStateController == null) return;

        if (gameStateController.currentState == GameStateController.GameState.Playing)
        {
            gameStateController.ReturnToMenu();
        }
        else
        {
            gameStateController.StartGame();
        }
    }
}