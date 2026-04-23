using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour {

    private void OnEnable() {
        InputManager.OnCharacterInteract += Interact;
    }

    private void OnDisable() {
        InputManager.OnCharacterInteract -= Interact;
    }

    public void Interact(InputAction.CallbackContext context) {
        RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward);

        for (int i = 0; i < hits.Length; i++) {
            if (hits[i].collider != null) {
                Geode geode = hits[i].collider.transform.parent.GetComponentInChildren<Geode>();
                if (geode != null) {
                    geode.Hit();
                    break;
                }
            }
        }
    }
}
