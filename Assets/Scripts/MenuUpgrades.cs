using TMPro;
using UnityEngine;

public class MenuUpgrades : MonoBehaviour {

    public CanvasGroup canvasGroup;
    public TMP_Text hitDamageCostText;
    public TMP_Text drillSpeedCostText;
    public TMP_Text hitAreaCostText;
    public int hitLevel = 1;
    public float hitDamageCost = 10f;
    public float drillSpeedCost = 30f;
    public int hitAreaLevel = 1;
    public float hitAreaCostt = 50f;

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
    }

    public void ActiveCanvasGroup(bool active) {
        canvasGroup.alpha = active ? 1 : 0;
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }

    public void UpgradeHitDamage() {
        if (Money.instance.CanBuy(hitDamageCost)) {
            Money.instance.SpendMoney(hitDamageCost);

            if (hitLevel >= Interactor.instance.hitDamageByLevels.Count) {
                Interactor.instance.hitDamage = float.MaxValue;

            } else {
                Interactor.instance.hitDamage = Interactor.instance.hitDamageByLevels[hitLevel];
            }

            hitLevel++;
            hitDamageCost = Mathf.Pow(hitDamageCost, 1.05f);
            hitDamageCostText.text = hitDamageCost.ToString("F0") + "$";
            Money.instance.moneyText.text = $"{Money.instance.currentMoney.ToString("F0")}$";
        }
    }

    public void UpgradeDrillSpeed() {
        if (Money.instance.CanBuy(drillSpeedCost)) {
            Money.instance.SpendMoney(drillSpeedCost);
            DrillStats.drillSpeed *= 1.5f;
            drillSpeedCost = Mathf.Pow(drillSpeedCost, 1.08f);
            drillSpeedCostText.text = drillSpeedCost.ToString("F0") + "$";
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
}
