using UnityEngine;

public class SellSpot : MonoBehaviour {

    public Transform sellPoint;
    public float sellDistance = 5;

    private void Update() {
        for (int i = 0; i < Active.sellableItems.Count; i++) {
            SellableItem sellableItem = Active.sellableItems[i];

            if (sellableItem.insideGeode) { continue; }

            if (Vector3.Distance(transform.position, sellableItem.transform.position) < sellDistance) {
                AudioManager.instance.PlayPop();
                Money.instance.AddMoney(sellableItem.sellAmount);
                Destroy(sellableItem.gameObject);
            }
        }
    }
}
