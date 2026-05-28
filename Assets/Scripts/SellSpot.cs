using UnityEngine;

public class SellSpot : MonoBehaviour {

    public Transform sellPoint;
    public float sellDistance = 5;

    private void Update() {
        for (int i = 0; i < SellableItem.activeSellableItems.Count; i++) {
            SellableItem sellableItem = SellableItem.activeSellableItems[i];

            if (Vector3.Distance(transform.position, sellableItem.transform.position) < sellDistance) {
                Money.instance.AddMoney(sellableItem.sellAmount);
                Destroy(sellableItem.gameObject);
            }
        }
    }
}
