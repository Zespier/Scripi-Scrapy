using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactor : MonoBehaviour {

    public float interactionDistance = 6;
    public float grabDistanceFromCamera = 4f;
    public float launchForce = 10f;
    public float attackSpeed = 2f;
    public float timeToCancelHit = 0.9f;
    public List<int> hitDamageByLevelss = new List<int>() { 1, 4, 9, 13, 18, 21 };
    public List<float> hitAreaByLevels = new List<float>() { 1, 1.3f, 2f, 3f, 4f, 5f };

    private Stack<GrabableItem> grabbedObjects = new Stack<GrabableItem>();
    private bool _isHitting;
    private float _hitCancelTimer;
    private float _attackSpeedTimer;
    private bool _lastFrameHadGeodeBeingHit;
    private RaycastHit[] _hits = new RaycastHit[10];
    private GrabableItem _lastVisualItem;

    //Primer golpe es fuerte, los demás solo 1
    public int FirstHitDamage => MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.HitDamage);
    public int HitArea => MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.HitArea);

    public static Interactor instance;
    private void Awake() {
        if (!instance) { instance = this; }
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

            if (grabbedObjects.Count > 0) {
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

        if (grabbedObjects.Count > 0) {

            GrabableItem visualItem = grabbedObjects.Peek();

            foreach (GrabableItem item in grabbedObjects) {
                if (item == visualItem) { continue; }
                item.Hide();
            }

            if (_lastVisualItem != null && _lastVisualItem != visualItem) {
                //_lastVisualItem.Hide();
            }

            visualItem.Show();
            visualItem.rb.isKinematic = true;
            visualItem.rb.linearVelocity = Vector3.zero;
            visualItem.rb.angularVelocity = Vector3.zero;
            visualItem.transform.position = (Camera.main.transform.position + Camera.main.transform.forward * grabDistanceFromCamera);

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

        GrabableItem grabableItem = grabbedObjects.Pop();

        grabableItem.rb.isKinematic = false;
        grabableItem.rb.AddForce(Camera.main.transform.forward * launchForce, ForceMode.Impulse);

        if (grabableItem is Geode geode) {
            geode.isLaunched = true;
        }
    }

    public void Interact(GrabableItem grabableItem) {
        if (!grabableItem.CanBeGrabbed) { return; }
        if (!Inventory.CanAddToInventory(grabableItem)) { return; }

        if (grabableItem.geodeParent != null) { //If it's inside geode grab the geode
            grabableItem = grabableItem.geodeParent;
        }
        grabbedObjects.Push(grabableItem);

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
                if (grabbedObjects.Contains(grabableItem)) { continue; }
                if (grabableItem.geodeParent != null && grabbedObjects.Contains(grabableItem.geodeParent)) { continue; }

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
