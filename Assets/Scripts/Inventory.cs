using UnityEngine;
using System.Collections.Generic;

public static class Inventory {
    public static List<InventorySlot> slots = new List<InventorySlot>();
    public static int MaxSlots => _slotsPerLevel[MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.ExtraInventorySlots)];

    private static List<int> _slotsPerLevel = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

    public static Valuable SearchPrefabOfSameType(Valuable valuable) {

        List<Valuable> valuablePrefabs = ValuableContainer.instance.valuablePrefabs;

        for (int i = 0; i < valuablePrefabs.Count; i++) {
            if (valuablePrefabs[i].type == valuable.type) {
                return valuablePrefabs[i];
            }
        }

        return null;
    }

    public static bool TryAddToInventory(Valuable valuable) {

        Valuable valuablePrefab = SearchPrefabOfSameType(valuable);

        //The translation to inventory varables of the valuable
        InventoryValuable inventoryValuable = new InventoryValuable {
            valuablePrefabReference = valuablePrefab,
            stackWeight = valuable.stackWeight
        };

        if (valuablePrefab == null) {
            Debug.LogError("Valuable is not on the ValuableContainer prefab list");
        }

        foreach (var slot in slots) {

            for (int j = 0; j < slot.items.Count; j++) {
                //If the reference of the valuable on the inventory is the same that the method found with this valuable, then it's the same type
                if (slot.items[j].valuablePrefabReference == valuablePrefab) {
                    //Vamos por buen camino, ahora vemos si cabe

                    if (slot.Stack + valuable.stackWeight <= slot.MaxStack) {

                        slot.items.Add(inventoryValuable);
                        return true;
                    }
                }
            }
        }

        //If we reached this part, it means that the item has no slot to fit in, let's try to add a new empty slot
        if (slots.Count >= MaxSlots) {
            Debug.Log("Inventory slots are maxed");
            return false;

        } else {
            slots.Add(new InventorySlot());
            slots[^1].items.Add(inventoryValuable);
            return true;
        }
    }

    public static void RemoveItemFromInventory(Valuable valuable) {

        Valuable valuablePrefab = SearchPrefabOfSameType(valuable);

        int index = 0;
        foreach (var slot in slots) {

            for (int j = 0; j < slot.items.Count; j++) {

                if (slot.items[j].valuablePrefabReference == valuablePrefab) {

                    slot.items.RemoveAt(j);
                    if (slot.Stack == 0) {
                        slots.RemoveAt(index);
                    }
                    return;
                }
            }
            index++;
        }
    }
}

public class InventorySlot {
    public List<InventoryValuable> items = new();
    public int Stack => CalculateStack();
    public int MaxStack => _stackPerLevel[MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.ExtraInventorySlots)];

    private List<int> _stackPerLevel = new List<int> { 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 64 };

    public int CalculateStack() {
        int totalStackWeight = 0;

        for (int i = 0; i < items.Count; i++) {
            totalStackWeight += items[i].stackWeight;
        }

        return totalStackWeight;
    }
}
