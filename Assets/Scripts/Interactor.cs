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

    private bool _isHitting;
    private float _hitCancelTimer;
    private float _attackSpeedTimer;
    private bool _lastFrameHadGeodeBeingHit;
    private RaycastHit[] _hits = new RaycastHit[10];
    private GrabableItem _lastVisualItem;
    private Camera mainCamera;

    //Primer golpe es fuerte, los demás solo 1
    public int FirstHitDamage => hitDamageByLevelss[MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.HitDamage)];
    public float HitArea => hitAreaByLevels[MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.HitArea)];

    public static Interactor instance;
    private void Awake() {
        if (!instance) { instance = this; }
        mainCamera = Camera.main;
    }

    protected virtual void SetTimer() {
        _attackSpeedTimer = !_lastFrameHadGeodeBeingHit ? Time.time : _attackSpeedTimer + 1f / attackSpeed;
    }

    private void Update() {

        //First active the cross if something is in front;
        GrabableItem itemInFront = GetItemInFront();
        for (int i = 0; i < Cross.instance.parts.Count; i++) {
            Cross.instance.parts[i].gameObject.SetActive(itemInFront != null);
        }


        if (InputManager.GameControls.Character.Attack.WasPressedThisFrame()) {

            if (Inventory.slots.Count > 0) {
                Launch();

            } else if (itemInFront != null && itemInFront is Geode geode) {
                Hit(geode, geode.pointOfInteraction, manualHit: true);
            }
        }

        if (InputManager.GameControls.Character.Interact.WasPressedThisFrame() && itemInFront != null) {
            Interact(itemInFront);
        }

        if (InputManager.GameControls.Character.Jump.WasPressedThisFrame()) {
            StartHitting();
        }

        if (_isHitting && Time.time - _attackSpeedTimer >= 1f / attackSpeed) {

            if (itemInFront != null && itemInFront is Geode geode) { //Check if the geode got hit
                Hit(geode, geode.pointOfInteraction);
                SaveSystem.statistics.automaticHits++;
                _hitCancelTimer = Time.time;
                return;
            }

            if (Time.time - _hitCancelTimer > timeToCancelHit) {
                _isHitting = false;
            }
        }

        _lastFrameHadGeodeBeingHit = itemInFront;

        #region Behaviour Of Grabbed Object

        if (Inventory.slots.Count > 0) {

            GrabableItem visualItem = Inventory.slots[^1].slotItems[^1];

            for (int i = 0; i < Inventory.slots.Count; i++) {
                for (int j = 0; j < Inventory.slots[i].slotItems.Count; j++) {
                    if (Inventory.slots[i].slotItems[j] == visualItem) { continue; }
                    Inventory.slots[i].slotItems[j].Hide();
                }
            }

            if (_lastVisualItem != null && _lastVisualItem != visualItem) {
                //_lastVisualItem.Hide();
            }

            visualItem.Show();
            visualItem.rb.isKinematic = true;
            visualItem.rb.linearVelocity = Vector3.zero;
            visualItem.rb.angularVelocity = Vector3.zero;
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

        //This launches the last item saved
        GrabableItem grabableItem = Inventory.slots[^1].slotItems[^1];
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

    public void Interact(GrabableItem grabableItem) {
        if (!grabableItem.CanBeGrabbed) { return; }

        if (grabableItem.geodeParent != null) { //If it's inside geode grab the geode
            grabableItem = grabableItem.geodeParent;
        }

        if (!Inventory.TryAddToInventory(grabableItem)) {
            /* Feedback of inventory full */
        }

        //ME QUEDA POR HACER
        /*
         * Las geodas ocupan un hueco entero de inventario
         * Las otras gemas o piedras ocupan por ejemplo hasta ocupar 20, y luego pasan al siguiente stack, esto hace que al romper más geodas y conseguir minerales nuevos, te haga querer comprarte la mejora de mejores bolsillos, y eso tambíen hace que el segundo clímax del juego vaya aumentando.
         * Luego, con la E, agarro una wea, y si pulso otra vez E, NO SE
         * Con el botón derecho se hace en el slime rancher, así qeu voy a probar botón derecho + E, las dos cosas, y soltar con el izquierdo, y hasta que no dejes de tener los bolsillos llenos no puedes pegar, básicamente con cosas en las manos no puedes pegar, de todas formas acabarías tirando todo lo que tienes en las manos con tal de quedarte con las manos vacías para poder pegar a la piedra.
         * 
         * As´que en vez de ver si estoy dándole al grabbed object, mmiro si tengo algo en la lista de platos, lo tiro, y si no tengo, golpeo, sencillo.
         * 
         * Con la ruedecilla del ratón te mueves entre un slot de inventario u otro, no sé si esto servirá para algo, pero está guay.
         */
    }

    public GrabableItem GetItemInFront() {
        int totalHits = Physics.RaycastNonAlloc(Camera.main.transform.position, Camera.main.transform.forward, _hits, interactionDistance);

        for (int i = totalHits; i < _hits.Length; i++) {
            _hits[i] = default;
        }

        for (int i = 0; i < totalHits; i++) {
            if (_hits[i].collider.CompareTag("Player")) { continue; }

            if (_hits[i].collider.TryGetComponent(out GrabableItem grabableItem)) {

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
                return grabableItem;
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
