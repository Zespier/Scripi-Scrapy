using UnityEngine;
using UnityEngine.InputSystem;

public class GeodeSpawner : MonoBehaviour {

    public Transform spawnPoint;
    public Geode geodePrefab;

    private void OnEnable() {
        InputManager.OnCharacterAttack += SpawnGeode;
    }

    private void OnDisable() {
        InputManager.OnCharacterAttack -= SpawnGeode;
    }

    private void SpawnGeode(InputAction.CallbackContext context) {
        if (context.phase != InputActionPhase.Started) { return; }

        Instantiate(geodePrefab, spawnPoint.position, Quaternion.identity).newParent = transform;
    }
}
