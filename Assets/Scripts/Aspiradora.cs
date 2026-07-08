using UnityEngine;

public class Aspiradora : MonoBehaviour {

    public float suckDistance = 2f;
    public float absortionDistance = 0.5f;
    public float suckPointDistanceFromPlayer = 1f;
    //public float accelerationAppliedToItems = 1f;
    public float maxVelocityForItems = 9f;

    private bool _isSucking;

    public static Aspiradora instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    private void Update() {
        if (!_isSucking) { return; }

        Vector3 suckPoint = CameraHolder.instance.transform.position + CameraHolder.instance.transform.forward * suckPointDistanceFromPlayer;

        for (int i = 0; i < Active.sellableItems.Count; i++) {

            SellableItem item = Active.sellableItems[i];
            if (!item.canBeGrabbed || item.rb == null) { continue; }

            //If it's too close to the point, successfully suck it
            if ((item.transform.position - suckPoint).sqrMagnitude < absortionDistance * absortionDistance) {
                Interactor.instance.Interact(item);
                continue;
            }

            ////If the item is going too fast, don't apply more accelaration
            //if (!(item.rb.linearVelocity.sqrMagnitude > maxVelocityForItems * maxVelocityForItems)) {
            //If the item is close to the suckPoint, suck it
            //TODO: Maybe circular movement with a SLerp?
            if ((item.transform.position - suckPoint).sqrMagnitude < suckDistance * suckDistance) {
                //item.rb.AddForce((suckPoint - item.transform.position) * accelerationAppliedToItems, ForceMode.Acceleration);

                item.rb.linearVelocity = Vector3.Slerp(item.rb.linearVelocity, (suckPoint - item.transform.position).normalized * maxVelocityForItems, Time.deltaTime * 7.5f);

                //item.rb.linearVelocity
            }
            //}
        }
    }

    public void StartSucking() {
        _isSucking = true;
    }

    public void StopSucking() {
        _isSucking = false;
    }
}
