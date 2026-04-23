using System.Collections.Generic;
using UnityEngine;

public class Geode : MonoBehaviour {

    public Rigidbody parent;
    public List<GameObject> children;
    public Transform newParent;

    [ContextMenu("Break")]
    public void Break() {
        for (int i = 0; i < children.Count; i++) {
            children[i].transform.parent = newParent;

            var rb = children[i].gameObject.AddComponent<Rigidbody>();

            rb.linearVelocity = parent.linearVelocity;
            rb.angularVelocity = parent.angularVelocity;

            Vector3 randomDirection = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized;
            float randomForce = Random.Range(5, 10);

            rb.AddForce(randomDirection * randomForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * randomForce, ForceMode.Impulse);

            Destroy(gameObject);
        }
    }
}
