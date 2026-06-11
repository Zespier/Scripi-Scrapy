using System.Collections.Generic;
using UnityEngine;

public class SellableItem : GrabableItem {

    public float sellAmount = 13f;
    public bool insideGeode;

    private void OnEnable() {
        Active.AddSellableItem(this);
    }

    private void OnDisable() {
        Active.RemoveSellableItem(this);
    }
}