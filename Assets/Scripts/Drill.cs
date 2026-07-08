using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Drill : Interactable {

    public Transform spawnPoint;
    public float spawnForce = 20f;
    public float spinForce = 1f;
    public TMP_Text geodesText;
    public List<Geode> allGeodePrefabs;
    public int maxGeodes = 3;

    private float _timer;
    private int _currentGeodesWaiting;
    private bool _spawning;

    private void Update() {
        geodesText.text = _currentGeodesWaiting.ToString();

        if (Time.time - _timer >= 1f / DrillStats.drillSpeed) {
            _timer = _timer + 1f / DrillStats.drillSpeed;
            AddWaitingGeode();
        }

        if (InputManager.GameControls.Character.Dance.WasPressedThisFrame()) {
            SpawnGeode();
        }
    }

    private void AddWaitingGeode() {
        _currentGeodesWaiting++;
        if (_currentGeodesWaiting > maxGeodes) {
            _currentGeodesWaiting = maxGeodes;
        }
    }

    public override bool Interact() {
        if (_spawning) { return false; }

        StartCoroutine(C_SpawnGeode());
        return true;
    }

    private IEnumerator C_SpawnGeode() {
        _spawning = true;
        _timer = Time.time;

        int maxGeodes = 10;

        float totalTime = Mathf.Lerp(0.5f, 1, _currentGeodesWaiting / maxGeodes);

        float timePerGeode = totalTime / _currentGeodesWaiting;

        float timer = Time.time;

        for (int i = 0; i < _currentGeodesWaiting; i++) {
            timer = Time.time;
            _timer = Time.time;

            SpawnGeode();
            while (Time.time < timer + timePerGeode) {
                yield return null;
            }
        }

        _currentGeodesWaiting = 0;
        _timer = Time.time;

        _spawning = false;
    }

    private void SpawnGeode() {
        Geode newGeode = Instantiate(allGeodePrefabs[MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.BetterGeodes)], spawnPoint.position, Quaternion.identity);
        newGeode.newParent = transform;
        newGeode.rb.AddForce(spawnPoint.forward * spawnForce, ForceMode.Impulse);
        newGeode.rb.AddTorque(Random.insideUnitSphere * spinForce, ForceMode.Impulse);

        AudioManager.instance.PlayPop();
        SaveSystem.statistics.geodesGenerated++;
    }

    public void ResetTimer() {
        _timer = Time.time;
    }
}

public static class DrillStats {
    public static float drillSpeed = 0.5f;
}