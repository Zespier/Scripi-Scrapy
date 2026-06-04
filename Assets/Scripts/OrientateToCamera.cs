using UnityEngine;

public class OrientateToCamera : MonoBehaviour {

    private Transform mainCamera;

    private void OnEnable() {
        mainCamera = Camera.main.transform;
    }

    private void Update() {
        transform.forward = mainCamera.forward;
    }
}
