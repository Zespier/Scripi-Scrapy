using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuUpgrades : MonoBehaviour {

    public CanvasGroup canvasGroup;

    public List<UpgradeDataSO> upgrades;
    public List<UpgradeButton> upgradeButtons;

    public static MenuUpgrades instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    private void Update() {
        if (InputManager.GameControls.Character.ToggleInventory.WasPressedThisFrame() || InputManager.GameControls.Interface.ToggleInventory.WasPressedThisFrame()) {
            ActiveCanvasGroup(!canvasGroup.interactable);
            if (canvasGroup.alpha == 1) {
                GameManager.instance.SetGameState(GameState.InInterface);
                InputManager.instance.TrySetInputMode(InputMode.Interface);
                MouseLock.UnlockMouse();

            } else {
                GameManager.instance.SetGameState(GameState.Playing);
                InputManager.instance.TrySetInputMode(InputMode.Character);
                MouseLock.LockMouse();
            }
        }

        if (InputManager.GameControls.Character.ChangeSkin.WasPressedThisFrame()) {
            Money.instance.currentMoney++;
            Money.instance.currentMoney *= 10;
        }

        CheckGameReset();
    }

    public int GetUpgradeLevel(UpgradeType type) {
        int level = 0;

        for (int i = 0; i < upgrades.Count; i++) {
            if (upgrades[i].type == type && upgrades[i].isUnlocked) {
                level++;
            }
        }
        return level;
    }

    public void ActiveCanvasGroup(bool active) {
        canvasGroup.alpha = active ? 1 : 0;
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }

    public void Upgrade(UpgradeDataSO upgradeData) {
        if (Money.instance.CanBuy(upgradeData.cost)) {
            Money.instance.SpendMoney(upgradeData.cost);

            for (int i = 0; i < upgrades.Count; i++) {
                if (upgrades[i].id == upgradeData.id) {
                    upgrades[i].isUnlocked = true;
                    break;
                }
            }
        }

        ApplyUpgradeEffect(upgradeData);
    }

    public void ApplyUpgradeEffect(UpgradeDataSO upgradeData) {
        switch (upgradeData.type) {
            case UpgradeType.HandDrill:
                HandDrillStats.isUnlocked = upgradeData.isUnlocked;
                break;
            case UpgradeType.HitDamage:
            case UpgradeType.DrillSpeed:
            case UpgradeType.HitArea:
            case UpgradeType.HandDrillSpeed:
            case UpgradeType.BetterGeodes:
            default:
                break;
        }
    }

    public void CheckGameReset() {
        if (Keyboard.current.pKey.isPressed && Keyboard.current.oKey.isPressed && Keyboard.current.iKey.isPressed) {

            GameManager.instance.saveOnDestroy = false;
            SaveSystem.DeleteSaveFile();

            SceneManager.LoadScene(0);
        }
    }
}
