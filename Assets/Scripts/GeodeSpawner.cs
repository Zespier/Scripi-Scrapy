using UnityEngine;

public class GeodeSpawner : MonoBehaviour {

    public Transform spawnPoint;
    public Geode geodePrefab;

    private void Update() {
        if (InputManager.GameControls.Character.Dance.WasPressedThisFrame()) {
            SpawnGeode();
        }
    }

    private void SpawnGeode() {
        Instantiate(geodePrefab, spawnPoint.position, Quaternion.identity).newParent = transform;
    }
}
