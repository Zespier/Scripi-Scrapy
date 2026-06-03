using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputManager : MonoBehaviour {

    public InputMode currentInputMode;
    public static string currentControlScheme = "";

    // Character Variables
    public static GameControls GameControls { get; private set; }
    public static Vector2 Movement => GameControls.Character.Movement.ReadValue<Vector2>();
    public static Vector2 ViewDirection => GameControls.Character.View.ReadValue<Vector2>();
    public static bool IsMoving => Movement.magnitude > 0.1f;
    public static bool IsChangingViewDirection => ViewDirection.magnitude > 0.1f;

    // Building Variables
    public static Vector2 BuildingRadialSelection => GameControls.Building.RadialSelection.ReadValue<Vector2>();

    public static Action<string, ControllerType> OnControlSchemeChanged;

    public static InputManager instance;
    private void Awake() {
        if (!instance) {
            instance = this;
            DontDestroyOnLoad(gameObject);

            GameControls = new GameControls();
            GameControls.Enable();

        } else {
            Destroy(gameObject);
        }
    }

    private void OnEnable() {
        InputUser.onChange += OnInputDeviceChange;
    }

    private void OnDisable() {
        InputUser.onChange -= OnInputDeviceChange;

        GameControls.System.Disable();
        GameControls.Character.Disable();
        GameControls.Building.Disable();
        GameControls.Interface.Disable();
    }

    public bool TrySetInputMode(InputMode mode) {
        if (!CanUseInputMode(mode))
            return false;

        SetInputMode(mode);

        return true;
    }

    //Whatever happens when exitting an input mode
    private void ExitCurrentInputMode() {
        switch (currentInputMode) {
            case InputMode.Character:
                break;
            case InputMode.Building:
                break;
            case InputMode.Interface:
                break;
            case InputMode.None:
                break;
            default:
                break;
        }
    }

    private void SetInputMode(InputMode newState) {
        ExitCurrentInputMode();

        currentInputMode = newState;

        EnablingOfInputMaps();

        EnterNewState();
    }

    private void EnablingOfInputMaps() {
        // desactivamos todo
        GameControls.Character.Disable();
        GameControls.Building.Disable();
        GameControls.Interface.Disable();

        // este siempre siempre siempre lo mantenemos activo
        GameControls.System.Enable();


        switch (currentInputMode) {
            case InputMode.Character:
                GameControls.Character.Enable();
                break;

            case InputMode.Building:
                GameControls.Building.Enable();
                break;

            case InputMode.Interface:
                GameControls.Interface.Enable();
                break;

            case InputMode.None:
                break;
        }
    }

    private void EnterNewState() {
        switch (currentInputMode) {
            case InputMode.Character:
                break;
            case InputMode.Building:
                break;
            case InputMode.Interface:
                break;
            case InputMode.None:
                break;
            default:
                break;
        }
    }

    private bool CanUseInputMode(InputMode mode) {
        //Something REALLY important to understand from this, is taht GameState and InputModes, even if they look alike, are very different. Game state is the state of the game, for example, "playing" means that the player is free to move, attack, build, interact with something on the scene, etc... 
        //InputMode in the other hand, is how the inputs are interpreted, for example, "Character" means the mechanics of the character Skuld, "Building" only reacts to the build mode inputs, and "Interface" moves throught UI. 
        //The problem we had earlier is that building should only be allowed when the game state is "playing", but you could change to building during a cinematic, or watching the endgame screen.
        //SOOOOO, this method is basically putting a little check before changing input mode, since InputMode is changed independetly of gameState.

        switch (GameManager.gameState) {
            case GameState.Playing:
                return mode == InputMode.Character || mode == InputMode.Building; //It would make 0 sense to detect interface inputs during gameplay
            case GameState.InInterface:
                return mode == InputMode.Interface; //During an interactable interface, it's only allowed to detect interface inputs
            case GameState.InCinematic:
                return mode == InputMode.None; //During a cinematic the only input mode allowed is no input detection at all

            //I know some people might want to go to InputMode.None during any other gamestate, but honestly, why? That is a cinematic.
            //If not, just add mode == InputMode.None to the other gameStates
            default:
                break;
        }

        return true;
    }

    private void OnInputDeviceChange(InputUser user, InputUserChange change, InputDevice device) {
        if (change == InputUserChange.ControlSchemeChanged) {

            currentControlScheme = user.controlScheme.Value.name;

            OnControlSchemeChanged?.Invoke(currentControlScheme, Controller.GetControllerType());

            //ES VERDAD EL RATON SI PONEMOS EL MANDO QUE PASA
            //MouseLock.ManageMouse(GameManager.GameState);
        }
    }
}

public enum InputMode {
    Character,
    Building,
    Interface,
    None
}