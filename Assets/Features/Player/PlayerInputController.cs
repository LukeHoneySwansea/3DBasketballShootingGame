using UnityEngine;
using UnityEngine.InputSystem;

// Handles global player input (menu toggle & game state switching)

public class PlayerInputController : MonoBehaviour
{
    #region SERIALIZED FIELDS

    [SerializeField] private GameStateController gameStateController;

    #endregion

    #region UNITY METHODS

    private void Update()
    {
        HandleMenuToggleInput();
    }

    #endregion

    #region INPUT HANDLING

    /// <summary>
    /// Checks for menu toggle input (M key)
    /// </summary>
    private void HandleMenuToggleInput()
    {
        // Safety check (Input System may not be ready in some edge cases)
        if (Keyboard.current == null) return;

        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            ToggleMenu();
        }
    }

    #endregion

    #region STATE CONTROL

    /// <summary>
    /// Toggles between Menu and Playing states
    /// </summary>
    private void ToggleMenu()
    {
        if (gameStateController == null) return;

        switch (gameStateController.CurrentState)
        {
            case GameStateController.GameState.Playing:
                // If currently playing go back to menu
                gameStateController.ReturnToMenu();
                break;

            case GameStateController.GameState.Menu:
                // If in menu go to start game
                gameStateController.StartGame();
                break;
        }
    }

    #endregion
}