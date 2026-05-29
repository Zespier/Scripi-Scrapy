using System.Collections;
using UnityEngine;

public class CameraHolder : MonoBehaviour {

    public Transform player;
    public float height = 1.75f;
    public Transform target;
    public Transform targetHelper;
    public bool resetRotationAtStart;
    public float maxVerticalAngle = 70f;
    public float minVerticalAngle = -40f;
    public float sensitivity = 8f;
    public float cameraRotationLerpSpeed = 10f;
    public Transform actualCamera;
    public ShakeData hitShake;
    public ShakeData breakShake;

    private Coroutine c_Shake;

    public static CameraHolder instance;
    private void Awake() {

        if (!instance) { instance = this; }

        if (resetRotationAtStart) {
            target.forward = -Vector3.forward;
            targetHelper.forward = -Vector3.forward;
        }
    }

    private void Update() {
        Movement();

        CameraForward();

        RotateCameraHolder();
    }

    public void Movement() {
        target.position = player.position + Vector3.up * height;
        targetHelper.position = target.position;
        transform.position = target.position;
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

    public void HitShake() {
        Shake(hitShake);
    }

    public void BreakShake() {
        Shake(breakShake);
    }

    public void Shake(ShakeData shakeData) {
        if (c_Shake != null) {
            StopCoroutine(c_Shake);
        }
        c_Shake = StartCoroutine(C_Shake(shakeData));
    }

    private IEnumerator C_Shake(ShakeData shakeData) {

        float x = Random.Range(-1f, 1f) * shakeData.shakeMagnitude;
        float y = Random.Range(-1f, 1f) * shakeData.shakeMagnitude;

        actualCamera.localPosition = new Vector3(x, y, 0);
        actualCamera.localRotation = shakeData.rotationHelper.localRotation;

        float timer = Time.time;

        while (Time.time - timer < shakeData.shakeDuration) {

            actualCamera.localPosition = Vector3.Lerp(new Vector3(x, y, 0), Vector3.zero, (Time.time - timer) / shakeData.shakeDuration);
            actualCamera.localRotation = Quaternion.Lerp(shakeData.rotationHelper.localRotation, Quaternion.identity, (Time.time - timer) / shakeData.shakeDuration);
            yield return null;
        }

        actualCamera.localPosition = Vector3.zero;
        actualCamera.localRotation = Quaternion.identity;
    }
}

public enum DeathType {
    Pasillo,
    DebajoDeLaMesa,
    SalaDeReuniones,
    Tele,
}

[System.Serializable]
public struct ShakeData {
    public float shakeDuration; //Recomended = 0.19f;
    public float shakeMagnitude; //Recomended = 0.09f;
    public Transform rotationHelper;
}