using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using static GameControls;

public enum InputState { Character, Building, Interface }
public class InputManager : MonoBehaviour, ISystemActions, ICharacterActions, IBuildingActions, IInterfaceActions {
    public static InputManager Instance { get; private set; }
    public static GameControls GameControls { get; private set; }
    public static string CurrentControlScheme = "";

    public InputState currentState;

    public static InterfaceActionsWrapper Interface = new InterfaceActionsWrapper();

    public static Action<string, ControllerType> OnControlSchemeChanged;

    // System Map Events
    public static Action<InputAction.CallbackContext> OnGamePause;
    public static Action<InputAction.CallbackContext> OnBuildingToggled;
    public static Action<InputAction.CallbackContext> OnInventoryToggled;

    // Character Map Events
    public static Action<InputAction.CallbackContext> OnCharacterAbility;
    public static Action<InputAction.CallbackContext> OnCharacterAttack;
    public static Action<InputAction.CallbackContext> OnCharacterInteract;
    public static Action<InputAction.CallbackContext> OnCharacterSprint;

    // Building Map Events
    public static Action<InputAction.CallbackContext> OnBuildConfirmed;
    public static Action<InputAction.CallbackContext> OnBuildNavigation;
    public static Action<InputAction.CallbackContext> OnBuildBranchSwitched;


    // Hacks
    public static Action<InputAction.CallbackContext> HACK_OnChangeSkin;

    // Character Variables
    public static Vector2 Movement => GameControls.Character.Movement.ReadValue<Vector2>();
    public static Vector2 ViewDirection => GameControls.Character.View.ReadValue<Vector2>();
    public static bool IsMoving => Movement.magnitude > 0.1f;
    public static bool IsChangingViewDirection => ViewDirection.magnitude > 0.1f;
    public bool IsLocked => _isLocked;

    // Building Variables
    public static Vector2 BuildingRadialSelection => GameControls.Building.RadialSelection.ReadValue<Vector2>();

    private bool _isLocked = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            GameControls = new GameControls();

            GameControls.System.SetCallbacks(this);
            GameControls.Character.SetCallbacks(this);
            GameControls.Building.SetCallbacks(this);
            GameControls.Interface.SetCallbacks(this);

            GameControls.System.Enable();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable() {
        InputUser.onChange += OnInputDeviceChange;

        GameControls.System.Enable();
    }

    private void OnDisable() {
        InputUser.onChange -= OnInputDeviceChange;

        GameControls.System.Disable();
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

    #region System Actions
    public void OnPause(InputAction.CallbackContext context) {
        OnGamePause?.Invoke(context);
    }

    public void OnToggleBuilder(InputAction.CallbackContext context) {
        OnBuildingToggled?.Invoke(context);
    }

    public void OnToggleInventory(InputAction.CallbackContext context) {
        OnInventoryToggled?.Invoke(context);
    }
    #endregion

    #region Character Actions
    public void OnAttack(InputAction.CallbackContext context) {
        OnCharacterAttack?.Invoke(context);
    }

    public void OnChangeSkin(InputAction.CallbackContext context) {
        HACK_OnChangeSkin?.Invoke(context);
    }

    public void OnDance(InputAction.CallbackContext context) {
    }

    public void OnInteract(InputAction.CallbackContext context) {
        OnCharacterInteract?.Invoke(context);
    }

    public void OnJump(InputAction.CallbackContext context) {
    }

    public void OnMovement(InputAction.CallbackContext context) {
    }

    public void OnSpecialAbility(InputAction.CallbackContext context) {
        OnCharacterAbility?.Invoke(context);
    }

    public void OnSprint(InputAction.CallbackContext context) {
        OnCharacterSprint?.Invoke(context);
    }

    public void OnView(InputAction.CallbackContext context) {
    }
    #endregion

    #region Building Actions
    public void OnConfirmPlacement(InputAction.CallbackContext context) {
        OnBuildConfirmed?.Invoke(context);
    }

    public void OnCycleUpgradesBranch(InputAction.CallbackContext context) {
        OnBuildBranchSwitched?.Invoke(context);
    }

    public void OnRadialSelection(InputAction.CallbackContext context) {
        OnBuildNavigation?.Invoke(context);
    }
    #endregion

    #region Interface Actions
    public void OnNavigate(InputAction.CallbackContext context)
    {
        Interface.OnNavigation?.Invoke(context);
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        Interface.OnSubmit?.Invoke(context);
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        Interface.OnCancel?.Invoke(context);
    }

    public void OnPoint(InputAction.CallbackContext context) {
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        Interface.OnClick?.Invoke(context);
    }

    public void OnScrollWheel(InputAction.CallbackContext context) {
    }

    public void OnMiddleClick(InputAction.CallbackContext context) {
    }

    public void OnRightClick(InputAction.CallbackContext context) {
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context) {
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) {
    }

    public void OnSwitchBranch(InputAction.CallbackContext context)
    {
        Interface.OnSwitchBranch?.Invoke(context);
    }

    public void OnSwitchMenu(InputAction.CallbackContext context)
    {
        Interface.OnSwitchMenu?.Invoke(context);
    }

    public void OnAccept(InputAction.CallbackContext context)
    {
        Interface.OnInteraction?.Invoke(context);
    }

    public void OnDetails(InputAction.CallbackContext context)
    {
        Interface.OnDetails?.Invoke(context);
    }

    public void OnBack(InputAction.CallbackContext context)
    {
        Interface.OnBack?.Invoke(context);
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        Interface.OnConfirm?.Invoke(context);
    }

    public void OnAny(InputAction.CallbackContext context) {
        Interface.OnAny?.Invoke(context);
    }

    #endregion
}

public class InterfaceActionsWrapper
{
    // Interface Map Events
    public Action<InputAction.CallbackContext> OnBack;
    public Action<InputAction.CallbackContext> OnSwitchBranch;
    public Action<InputAction.CallbackContext> OnSwitchMenu;
    public Action<InputAction.CallbackContext> OnClick;
    public Action<InputAction.CallbackContext> OnConfirm;
    public Action<InputAction.CallbackContext> OnDetails;
    public Action<InputAction.CallbackContext> OnInteraction;
    public Action<InputAction.CallbackContext> OnSubmit;
    public Action<InputAction.CallbackContext> OnCancel;
    public Action<InputAction.CallbackContext> OnNavigation;
    public Action<InputAction.CallbackContext> OnAny;
}
