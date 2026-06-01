using TMPro;
using UnityEngine;

public class MenuUpgrades : MonoBehaviour {

    public CanvasGroup canvasGroup;
    public TMP_Text upgradeCostText;
    public float hitDamageCost = 10f;

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
    }

    public void ActiveCanvasGroup(bool active) {
        canvasGroup.alpha = active ? 1 : 0;
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }

    public void UpgradeHitDamage() {
        if (Money.instance.CanBuy(hitDamageCost)) {
            Money.instance.SpendMoney(hitDamageCost);
            Interactor.instance.hitDamage++;
            hitDamageCost = Mathf.Pow(hitDamageCost, 1.05f);
            upgradeCostText.text = hitDamageCost.ToString("F0") + "$";
            Money.instance.moneyText.text = $"{Money.instance.currentMoney.ToString("F0")}$";
        }
    }
}
