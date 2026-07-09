using UnityEngine;
using System.Collections.Generic;

public static class Inventory {
    public static List<InventorySlot> slots = new List<InventorySlot>();
    public static int MaxSlots => _slotsPerLevel[MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.ExtraInventorySlots)];

    private static List<int> _slotsPerLevel = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

    public static bool TryAddToInventory(GrabableItem grabableItem) {

        for (int i = 0; i < slots.Count; i++) {
            for (int j = 0; j < slots[i].slotItems.Count; j++) {
                if (slots[i].slotItems[j].type == grabableItem.type) {
                    //Vamos por buen camino, ahora vemos si cabe

                    if (slots[i].Stack + grabableItem.stackWeight <= slots[i].MaxStack) {
                        slots[i].slotItems.Add(grabableItem);
                        if (grabableItem is SellableItem sellableItem) {
                            sellableItem.canBeSelled = false;
                        }
                        return true;
                    } else {
                        //Debug.LogError("Can't fit in this slot, try with another one");
                    }
                }
            }
        }

        //If we reached this part, it means that the item has no slot to fit in, let's try to add a new empty slot
        if (slots.Count >= MaxSlots) {
            Debug.LogError("Inventory slots are maxed");
            return false;

        } else {
            slots.Add(new InventorySlot());
            slots[^1].slotItems.Add(grabableItem);
            if (grabableItem is SellableItem sellableItem) {
                sellableItem.canBeSelled = false;
            }
            return true;
        }
    }

    public static void RemoveItemFromInventory(GrabableItem grabableItem) {
        for (int i = 0; i < slots.Count; i++) {
            for (int j = 0; j < slots[i].slotItems.Count; j++) {
                if (slots[i].slotItems[j] == grabableItem) {
                    slots[i].slotItems.Remove(grabableItem);
                    if (slots[i].Stack == 0) {
                        slots.RemoveAt(i);
                    }
                    return;
                }
            }
        }
    }
}

public class InventorySlot {
    public List<GrabableItem> slotItems = new();
    public int Stack => CalculateStack();
    public int MaxStack => _stackPerLevel[MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.ExtraInventorySlots)];

    private List<int> _stackPerLevel = new List<int> { 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 64 };

    public int CalculateStack() {
        int totalStackWeight = 0;

        for (int i = 0; i < slotItems.Count; i++) {
            totalStackWeight += slotItems[i].stackWeight;
        }

        return totalStackWeight;
    }
}
