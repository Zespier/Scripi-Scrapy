using System.Collections.Generic;

public static class Inventory {
    public static List<InventorySlot> slots = new List<InventorySlot>();
    public static int MaxSlots => _slotsPerLevel[MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.ExtraInventorySlots)];

    private static List<int> _slotsPerLevel = new List<int> { 5, 6, 7, 8, 9, 10 };
}

public class InventorySlot {
    public GrabableItem grabableItem;
    public int stack;
    public int MaxStack => _stackPerLevel[MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.ExtraInventorySlots)];

    private List<int> _stackPerLevel = new List<int> { 20, 25, 30, 35, 64 };
}