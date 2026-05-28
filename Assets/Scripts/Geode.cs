using System.Collections.Generic;
using UnityEngine;

public class Geode : MonoBehaviour {

    public Vector2 breakForce = new Vector2(4, 8);
    public Rigidbody rb;
    public List<GameObject> children;
    public Transform newParent;
    public bool isLaunched;
    public int health = 5;

    private int _currentHealth;

    private void OnEnable() {
        _currentHealth = health;
    }

    [ContextMenu("Break")]
    public void Break() {

        for (int i = 0; i < children.Count; i++) {
            children[i].transform.parent = newParent;

            var rb = children[i].gameObject.AddComponent<Rigidbody>();

            rb.linearVelocity = this.rb.linearVelocity;
            rb.angularVelocity = this.rb.angularVelocity;

            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1), Random.Range(-1f, 1), Random.Range(-1f, 1)).normalized;
            float randomForce = Random.Range(breakForce.x, breakForce.y);

            rb.AddForce(randomDirection * randomForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * randomForce, ForceMode.Impulse);
        }

        Destroy(gameObject);

        CameraHolder.instance.BreakShake();
    }

    public void Hit() {
        Cross.instance.CrossAnimation();

        _currentHealth--;
        if (_currentHealth <= 0) {
            Break();
            AudioManager.instance.PlayRockBreak();

        } else {
            CameraHolder.instance.HitShake();
            AudioManager.instance.PlayRockHit();
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (isLaunched) {
            Hit();
            isLaunched = false;
        }
    }
}
