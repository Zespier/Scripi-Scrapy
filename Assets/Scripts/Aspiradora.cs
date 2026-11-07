using UnityEngine;

public class Aspiradora : MonoBehaviour {

    public float suckDistance = 2f;
    public float absortionDistance = 0.5f;
    public float suckPointDistanceFromPlayer = 1f;
    public float maxVelocityForItems = 9f;

    private bool _isSucking;

    public static Aspiradora instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    private void Update() {
        if (!_isSucking) { return; }

        Vector3 suckPoint = CameraHolder.instance.transform.position + CameraHolder.instance.transform.forward * suckPointDistanceFromPlayer;

        for (int i = 0; i < Active.valuable.Count; i++) {

            Valuable item = Active.valuable[i];
            if (item.rb == null) { continue; }

            //If it's too close to the point, successfully suck it
            if ((item.transform.position - suckPoint).sqrMagnitude < absortionDistance * absortionDistance) {
                Interactor.instance.Interact(item);
                continue;
            }

            //Accelerate the item with a lerp towards the aspiradora
            if ((item.transform.position - suckPoint).sqrMagnitude < suckDistance * suckDistance) {
                item.rb.linearVelocity = Vector3.Slerp(item.rb.linearVelocity, (suckPoint - item.transform.position).normalized * maxVelocityForItems, Time.deltaTime * 7.5f);
            }
        }
    }

    public void StartSucking() {
        _isSucking = true;
    }

    public void StopSucking() {
        _isSucking = false;
    }
}
