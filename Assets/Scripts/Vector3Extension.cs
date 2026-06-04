using UnityEngine;

public static class Vector3Extension {
    public static float DistanceSquared(this Vector3 v1, Vector3 v2) {
        return (v1 - v2).sqrMagnitude;
    }
}
