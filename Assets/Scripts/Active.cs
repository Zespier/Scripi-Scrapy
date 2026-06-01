using System.Collections.Generic;
using UnityEngine;

public static class Active {

    public static List<SellableItem> sellableItems = new(capacity: 200);

    public static void AddSellableItem(SellableItem sellableItem) {
        if (!sellableItems.Contains(sellableItem)) {
            sellableItems.Add(sellableItem);
        }
    }

    public static void RemoveSellableItem(SellableItem sellableItem) {
        if (sellableItems.Contains(sellableItem)) {
            sellableItems.Remove(sellableItem);
        }
    }

    public static List<Geode> geodes = new(capacity: 200);

    public static void AddGeode(Geode geode) {
        if (!geodes.Contains(geode)) {
            geodes.Add(geode);
        }
    }

    public static void RemoveGeode(Geode geode) {
        if (geodes.Contains(geode)) {
            geodes.Remove(geode);
        }
    }
}
