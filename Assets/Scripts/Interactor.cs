using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour {

    public float grabDistanceFromCamera = 4f;
    public float launchForce = 10f;

    private Rigidbody grabbedObject;

    private void FixedUpdate() {
        if (grabbedObject != null) {
            grabbedObject.useGravity = false;
            grabbedObject.MovePosition(Camera.main.transform.position + Camera.main.transform.forward * grabDistanceFromCamera);
        }
    }

    private void OnEnable() {
        InputManager.OnCharacterInteract += Interact;
    }

    private void OnDisable() {
        InputManager.OnCharacterInteract -= Interact;
    }

    public void Interact(InputAction.CallbackContext context) {
        if (context.phase != InputActionPhase.Started) { return; }

        RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward);

        for (int i = 0; i < hits.Length; i++) {

            if (hits[i].collider != null) {
                if (hits[i].collider.CompareTag("Player")) { continue; }

                Rigidbody rigidbody = hits[i].collider.GetComponent<Rigidbody>();
                if (rigidbody != null) {

                    if (grabbedObject == rigidbody) {
                        grabbedObject.useGravity = true;
                        grabbedObject = null;
                        rigidbody.AddForce(Camera.main.transform.forward * launchForce, ForceMode.Impulse);


                    } else {
                        grabbedObject = rigidbody;
                    }
                    //geode.Hit();
                    break;
                } else {

                    rigidbody = hits[i].collider.transform.parent.GetComponentInChildren<Rigidbody>();
                    if (rigidbody != null) {

                        Geode geode = rigidbody.GetComponent<Geode>();
                        if (geode != null) {
                            if (grabbedObject == geode.rb) {
                                grabbedObject.useGravity = true;
                                grabbedObject = null;
                                geode.rb.AddForce(Camera.main.transform.forward * launchForce, ForceMode.Impulse);
                                geode.isLaunched = true;


                            } else {
                                grabbedObject = geode.rb;
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
}
