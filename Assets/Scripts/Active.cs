using System.Collections.Generic;
using UnityEngine;

public static class Active {

    public static List<Valuable> valuable = new(capacity: 200);

    public static void AddValuable(Valuable valuable) {
        if (!Active.valuable.Contains(valuable)) {
            Active.valuable.Add(valuable);
        }
    }

    public static void RemoveValuable(Valuable valuable) {
        if (Active.valuable.Contains(valuable)) {
            Active.valuable.Remove(valuable);
        }
    }

    public static List<Meteor> meteor = new(capacity: 200);

    public static void AddMeteor(Meteor meteor) {
        if (!Active.meteor.Contains(meteor)) {
            Active.meteor.Add(meteor);
        }
    }

    public static void RemoveMeteor(Meteor meteor) {
        if (Active.meteor.Contains(meteor)) {
            Active.meteor.Remove(meteor);
        }
    }
}
