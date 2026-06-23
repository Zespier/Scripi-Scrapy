using System.Collections.Generic;
using UnityEngine;

public class SellableItem : GrabableItem {

    public long sellAmount = 13L;
    public bool insideGeode;
    public bool canBeSelled = true;

    private void OnEnable() {
        Active.AddSellableItem(this);
    }

    private void OnDisable() {
        Active.RemoveSellableItem(this);
    }
}