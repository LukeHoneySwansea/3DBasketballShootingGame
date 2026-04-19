using UnityEngine;
using UnityEngine.Events;

// Controls overall game state (Menu ↔ Gameplay)

public class GameStateController : MonoBehaviour
{
    #region ENUMS

    public enum GameState
    {
        Menu,
        Playing
    }

    #endregion

    #region SERIALIZED FIELDS

    [Header("Events")]
    public UnityEvent OnMenuState;
    public UnityEvent OnPlayState;

    [Header("References")]
    [SerializeField] private GameRoundController roundController;

    #endregion

    #region PUBLIC STATE

    public GameState CurrentState => _currentState;

    #endregion

    #region PRIVATE FIELDS

    private GameState _currentState = GameState.Menu;

    #endregion

    #region UNITY METHODS

    private void Start()
    {
        SetState(GameState.Menu);
    }

    #endregion

    #region STATE CONTROL

    public void SetState(GameState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;

        switch (_currentState)
        {
            case GameState.Menu:
                EnterMenuState();
                break;

            case GameState.Playing:
                EnterPlayState();
                break;
        }
    }

    public void StartGame()
    {
        SetState(GameState.Playing);
    }

    public void ReturnToMenu()
    {
        SetState(GameState.Menu);
    }

    #endregion

    #region STATE HANDLERS

    private void EnterMenuState()
    {
        // Reset game when returning to menu (fixes round carry-over bug)
        if (roundController != null)
        {
            roundController.ResetGame();
        }

        OnMenuState?.Invoke();
    }

    private void EnterPlayState()
    {
        OnPlayState?.Invoke();

        // Start first round explicitly
        if (roundController != null)
        {
            roundController.StartRound();
        }
    }

    #endregion
}