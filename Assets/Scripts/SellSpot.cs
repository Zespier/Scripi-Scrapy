using UnityEngine;

public class SellSpot : MonoBehaviour {

    public Transform sellPoint;
    public float sellDistance = 5;

    private void Update() {
        for (int i = 0; i < Active.valuables.Count; i++) {
            Valuable valuable = Active.valuables[i];

            if (transform.position.DistanceSquared(valuable.transform.position) < sellDistance * sellDistance) {
                AudioManager.instance.PlayPop();
                Money.instance.AddMoneyBySellableItem(valuable.sellAmount);
                SaveSystem.statistics.valuablesSelled++;
                valuable.gameObject.SetActive(false);
                break;
            }
        }
    }
}
