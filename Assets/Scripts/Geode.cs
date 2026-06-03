using System.Collections.Generic;
using UnityEngine;

public class Geode : MonoBehaviour {

    public Vector2 breakForce = new Vector2(4, 8);
    public Rigidbody rb;
    public List<GameObject> children;
    public Transform newParent;
    public bool isLaunched;
    public int health = 5;

    private float _currentHealth;

    private void OnEnable() {
        _currentHealth = health;

        Active.AddGeode(this);
    }

    private void OnDisable() {
        Active.RemoveGeode(this);
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


            if (children[i].TryGetComponent(out GeodePart geodePart)) {
                geodePart.StartDissapearTimer();

            } else if (children[i].TryGetComponent(out SellableItem sellableItem)) {
                sellableItem.insideGeode = false;
            }
        }

        SaveSystem.statistics.geodesBroken++;
        CameraHolder.instance.BreakShake();

        Destroy(gameObject);
    }

    public void Hit(Vector3 hitPoint, bool hitWall = false, bool manualHit = false) {
        Cross.instance.CrossAnimation();

        _currentHealth -= Interactor.instance.hitDamage;
        if (_currentHealth <= 0) {
            Break();
            if (!manualHit) { SaveSystem.statistics.geodesBrokenWithAutomaticGear++; }
            AudioManager.instance.PlayRockBreak();

        } else {
            CameraHolder.instance.HitShake();
            AudioManager.instance.PlayRockHit();
        }

        if (!hitWall && Interactor.instance.hitArea == 1) { return; }

        for (int i = 0; i < Active.geodes.Count; i++) {
            Geode geode = Active.geodes[i];
            if (geode == this) { continue; }

            if (Vector3.Distance(hitPoint, geode.transform.position) <= Interactor.instance.hitArea) {
                geode.HitByArea();
            }
        }
    }

    public void HitByArea() {
        SaveSystem.statistics.CollateralHit();
        _currentHealth -= Interactor.instance.hitDamage;
        if (_currentHealth <= 0) {
            Break();
            SaveSystem.statistics.geodesBrokenByCollateralDamage++;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (isLaunched) {
            Hit(Vector3.zero, hitWall: true);
            isLaunched = false;
        }
    }
}
