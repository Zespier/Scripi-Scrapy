using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour {

    public float grabDistanceFromCamera = 4f;
    public float launchForce = 10f;
    public float attackSpeed = 2f;
    public float timeToCancelHit = 0.9f;
    public float hitDamage = 1;
    public List<float> hitDamageByLevels = new List<float>() { 1, 1.3f, 1.7f, 2.6f, 5 };
    public float hitArea = 1;
    public List<float> hitAreaByLevels = new List<float>() { 1, 1.3f, 2f, 3f, 4f, 5f };
     
    private Rigidbody grabbedObject;
    private bool _isHitting;
    private float _hitCancelTimer;
    private float _attackSpeedTimer;
    private bool _lastFrameHadGeodeBeingHit;

    public static Interactor instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    protected virtual void SetTimer() {
        _attackSpeedTimer = !_lastFrameHadGeodeBeingHit ? Time.time : _attackSpeedTimer + 1f / attackSpeed;
    }

    private void Update() {
        if (InputManager.GameControls.Character.Interact.WasPressedThisFrame()) {
            Interact();
        }

        if (InputManager.GameControls.Character.Attack.WasPressedThisFrame()) {
            Hit(manualHit: true);
        }

        if (InputManager.GameControls.Character.Jump.WasPressedThisFrame()) {
            StartHitting();
        }

        bool thereIsGeodeInFront = Hit(justCheck: true);

        for (int i = 0; i < Cross.instance.parts.Count; i++) {
            Cross.instance.parts[i].gameObject.SetActive(thereIsGeodeInFront);
        }

        if (_isHitting && Time.time - _attackSpeedTimer >= 1f / attackSpeed) {
            if (Hit()) {
                SaveSystem.statistics.automaticHits++;
                _hitCancelTimer = Time.time;
                return;
            }

            if (Time.time - _hitCancelTimer > timeToCancelHit) {
                _isHitting = false;
            }
        }

        _lastFrameHadGeodeBeingHit = thereIsGeodeInFront;
    }

    private void FixedUpdate() {
        if (grabbedObject != null) {
            grabbedObject.useGravity = false;
            grabbedObject.linearVelocity = Vector3.zero;
            grabbedObject.angularVelocity = Vector3.zero;
            grabbedObject.MovePosition(Camera.main.transform.position + Camera.main.transform.forward * grabDistanceFromCamera);
        }
    }

    public void StartHitting() {
        _isHitting = true;
        _hitCancelTimer = Time.time;
        _lastFrameHadGeodeBeingHit = false;
    }

    public void Interact() {

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

    public bool Hit(bool manualHit = false, bool justCheck = false) {

        RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward);

        for (int i = 0; i < hits.Length; i++) {

            if (hits[i].collider != null) {
                if (hits[i].collider.CompareTag("Player")) { continue; }

                Rigidbody rigidbody = hits[i].collider.GetComponent<Rigidbody>();
                if (rigidbody != null && rigidbody.TryGetComponent(out Geode geode)) {

                    if (justCheck) { return true; }
                    SaveSystem.statistics.Hit(manualHit: manualHit);

                    geode.Hit(hits[i].point, manualHit: manualHit);
                    SetTimer();
                    return true;

                } else {

                    rigidbody = hits[i].collider.transform.parent.GetComponentInChildren<Rigidbody>();
                    if (rigidbody != null && rigidbody.TryGetComponent(out Geode geodee)) {

                        if (justCheck) { return true; }
                        SaveSystem.statistics.Hit(manualHit: manualHit);

                        geodee.Hit(hits[i].point, manualHit: manualHit);
                        SetTimer();
                        return true;
                    }
                }
            }
        }

        return false;
    }
}
