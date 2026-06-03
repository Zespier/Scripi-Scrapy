using UnityEngine;

public class Drill : MonoBehaviour {

    public Vein vein;
    public Transform spawnPoint;
    public float spawnForce = 20f;
    public float spinForce = 1f;

    private float _timer;

    private void Update() {
        if (Time.time - _timer >= 1f / DrillStats.drillSpeed) {
            _timer = _timer + 1f / DrillStats.drillSpeed;
            SpawnGeode();
        }

        if (InputManager.GameControls.Character.Dance.WasPressedThisFrame()) {
            SpawnGeode();
        }
    }

    private void SpawnGeode() {
        Geode newGeode = Instantiate(vein.DecideGeode(), spawnPoint.position, Quaternion.identity);
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