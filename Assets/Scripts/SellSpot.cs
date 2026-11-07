using UnityEngine;

public class SellSpot : MonoBehaviour {

    public Transform sellPoint;
    public float sellDistance = 5;

    private void Update() {
        for (int i = 0; i < Active.valuable.Count; i++) {
            Valuable sellableItem = Active.valuable[i];

            if (transform.position.DistanceSquared(sellableItem.transform.position) < sellDistance * sellDistance) {
                AudioManager.instance.PlayPop();
                Money.instance.AddMoneyBySellableItem(sellableItem.sellAmount);
                SaveSystem.statistics.valuablesSelled++;
                Destroy(sellableItem.gameObject);
                break;
            }
        }
    }
}
