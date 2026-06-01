using System.Collections.Generic;
using UnityEngine;

public class SellableItem : MonoBehaviour {

    public static List<SellableItem> activeSellableItems = new(capacity: 200);

    public float sellAmount = 13f;
    public bool insideGeode;

    private void OnEnable() {
        AddSellableItem(this);
    }

    private void OnDisable() {
        RemoveSellableItem(this);
    }

    public void AddSellableItem(SellableItem sellableItem) {
        if (!activeSellableItems.Contains(sellableItem)) {
            activeSellableItems.Add(sellableItem);
        }
    }

    public void RemoveSellableItem(SellableItem sellableItem) {
        if (activeSellableItems.Contains(sellableItem)) {
            activeSellableItems.Remove(sellableItem);
        }
    }
}
