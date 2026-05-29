using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameState debugState;

    private GameControls _controls;

    public static GameState gameState;

    public static GameManager instance;
    private void Awake() {
        if (!instance) { instance = this; }

        _controls = new GameControls();
        _controls.Enable();

        SetGameState(GameState.Playing);
        InputManager.instance.TrySetInputMode(InputMode.Character);
        MouseLock.LockMouse();
    }

    //Whatever happens when exiting a game state
    private void ExitCurrentGameState() {
        switch (gameState) {
            case GameState.Playing:
                break;
            case GameState.InCinematic:
                break;
            case GameState.InInterface:
                break;
            default:
                break;
        }
    }

    public void SetGameState(GameState newState) {
        ExitCurrentGameState();

        gameState = newState;

        EnterGameState();
    }
    //Whatever happens when entering a new gamestate
    private void EnterGameState() {
        switch (gameState) {
            case GameState.Playing:
                break;
            case GameState.InCinematic:
                break;
            case GameState.InInterface:
                break;
            default:
                break;
        }
    }

    private void Update() {
        debugState = gameState;
    }
}

public enum GameState : byte {
    Playing, //The player is in the most basic state of the game, usually you can transition to any state or Input mode from here
    InInterface, //Basically any interface that allows input interaction
    InCinematic, //Interfaces without interaction. Can't pause during a cinematic.
}
//TODO: Task list shows the number of this line, I'm interested in seeing the total amount of lines my game has, so I will put this in the last line of every script I find.
