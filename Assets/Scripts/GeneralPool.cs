using System.Collections.Generic;
using UnityEngine;

public class GeneralPool : MonoBehaviour {

    public List<Valuable> valuablePrefabs;
    public List<Valuable> valuables = new();

    public static GeneralPool instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    public Valuable GetValuableFromPool(ItemType type) {

        if (valuables.Count > 0) {
            Valuable valuable = valuables.Find(v => v.type == type);
            valuables.Remove(valuable);
            valuable.gameObject.SetActive(true);
            return valuable;


        } else {
            return Instantiate(valuablePrefabs.Find(v => v.type == type), ValuableContainer.instance.transform);
        }
    }
}