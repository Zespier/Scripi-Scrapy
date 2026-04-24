using System.Collections.Generic;
using UnityEngine;

public class Geode : MonoBehaviour {

    public Vector2 breakForce = new Vector2(4, 8);
    public Rigidbody rb;
    public List<GameObject> children;
    public Transform newParent;
    public bool isLaunched;

    [ContextMenu("Break")]
    public void Break() {
        for (int i = 0; i < children.Count; i++) {
            children[i].transform.parent = newParent;

            var rb = children[i].gameObject.AddComponent<Rigidbody>();

            rb.linearVelocity = this.rb.linearVelocity;
            rb.angularVelocity = this.rb.angularVelocity;

            Vector3 randomDirection = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized;
            float randomForce = Random.Range(breakForce.x, breakForce.y);

            rb.AddForce(randomDirection * randomForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * randomForce, ForceMode.Impulse);

            Destroy(gameObject);
        }
    }

    public void Hit() {
        Break();
    }

    private void OnCollisionEnter(Collision collision) {
        if (isLaunched) {
            Hit();
            isLaunched = false;
        }
    }
}
