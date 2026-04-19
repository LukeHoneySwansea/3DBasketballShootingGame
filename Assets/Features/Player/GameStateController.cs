using UnityEngine;
using UnityEngine.Events;

// Controls overall game state (menu / gameplay)

public class GameStateController : MonoBehaviour
{
    public enum GameState
    {
        Menu,
        Playing
    }

    public GameState currentState = GameState.Menu;

    [Header("Events")]
    public UnityEvent OnMenuState;
    public UnityEvent OnPlayState;

    void Start()
    {
        SetState(GameState.Menu);
    }

    public void SetState(GameState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        switch (currentState)
        {
            case GameState.Menu:
                OnMenuState?.Invoke();
                break;

            case GameState.Playing:
                OnPlayState?.Invoke();
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
}