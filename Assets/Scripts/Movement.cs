using UnityEngine;

public class Movement : MonoBehaviour {

    public Rigidbody rb;
    public float speed = 6;

    private void Update() {
        //Vector3 speed = new Vector3(Camera.main.transform.x, rb.linearVelocity.y, InputManager.Movement.y).normalized * speed

        Vector3 customRight = Camera.main.transform.right;
        customRight.y = 0;
        customRight = customRight.normalized;

        Vector3 customForwward = Camera.main.transform.forward;
        customForwward.y = 0;
        customForwward = customForwward.normalized;

        Vector3 linearVelocity = (customRight * InputManager.Movement.x + customForwward * InputManager.Movement.y).normalized * speed;
        linearVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = linearVelocity;

        //rb.linearVelocity = new Vector3(InputManager.Movement.x * , rb.linearVelocity.y, InputManager.Movement.y).normalized * speed;
    }
}
