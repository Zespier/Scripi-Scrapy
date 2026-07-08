using UnityEngine;

public class SellSpot : MonoBehaviour {

    public Transform sellPoint;
    public float sellDistance = 5;

    private void Update() {
        for (int i = 0; i < Active.sellableItems.Count; i++) {
            SellableItem sellableItem = Active.sellableItems[i];

            if (!sellableItem.canBeSelled) { continue; }

            if (transform.position.DistanceSquared(sellableItem.transform.position) < sellDistance * sellDistance) {
                AudioManager.instance.PlayPop();
                Money.instance.AddMoneyBySellableItem(sellableItem.sellAmount);
                SaveSystem.statistics.itemsSelled++;
                Destroy(sellableItem.gameObject);
                break;
            }
        }

        //TODO: This makes bad fps a nightmare to sell, but it shouldn't reach that point, right?
    }
}
