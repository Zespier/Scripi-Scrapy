using System.Collections.Generic;
using UnityEngine;

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

    private bool _isHitting;
    private float _hitCancelTimer;
    private float _attackSpeedTimer;
    private bool _lastFrameHadGeodeBeingHit;
    private bool _lastFrameWasLaunching;
    private float _launchTimer;
    private float _timeLaunching;
    private RaycastHit[] _hits = new RaycastHit[10];
    private GrabableItem _lastVisualItem;
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

        //First active the cross if something is in front;
        Interactable interactableInFront = GetItemInFront();
        for (int i = 0; i < Cross.instance.parts.Count; i++) {
            Cross.instance.parts[i].gameObject.SetActive(interactableInFront != null);
        }

        bool _launchIsPressed = InputManager.GameControls.Character.Attack.IsPressed();
        if (_launchIsPressed) {
            _timeLaunching += Time.deltaTime;
        }

        bool _launchSuccessfull = false;
        if (_launchIsPressed && Time.time > _launchTimer + 1 / LaunchSpeed) { //This is the "automatic" launch
            if (Inventory.slots.Count > 0) {
                Launch();
                _launchIsPressed = true;
            }
        }


        if (InputManager.GameControls.Character.Attack.WasPressedThisFrame()) {
            _timeLaunching = 0;

            if (_launchSuccessfull) {
                //If we launched an item this frame, then do nothing if it happens that it was the same frame that the input was pressed


            } else {
                //In case we didnt launch but the input was pressed
                if (Inventory.slots.Count > 0) { //This is the "manual" launch
                    Launch();

                    //If there is no items on the inventory, hit whatever is in front
                } else if (interactableInFront != null && interactableInFront is Geode geode) {
                    Hit(geode, geode.pointOfInteraction, manualHit: true);
                }
            }
        }

        bool interactedWithSomething = false;
        if (InputManager.GameControls.Character.Interact.WasPressedThisFrame() && interactableInFront != null) {
            interactedWithSomething = Interact(interactableInFront);
        }

        if (!interactedWithSomething && InputManager.GameControls.Character.Interact.WasPressedThisFrame()) {
            Aspiradora.instance.StartSucking();
        }

        if (InputManager.GameControls.Character.Interact.WasReleasedThisFrame()) {
            Aspiradora.instance.StopSucking();
        }

        if (InputManager.GameControls.Character.Jump.WasPressedThisFrame()) {
            StartHitting();
        }

        if (_isHitting && Time.time - _attackSpeedTimer >= 1f / attackSpeed) {

            if (interactableInFront != null && interactableInFront is Geode geode) { //Check if the geode got hit
                Hit(geode, geode.pointOfInteraction);
                SaveSystem.statistics.automaticHits++;
                _hitCancelTimer = Time.time;
                return;
            }

            if (Time.time - _hitCancelTimer > timeToCancelHit) {
                _isHitting = false;
            }
        }

        _lastFrameHadGeodeBeingHit = interactableInFront;
        _lastFrameWasLaunching = _launchIsPressed;

        #region Behaviour Of Grabbed Object

        if (Inventory.slots.Count > 0) {

            GrabableItem visualItem = Inventory.slots[^1].slotItems[^1];

            for (int i = 0; i < Inventory.slots.Count; i++) {
                for (int j = 0; j < Inventory.slots[i].slotItems.Count; j++) {
                    if (Inventory.slots[i].slotItems[j] == visualItem) { continue; }
                    Inventory.slots[i].slotItems[j].Hide();
                    Inventory.slots[i].slotItems[j].RestoreFunctionality();
                }
            }

            if (_lastVisualItem != null && _lastVisualItem != visualItem) {
                //_lastVisualItem.Hide();
            }

            visualItem.Show();
            visualItem.RemoveFunctionality();
            grabPoint.transform.localPosition = new Vector3(grabPoint.localPosition.x, grabPoint.localPosition.y, grabDistanceFromCamera);
            visualItem.transform.position = grabPoint.position;

            _lastVisualItem = visualItem;

        } else {
            _lastVisualItem = null;
        }

        #endregion
    }

    public void StartHitting() {
        _isHitting = true;
        _hitCancelTimer = Time.time;
        _lastFrameHadGeodeBeingHit = false;
    }

    public void Launch() {
        SetLaunchTimer();

        //This launches the last item saved
        GrabableItem grabableItem = Inventory.slots[^1].slotItems[^1];
        grabableItem.RestoreFunctionality();
        grabableItem.gameObject.SetActive(true);
        Inventory.RemoveItemFromInventory(grabableItem);

        grabableItem.rb.isKinematic = false;
        grabableItem.rb.AddForce(mainCamera.transform.forward * launchForce, ForceMode.Impulse);

        if (grabableItem is Geode geode) {
            geode.isLaunched = true;
        }
        if (grabableItem is SellableItem sellableItem) {
            sellableItem.canBeSelled = true;
        }
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

            if (_hits[i].collider.TryGetComponent(out Interactable interactable)) {

                //This is everything we need if the interactable was grabable
                if (interactable is GrabableItem grabableItem) {
                    bool grabableItemIsAlreadyGrabbed = false;

                    //If the item is already being hold, ignore
                    for (int n = 0; n < Inventory.slots.Count; n++) {
                        if (Inventory.slots[n].slotItems.Contains(grabableItem)) {
                            grabableItemIsAlreadyGrabbed = true;
                            break;
                        }
                    }

                    //If the geode is already being hold, ignore
                    if (grabableItem.geodeParent != null) {
                        for (int n = 0; n < Inventory.slots.Count; n++) {
                            if (Inventory.slots[n].slotItems.Contains(grabableItem.geodeParent)) {
                                grabableItemIsAlreadyGrabbed = true;
                                break;
                            }
                        }
                    }

                    if (grabableItemIsAlreadyGrabbed) { continue; }

                    if (grabableItem.geodeParent != null) {
                        grabableItem = grabableItem.geodeParent;
                    }
                    grabableItem.pointOfInteraction = _hits[i].point;

                    //Si no pongo ento y devuelvo el interactable cago, porque grabableItem y interactable eran al empezar 8 bytes apuntando a la clase que se encuentra en la memoria a largo plazo, pero grabable Item si tiene geodeParent acaba de cambiar y está aputnando al otro objeto, mientras que interactable sigue apuntando al anterior.
                    return grabableItem;
                }

                //With default interactables we only return it
                return interactable;
            }
        }

        return null;
    }

    public void Hit(Geode geode, Vector3 hitPoint, bool manualHit = false) {
        SaveSystem.statistics.Hit(manualHit: manualHit);
        geode.Hit(hitPoint, manualHit: manualHit);
        SetTimer();
    }
}
