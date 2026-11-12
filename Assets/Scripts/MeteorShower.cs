using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorShower : MonoBehaviour {

    public MeteorFallingPointList meteorFallingPointList;
    public Transform spawnPoint;
    public float spawnArea = 100f;
    public List<PlayableArea> playableAreas;
    public Transform fullyCoverPoint;
    public float fullyCoverArea = 50f;
    public float spawnForce = 20f;
    public float spinForce = 1f;
    public List<Meteor> allMeteorPrefabs;
    public List<Vector3> occupiedPoints = new(capacity: 1000);
    [Range(0.001f, 1f)]
    public float minDistanceBetweenPoints = 0.5f;
    public float baseSpawnSpeed = 0.15f;

    private float _timer;

    public static MeteorShower instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    private void OnDrawGizmos() {
        for (int i = 0; i < playableAreas.Count; i++) {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(playableAreas[i].center.position, playableAreas[i].area);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(fullyCoverPoint.position, fullyCoverArea);
    }

    private void Start() {
        ResetPointListIndex();
    }

    private void Update() {
        if (Time.time - _timer >= baseSpawnSpeed / MeteorShowerUpgrades.meteorSpeed) {
            _timer = _timer + baseSpawnSpeed / MeteorShowerUpgrades.meteorSpeed;
            SpawnMeteor();
        }
    }

    public void ResetPointListIndex() {
        meteorFallingPointList.currentIndex = 0;
    }

    private void SpawnMeteor() {
        Vector3 spawnPoint = GetSpawnPoint();
        Vector3 impactPoint = GetPointInsidePlayableArea();
        Meteor newMeteor = Instantiate(allMeteorPrefabs[MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.BetterMeteors)], spawnPoint, Quaternion.identity, MeteorContainer.instance.transform);


        newMeteor.state = GrabableState.Falling;
        newMeteor.impactPoint = impactPoint;
        //newMeteor.rb.AddForce(spawnPoint.forward * spawnForce, ForceMode.Impulse);
        //newMeteor.rb.linearVelocity = Vector3.zero;
        //newMeteor.rb.AddTorque(Random.insideUnitSphere * spinForce, ForceMode.Impulse);
        //newMeteor.rb.isKinematic = true;

        SaveSystem.statistics.meteoritesGenerated++;

        if (Time.time - _timer >= baseSpawnSpeed / MeteorShowerUpgrades.meteorSpeed) {
            _timer = _timer + baseSpawnSpeed / MeteorShowerUpgrades.meteorSpeed;
            SpawnMeteor();
        }
    }

    public void ResetTimer() {
        _timer = Time.time;
    }

    public Vector3 GetSpawnPoint() {
        return new Vector3(spawnPoint.position.x + Random.Range(-spawnArea, spawnArea), spawnPoint.position.y + Random.Range(-spawnArea*0.2f, spawnArea*0.2f), spawnPoint.position.z + Random.Range(-spawnArea, spawnArea));
    }

    public Vector3 GetPointInsidePlayableArea() {
        Vector3 randomPoint = default;
        float _minDistanceBetweenPoints = minDistanceBetweenPoints;

        if (meteorFallingPointList.HasNextPoint) {
            Vector3 nextPoint = meteorFallingPointList.GetNextPoint();
            occupiedPoints.Add(nextPoint);
            return nextPoint;

        } else {

            bool insidePlayableArea = false;
            bool validPoint = true;
            int checkCounter = 0;
            int turboCheckCounter = 0;
            do {

                validPoint = true;
                insidePlayableArea = false;
                Vector2 randomPointXZ = new(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

                while (randomPointXZ.sqrMagnitude > 1f) { //Until we get a point inside a unit circle
                    randomPointXZ = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                }

                randomPointXZ *= fullyCoverArea;
                randomPointXZ += new Vector2(fullyCoverPoint.position.x, fullyCoverPoint.position.z);

                randomPoint = new Vector3(randomPointXZ.x, 0, randomPointXZ.y);


                #region CheckInsidePlayableArea
                for (int i = 0; i < playableAreas.Count; i++) {

                    Vector2 centerXZ = new Vector2(playableAreas[i].center.position.x, playableAreas[i].center.position.z);

                    //The formula of a point inside a circle => (x-h)^2 + (y-k)^2 < r^2
                    if ((randomPointXZ - centerXZ).sqrMagnitude <= playableAreas[i].area * playableAreas[i].area) {
                        insidePlayableArea = true;
                        break;
                    }
                }
                #endregion

                #region CheckNot-Repetitive Point
                if (insidePlayableArea) {

                    for (int i = 0; i < occupiedPoints.Count; i++) {
                        Vector2 occupiedPointXZ = new Vector2(occupiedPoints[i].x, occupiedPoints[i].z);

                        //The formula of a point inside a circle => (x-h)^2 + (y-k)^2 < r^2
                        if ((randomPointXZ - occupiedPointXZ).sqrMagnitude <= _minDistanceBetweenPoints * _minDistanceBetweenPoints) {
                            validPoint = false; //It's inside another occupied point
                            break;
                        }
                    }
                }
                #endregion

                turboCheckCounter++;
                checkCounter++;
                if (checkCounter >= 100000) {
                    _minDistanceBetweenPoints /= 2;
                    checkCounter = 0;
                }

            } while ((!insidePlayableArea || !validPoint) && turboCheckCounter < 1000000);
        }

        if (randomPoint != default) {
            meteorFallingPointList.AddPoint(randomPoint);
        }

        occupiedPoints.Add(randomPoint);
        return randomPoint;
    }
}

public static class MeteorShowerUpgrades {
    public static float meteorSpeed = 0.5f;
}