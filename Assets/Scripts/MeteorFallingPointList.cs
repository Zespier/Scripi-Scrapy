using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeteorFallingPointList", menuName = "Falling Point List")]
public class MeteorFallingPointList : ScriptableObject {

    public List<Vector3> validConsecutivePoints = new();
    public int currentIndex;
    [Range(2, 10000)]
    public int pointsToCloseLoop = 1000;

    public bool HasNextPoint => HasNexttPoint();

    public bool HasNexttPoint() {

        bool hasNextPoint = currentIndex < validConsecutivePoints.Count;

        if (!hasNextPoint && validConsecutivePoints.Count >= pointsToCloseLoop) {
            currentIndex = 0;
            return true;
        }

        return hasNextPoint;
    }

    public Vector3 GetNextPoint() {
        if (!HasNextPoint) {
            return Vector3.zero;
        }

        return validConsecutivePoints[currentIndex++];
    }

    public void AddPoint(Vector3 point) {
        validConsecutivePoints.Add(point);
    }

    public void ResetIndex() {
        currentIndex = 0;
    }
}
