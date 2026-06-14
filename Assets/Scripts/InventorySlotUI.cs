using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour {

    public Image image;
    public TMP_Text stack;
    public InventorySlot inventorySlot;

    private void Update() {
        image.sprite = inventorySlot.grabableItem.itemSprite;
        stack.text = inventorySlot.stack.ToString();
    }
}