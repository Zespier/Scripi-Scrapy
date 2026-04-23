using System.Collections;
using UnityEngine;

public class CameraHolder : MonoBehaviour {

    public Transform target;
    public Transform targetHelper;
    public bool resetRotationAtStart;
    public float maxVerticalAngle = 70f;
    public float minVerticalAngle = -40f;
    public float sensitivity = 8f;
    public float cameraRotationLerpSpeed = 10f;
    public Transform actualCamera;
    public float shakeDuration = 1.5f;
    public float shakeMagnitude = 0.2f;
   
    public static CameraHolder instance;
    private void Awake() {

        if (!instance) { instance = this; }

        if (resetRotationAtStart) {
            target.forward = -Vector3.forward;
            targetHelper.forward = -Vector3.forward;
        }
    }

    private void OnEnable() {
        //InputManager.OnCharacterInteract += 
    }

    private void OnDisable() {
        //InputManager.OnCharacterInteract -= 
    }

    private void Update() {

        CameraForward();

        RotateCameraHolder();
    }

    public void CameraForward() {
        //transform.forward = target.forward;
        transform.forward = targetHelper.forward;
    }

    public void RotateCameraHolder() {

        target.forward = Vector3.Slerp(target.forward, targetHelper.forward, Time.deltaTime * cameraRotationLerpSpeed);

        //Horizontal
        targetHelper.forward = Quaternion.AngleAxis(InputManager.ViewDirection.x * sensitivity * Time.deltaTime, Vector3.up) * targetHelper.forward;

        //Complex vertical
        Vector3 straightForward = targetHelper.forward;
        straightForward.y = 0;

        Vector3 newForward = Quaternion.AngleAxis(-InputManager.ViewDirection.y * sensitivity * Time.deltaTime, targetHelper.right) * targetHelper.forward;

        float signedAngle = Vector3.SignedAngle(newForward, straightForward, targetHelper.right);

        if (signedAngle < minVerticalAngle || signedAngle > maxVerticalAngle) { return; }

        targetHelper.forward = newForward;
    }

    public IEnumerator C_Shake() {
        Vector3 originalPos = Vector3.zero;

        float timer = Time.time;

        while (Time.time - timer < shakeDuration) {
            float x = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * shakeMagnitude;

            actualCamera.localPosition = new Vector3(x, y, originalPos.z);

            yield return null;
        }

        actualCamera.localPosition = originalPos;
    }
}

public enum DeathType {
    Pasillo,
    DebajoDeLaMesa,
    SalaDeReuniones,
    Tele,
}
