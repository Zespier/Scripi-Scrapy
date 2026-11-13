using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour {

    public Transform grabPoint;
    public float interactionDistance = 6;
    public float grabDistanceFromCamera = 4f;
    public float launchForce = 10f;
    public float attackSpeed = 2f;
    public float timeToCancelHit = 0.9f;
    public List<int> hitDamageByLevelss = new List<int>() { 1, 4, 9, 13, 18, 21 };
    public List<float> hitAreaByLevels = new List<float>() { 1, 1.3f, 2f, 3f, 4f, 5f };
    public Vector2 launchSpeedRange = new Vector2(2, 10);
    public float timeToReachMaxLaunchSpeed = 4f;
    public int slotSelected = 0;
    public Animator aspiradoraAnimator;

    private bool _isHitting;
    private float _hitCancelTimer;
    private float _attackSpeedTimer;
    private bool _lastFrameHadGeodeBeingHit;
    private bool _lastFrameWasLaunching;
    private float _launchTimer;
    private float _timeLaunching;
    private RaycastHit[] _hits = new RaycastHit[10];
    private Camera mainCamera;

    //Primer golpe es fuerte, los demás solo 1
    public int FirstHitDamage => hitDamageByLevelss[MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.HitDamage)];
    public float HitArea => hitAreaByLevels[MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.HitArea)];
    public float LaunchSpeed => Mathf.Lerp(launchSpeedRange.x, launchSpeedRange.y, _timeLaunching / timeToReachMaxLaunchSpeed);

    public static Interactor instance;
    private void Awake() {
        if (!instance) { instance = this; }
        mainCamera = Camera.main;
    }

    private void SetTimer() {
        _attackSpeedTimer = !_lastFrameHadGeodeBeingHit ? Time.time : _attackSpeedTimer + 1f / attackSpeed;
    }

    private void SetLaunchTimer() {
        _launchTimer = !_lastFrameWasLaunching ? Time.time : _launchTimer + 1f / LaunchSpeed;
    }

    private void Update() {

        #region Cross Activation
        //First active the cross if something is in front;
        Interactable interactableInFront = GetItemInFront();
        Meteor meteorInFront = GetMeteorInFront();
        for (int i = 0; i < Cross.instance.parts.Count; i++) {
            Cross.instance.parts[i].gameObject.SetActive(interactableInFront != null || meteorInFront != null);
        }
        #endregion

        #region Launching Counters
        bool _launchIsPressed = InputManager.GameControls.Character.Attack.IsPressed();
        if (_launchIsPressed) {
            _timeLaunching += Time.deltaTime;
        }

        bool _launchSuccessfull = false;
        if (_launchIsPressed && _lastFrameWasLaunching && Time.time > _launchTimer + 1 / LaunchSpeed) { //This is the "automatic" launch
            if (Inventory.slots.Count > 0) {
                Launch();
                _launchIsPressed = true;
            }
        }
        #endregion

        #region Launch or Hit   
        if (InputManager.GameControls.Character.Attack.WasPressedThisFrame()) {
            _timeLaunching = 0;

            if (_launchSuccessfull) {
                //If we launched an item this frame, then do nothing if it happens that it was the same frame that the input was pressed


            } else {
                //In case we didnt launch but the input was pressed
                if (Inventory.slots.Count > 0) { //This is the "manual" launch
                    Launch();

                    //TODO: This is if I want to hit Grabables for some reason

                    //If there is no items on the inventory, hit whatever is in front
                } /*else if (interactableInFront != null && interactableInFront is Meteor geode) { 
                    Hit(geode, geode.pointOfInteraction, manualHit: true);
                }*/

                else if (meteorInFront != null) {
                    Hit(meteorInFront, meteorInFront.hitPoint, manualHit: true);
                }
            }
        }
        #endregion

        #region Interact
        bool interactedWithSomething = false;
        if (InputManager.GameControls.Character.Interact.WasPressedThisFrame() && interactableInFront != null) {
            interactedWithSomething = Interact(interactableInFront);
        }
        #endregion

        #region Sucking
        //Sucking only happens when nothing on the cross can be interacted
        if (!interactedWithSomething && InputManager.GameControls.Character.Interact.WasPressedThisFrame()) {
            Aspiradora.instance.StartSucking();
        }

        if (InputManager.GameControls.Character.Interact.WasReleasedThisFrame()) {
            Aspiradora.instance.StopSucking();
        }
        #endregion

        #region Automatic Hitting
        if (InputManager.GameControls.Character.Jump.WasPressedThisFrame()) {
            StartHitting();
        }

        if (_isHitting && Time.time - _attackSpeedTimer >= 1f / attackSpeed) {

            //if (interactableInFront != null && interactableInFront is Meteor geode) { //Check if the geode got hit
            //    Hit(geode, geode.pointOfInteraction);
            //    SaveSystem.statistics.automaticHits++;
            //    _hitCancelTimer = Time.time;
            //    return;
            //}

            if (Time.time - _hitCancelTimer > timeToCancelHit) {
                _isHitting = false;
            }
        }
        #endregion

        _lastFrameHadGeodeBeingHit = interactableInFront;
        _lastFrameWasLaunching = _launchIsPressed;
    }

    public void StartHitting() {
        _isHitting = true;
        _hitCancelTimer = Time.time;
        _lastFrameHadGeodeBeingHit = false;
    }

    public void Launch() {
        SetLaunchTimer();

        aspiradoraAnimator.Play("Shoot");

        slotSelected = Inventory.slots.Count - 1; //TODO: Change to input management of the player

        //This launches the last item saved
        InventoryValuable inventoryValuable = Inventory.slots[slotSelected].items[^1];
        Valuable valuable = GeneralPool.instance.GetValuableFromPool(inventoryValuable.valuablePrefabReference.type);

        valuable.gameObject.SetActive(true);
        Inventory.RemoveItemFromInventory(valuable);

        grabPoint.transform.localPosition = new Vector3(grabPoint.localPosition.x, grabPoint.localPosition.y, grabDistanceFromCamera);
        valuable.transform.position = grabPoint.position;

        valuable.rb.isKinematic = false;
        valuable.rb.AddForce(mainCamera.transform.forward * launchForce, ForceMode.Impulse);
    }

    public bool Interact(Interactable interactable) {
        return interactable.Interact();
    }

    public Interactable GetItemInFront() {
        int totalHits = Physics.RaycastNonAlloc(Camera.main.transform.position, Camera.main.transform.forward, _hits, interactionDistance);

        for (int i = totalHits; i < _hits.Length; i++) {
            _hits[i] = default;
        }

        for (int i = 0; i < totalHits; i++) {
            if (_hits[i].collider.CompareTag("Player")) { continue; }

            if (_hits[i].collider.CompareTag("Valuable")) {
                //Valuables have their collider nside 2 children
                if (_hits[i].collider.transform.parent.parent.TryGetComponent(out GrabableItem grabable)) {

                    if (grabable.state != GrabableState.None) { continue; }

                    grabable.pointOfInteraction = _hits[i].point;

                    //With default interactables we only return it
                    return grabable;
                }
            }
        }

        return null;
    }

    public Meteor GetMeteorInFront() {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        for (int i = 0; i < Active.meteors.Count; i++) {
            Meteor meteor = Active.meteors[i];

            if (meteor.CheckRay(ray, out Vector3 hitPoint)) {
                meteor.hitPoint = hitPoint;
                return meteor;
            }
        }

        return null;
    }

    public void Hit(Meteor meteor, Vector3 hitPoint, bool manualHit = false) {
        SaveSystem.statistics.Hit(manualHit: manualHit);
        meteor.Hit(hitPoint, manualHit: manualHit);
        SetTimer();
    }
}
