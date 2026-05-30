using UnityEngine;

public class Drill : MonoBehaviour {

    public Vein vein;
    public float drillSpeed = 0.5f;
    public Transform spawnPoint;
    public float spawnForce = 20f;
    public float spinForce = 1f;

    private float _timer;

    private void Update() {
        if (Time.time - _timer >= 1f / drillSpeed) {
            _timer = _timer + 1f / drillSpeed;
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
    }

    public void ResetTimer() {
        _timer = Time.time;
    }
}
