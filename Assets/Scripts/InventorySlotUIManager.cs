using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlotUIManager : MonoBehaviour {

    public CanvasGroup canvasGroup;
    public List<InventorySlotUI> inventorySlotsUI = new();
    public float slotsWidth = 100;

    private void Awake() {
        ActiveCanvasGroup(true);
    }

    private void Update() {
        for (int i = 0; i < Inventory.slots.Count; i++) {
            inventorySlotsUI[i].gameObject.SetActive(true);
            inventorySlotsUI[i].image.sprite = Inventory.slots[i].slotItems[0].itemSprite;
            inventorySlotsUI[i].stack.text = $"x{Inventory.slots[i].slotItems.Count.FormatNumber()}";

            inventorySlotsUI[i].rectTransform.localPosition = (slotsWidth * i * Vector3.right) - ((slotsWidth * Inventory.slots.Count) / 2f * Vector3.right);
        }

        for (int i = Inventory.slots.Count; i < inventorySlotsUI.Count; i++) {
            inventorySlotsUI[i].gameObject.SetActive(false);
        }
    }

    private void ActiveCanvasGroup(bool active) {
        canvasGroup.alpha = active ? 1.0f : 0;
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }
}
