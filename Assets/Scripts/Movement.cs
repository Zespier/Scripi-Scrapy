using UnityEngine;

public class Movement : MonoBehaviour {

    public Rigidbody rb;
    public float speed = 6;

    private void Update() {
        rb.linearVelocity = new Vector3(InputManager.Movement.x, rb.linearVelocity.y, InputManager.Movement.y).normalized * speed;
    }
}
