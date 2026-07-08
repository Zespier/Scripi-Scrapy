using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EconomyCreationCamera : MonoBehaviour {

    public Camera cam;
    public float sensitivity = 10f;
    public float sensitivityPerSize = 0.02f;
    public Vector2 sizeLimit = new Vector2(100, 10000);

    private float _lerpedScroll;
    private Vector2 _lerpedMovement;
    private Vector2 _lastDelta;

    public Vector2 Position => new Vector3(transform.position.x, transform.position.y);

    public static EconomyCreationCamera instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    private void Update() {

        Scroll();

        Move();
    }

    private void Scroll() {

        //if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {

        //    SetDelta(new Vector2(0, -Input.mouseScrollDelta.y * (sensitivity * 3 + cam.orthographicSize * sensitivityPerSize / 20)));

        //} else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {

        //    SetDelta(new Vector2(Input.mouseScrollDelta.y * (sensitivity * 3 + cam.orthographicSize * sensitivityPerSize / 20), 0));

        //} else {
        _lerpedScroll = Mathf.Lerp(_lerpedScroll, -Mouse.current.scroll.ReadValue().y, Time.deltaTime / 0.05f);
        cam.orthographicSize = cam.orthographicSize += _lerpedScroll * (sensitivity + cam.orthographicSize * sensitivityPerSize);

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, sizeLimit.x, sizeLimit.y);
        //}
    }

    private void Move() {
        float currentGlobalHeight = cam.orthographicSize * 2f;
        float ratio = currentGlobalHeight / 1080f;

        _lerpedMovement = Vector2.Lerp(_lerpedMovement, -_lastDelta, Time.deltaTime / 0.01f);
        transform.position += (Vector3)_lerpedMovement * ratio;

        _lastDelta = Vector2.zero;
    }

    public void SetDelta(Vector2 delta) {
        _lastDelta = delta;
    }
}
