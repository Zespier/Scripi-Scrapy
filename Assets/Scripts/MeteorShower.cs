using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorShower : MonoBehaviour {

    public Transform spawnPoint;
    public float spawnForce = 20f;
    public float spinForce = 1f;
    public List<Meteor> allMeteorPrefabs;

    private float _timer;

    private void Update() {
        if (Time.time - _timer >= 1f / MeteorShowerUpgrades.meteorSpeed) {
            _timer = _timer + 1f / MeteorShowerUpgrades.meteorSpeed;
            SpawnMeteor();
        }
    }

    private void SpawnMeteor() {
        Meteor newGeode = Instantiate(allMeteorPrefabs[MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.BetterMeteors)], spawnPoint.position, Quaternion.identity, MeteorContainer.instance.transform);
        newGeode.rb.AddForce(spawnPoint.forward * spawnForce, ForceMode.Impulse);
        newGeode.rb.AddTorque(Random.insideUnitSphere * spinForce, ForceMode.Impulse);

        SaveSystem.statistics.meteoritesGenerated++;
    }

    public void ResetTimer() {
        _timer = Time.time;
    }
}

public static class MeteorShowerUpgrades {
    public static float meteorSpeed = 0.5f;
}