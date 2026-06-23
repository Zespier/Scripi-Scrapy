using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUpgrades : MonoBehaviour {

    public CanvasGroup canvasGroup;
    public RectTransform outline;
    public List<UpgradeDataSO> upgrades;
    //public List<UpgradeButton> upgradeButtons;
    public Image upgradeImage;
    public new TMP_Text name;
    public TMP_Text details;
    public TMP_Text cost;

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
            if (upgrades[i].type == type && upgrades[i].isUpgraded) {
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
                    upgrades[i].isUpgraded = true;
                    break;
                }
            }
        }

        ApplyUpgradeEffect(upgradeData);
    }

    public void ApplyUpgradeEffect(UpgradeDataSO upgradeData) {
        switch (upgradeData.type) {
            case UpgradeType.HandDrill:
                HandDrillStats.isUnlocked = upgradeData.isUpgraded;
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

    public void ShowDetails(UpgradeDataSO upgradeDataSO) {
        upgradeImage.enabled = true;
        upgradeImage.sprite = upgradeDataSO.sprite;
        name.text = upgradeDataSO.name;
        details.text = upgradeDataSO.details;
        cost.text = upgradeDataSO.isUpgraded ? "SOLD" : upgradeDataSO.cost.FormatNumber();
    }

    public void HideDetails() {
        upgradeImage.enabled = false;
        name.text = "";
        details.text = "";
        cost.text = "";
    }
}

public static class Extensions {
    public static string FormatNumber(this long number) {
        string result = number.ToString();

        int numbersFound = 0;
        int dotNumber = 0;
        for (int i = result.Length - 1; i >= 0; i--) {
            if (int.TryParse($"{result[i]}", out int a)) {
                numbersFound++;
            } else {
                numbersFound = 0;
            }
            //Seems like the 989 was separated correctly, but then that first 9 is added to the left side as well, 
            //1  234 567
            //12.234.567$
            //1234.567$

            //But then this works well, the 910 is separated and no 9 is added before the dot.
            //123 456
            //123.456$
            if (numbersFound >= 4) {
                result = result.Substring(0, result.Length - 3 - (4 * dotNumber)) + "." + result.Substring(i + 1, result.Length - (i + 1));
                numbersFound = 0;
                dotNumber++;
                i++;
            }
        }

        return result;
    }

    public static string FormatNumber(this int number) {
        return ((long)number).FormatNumber();
    }

    public static string FormatNumber(this float number) {
        return ((long)number).FormatNumber();
    }
}