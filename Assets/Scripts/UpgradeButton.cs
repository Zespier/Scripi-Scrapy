using UnityEngine;
using UnityEngine.EventSystems;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public RectTransform rectTransform;
    public UpgradeDataSO upgradeData;

    public void OnPointerClick(PointerEventData eventData) {
        MenuUpgrades.instance.Upgrade(upgradeData);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        MenuUpgrades.instance.outline.position = rectTransform.position;
        MenuUpgrades.instance.ShowDetails(upgradeData);
    }

    public void OnPointerExit(PointerEventData eventData) {
        MenuUpgrades.instance.outline.position = new Vector3(1000000000, 0, 0);
        MenuUpgrades.instance.HideDetails();
    }
}
