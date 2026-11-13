using System.Collections.Generic;
using UnityEngine;

public class Valuable : GrabableItem {

    public long sellAmount = 13L;

    public override bool Interact() {
        if (state != GrabableState.None) { return false; }

        if (!Inventory.TryAddToInventory(this)) {
            return false;
            /* Feedback of inventory full */
        } else {
            AudioManager.instance.PlayPop();
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            gameObject.gameObject.SetActive(false);
            return true;
        }
    }

    private void OnEnable() {
        Active.AddValuable(this);
        GeneralPool.instance.valuables.Remove(this);
    }

    private void OnDisable() {
        Active.RemoveValuable(this);
        GeneralPool.instance.valuables.Add(this);
    }
}