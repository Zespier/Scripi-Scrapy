using System.Collections.Generic;
using UnityEngine;

public class ValuableContainer : MonoBehaviour {

    public List<Valuable> valuablePrefabs;

    public static ValuableContainer instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }
}
