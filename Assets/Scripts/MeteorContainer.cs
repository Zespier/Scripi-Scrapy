using System.Collections.Generic;
using UnityEngine;

public class MeteorContainer : MonoBehaviour {

    public List<Meteor> meteorPrefabs;

    public static MeteorContainer instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }
}
