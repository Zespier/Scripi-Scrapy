using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuUpgrades : MonoBehaviour {

    public CanvasGroup canvasGroup;
    public TMP_Text hitDamageCostText;
    public TMP_Text drillSpeedCostText;
    public TMP_Text hitAreaCostText;

    public List<UpgradeData> upgrades = new List<UpgradeData> {
        new UpgradeData("HitDamage_1",UpgradeType.HitDamage, false),
        new UpgradeData("DrillSpeed_1",UpgradeType.DrillSpeed, false),
        new UpgradeData("HitArea_1",UpgradeType.HitArea, false),
        new UpgradeData("HandDrill",UpgradeType.HandDrill, false),
        new UpgradeData("HandDrillSpeed_1",UpgradeType.HandDrillSpeed, false),
    };

    public bool pressedP;
    public bool pressedO;

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
            Money.instance.moneyText.text = $"{Money.instance.currentMoney.ToString("F0")}$";
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

    public void UpgradeHit1Damage() {
        if (Money.instance.CanBuy(UpgradesCost.hitDamage1Cost)) {
            Money.instance.SpendMoney(UpgradesCost.hitDamage1Cost);

            for (int i = 0; i < upgrades.Count; i++) {
                if (upgrades[i].id == "HitDamage_1") {
                    upgrades[i].isUnlocked = true;
                    break;
                }
            }

            Interactor.instance.hitDamage = Interactor.instance.hitDamageByLevels[GetUpgradeLevel(UpgradeType.HitDamage)];

            Money.instance.moneyText.text = $"{Money.instance.currentMoney.ToString("F0")}$";
        }
    }

    public void UpgradeDrillSpeed() {
        if (Money.instance.CanBuy(UpgradesCost.drillSpeed1Cost)) {
            Money.instance.SpendMoney(UpgradesCost.drillSpeed1Cost);

            for (int i = 0; i < upgrades.Count; i++) {
                if (upgrades[i].id == "HitDamage_1") {
                    upgrades[i].isUnlocked = true;
                    break;
                }
            }

            DrillStats.drillSpeed *= 1.5f;

            Money.instance.moneyText.text = $"{Money.instance.currentMoney.ToString("F0")}$";
        }
    }

    public void UpgradeHitArea() {
        if (Money.instance.CanBuy(hitAreaCostt)) {
            Money.instance.SpendMoney(hitAreaCostt);

            if (hitAreaLevel >= Interactor.instance.hitAreaByLevels.Count) {
                Interactor.instance.hitArea = float.MaxValue;

            } else {
                Interactor.instance.hitArea = Interactor.instance.hitAreaByLevels[hitAreaLevel];
            }

            hitAreaLevel++;
            hitAreaCostt = Mathf.Pow(hitAreaCostt, 1.05f);
            hitAreaCostText.text = hitAreaCostt.ToString("F0") + "$";
            Money.instance.moneyText.text = $"{Money.instance.currentMoney.ToString("F0")}$";
        }
    }

    #region GameReset
    public void CheckGameReset() {

        if (pressedP && pressedO && Keyboard.current.iKey.wasPressedThisFrame) {
            ResetGame();
        } else {
            pressedP = false;
            pressedO = false;
        }
        if (pressedP && Keyboard.current.oKey.wasPressedThisFrame) {
            pressedO = true;
        } else {
            pressedP = false;
            pressedO = false;
        }
        if (Keyboard.current.pKey.wasPressedThisFrame) {
            pressedP = true;
        } else {
            pressedP = false;
        }
    }

    public void ResetGame() {
        GameManager.instance.saveOnDestroy = false;
        SaveSystem.DeleteSaveFile();

        SceneManager.LoadScene(0);
    }

    #endregion
}

public static class UpgradesCost {
    public static readonly int hitDamage1Cost = 10;
    public static readonly int drillSpeed1Cost = 30;
    public static readonly int hitArea1Cost = 50;
}