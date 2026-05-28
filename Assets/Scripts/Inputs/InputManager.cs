using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public enum InputState { Character, Building, Interface }
public class InputManager : MonoBehaviour {
    public static InputManager Instance { get; private set; }
    public static GameControls GameControls { get; private set; }
    public static string CurrentControlScheme = "";

    public InputState currentState;

    public static Action<string, ControllerType> OnControlSchemeChanged;

    // Character Variables
    public static Vector2 Movement => GameControls.Character.Movement.ReadValue<Vector2>();
    public static Vector2 ViewDirection => GameControls.Character.View.ReadValue<Vector2>();
    public static bool IsMoving => Movement.magnitude > 0.1f;
    public static bool IsChangingViewDirection => ViewDirection.magnitude > 0.1f;
    public bool IsLocked => _isLocked;

    // Building Variables
    public static Vector2 BuildingRadialSelection => GameControls.Building.RadialSelection.ReadValue<Vector2>();

    private bool _isLocked = false;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            GameControls = new GameControls();

            GameControls.System.Enable();
        } else {
            Destroy(gameObject);
        }
    }

    private void OnEnable() {
        InputUser.onChange += OnInputDeviceChange;

        GameControls.System.Enable();
        GameControls.Character.Enable();
        GameControls.Interface.Enable();
    }

    private void OnDisable() {
        InputUser.onChange -= OnInputDeviceChange;

        GameControls.System.Disable();
        GameControls.Character.Disable();
        GameControls.Interface.Disable();
    }

    public void SetInputState(InputState newState) {
        if (_isLocked) return;

        // desactivamos todo
        GameControls.Character.Disable();
        GameControls.Building.Disable();
        GameControls.Interface.Disable();

        // este siempre siempre siempre lo mantenemos activo
        GameControls.System.Enable();

        switch (newState) {
            case InputState.Character:
                GameControls.Character.Enable();
                currentState = InputState.Character;

                // Debug.Log("character enabled");
                break;
            case InputState.Building:
                GameControls.Building.Enable();
                currentState = InputState.Building;

                // Debug.Log("building enabled");
                break;
            case InputState.Interface:
                GameControls.Interface.Enable();
                currentState = InputState.Interface;

                // Debug.Log("interface enabled");
                break;
        }
    }

    public void DisableInputsForCinematic() {
        GameControls.Character.Disable();
        GameControls.Building.Disable();
        GameControls.Interface.Disable();
        LockInputState(true);
    }

    public void RecoverInputsAfterCinematic() {
        LockInputState(false);
        SetInputState(InputState.Character);
    }

    public void LockInputState(bool state) {
        _isLocked = state;
    }

    private void OnInputDeviceChange(InputUser user, InputUserChange change, InputDevice device) {
        if (change == InputUserChange.ControlSchemeChanged) {
            // Debug.Log($"Control Scheme Changed: {user.controlScheme.Value.name}\nController type: {Controller.GetControllerType()}");

            CurrentControlScheme = user.controlScheme.Value.name;

            OnControlSchemeChanged?.Invoke(CurrentControlScheme, Controller.GetControllerType());

            MouseLock.ManageMouse(GameManager.GameState);
        }

        if (change == InputUserChange.DevicePaired) {
        }
    }

}
