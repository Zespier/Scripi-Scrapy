using System.Collections.Generic;
using UnityEngine;

public static class Active {

    #region Valuables

    public static List<Valuable> valuables = new(capacity: 200);

    public static void AddValuable(Valuable valuable) {
        if (!valuables.Contains(valuable)) {
            valuables.Add(valuable);
        }
    }

    public static void RemoveValuable(Valuable valuable) {
        if (valuables.Contains(valuable)) {
            valuables.Remove(valuable);
        }
    }

    #endregion

    #region Meteors

    public static List<Meteor> meteors = new(capacity: 200);

    public static void AddMeteor(Meteor meteor) {
        if (!meteors.Contains(meteor)) {
            meteors.Add(meteor);
        }
    }

    public static void RemoveMeteor(Meteor meteor) {
        if (meteors.Contains(meteor)) {
            meteors.Remove(meteor);
        }
    }

    #endregion
}
