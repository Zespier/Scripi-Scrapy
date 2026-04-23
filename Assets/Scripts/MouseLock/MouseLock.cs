using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseLock {

    public static CursorState CursorState { get; set; }

    public static void LockMouse() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        CursorState = CursorState.Hidden;
    }

    public static void UnlockMouse() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        CursorState = CursorState.Visible;
    }

    public static void ManageMouse(GameState gameState) {

        if (Controller.IsControllerConnected()) {
            LockMouse();
        } else {

            UnlockMouse();

            switch (gameState) {
                case GameState.Playing:
                    Scene currentScene = SceneManager.GetActiveScene();
                    if (currentScene.name == "Lobby" || currentScene.name == "Lobby_SD") {
                        LockMouse();
                    } else {
                        UnlockMouse();
                    }
                    break;

                case GameState.SeeingAnimation:
                case GameState.Dying:
                    LockMouse();
                    break;
                case GameState.Dialoguing:
                case GameState.Paused:
                case GameState.InInterface:
                case GameState.WatchingQuestLog:
                case GameState.SeeingForgeAnimation:
                    UnlockMouse();
                    break;
                default:
                    break;
            }
        }
    }
}
public enum CursorState {
    Visible,
    Hidden
}
//TODO: Task list shows the number of this line, I'm interested in seeing the total amount of lines my game has, so I will put this in the last line of every script I find.