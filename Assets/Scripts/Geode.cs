using System.Collections.Generic;
using UnityEngine;

public class Geode : MonoBehaviour {

    public Vector2 breakForce = new Vector2(4, 8);
    public Rigidbody rb;
    public List<GameObject> children;
    public Transform newParent;
    public bool isLaunched;
    public int health = 4;

    private float _currentHealth;
    private bool _wasHitForTheFirstTime;
    private ParticleFeedback _cantBreakThisGeodeFeedback;

    private void OnEnable() {
        _currentHealth = health;

        Active.AddGeode(this);
    }

    private void OnDisable() {
        Active.RemoveGeode(this);
    }

    public void ResetVariables() {
        _currentHealth = health;
        _wasHitForTheFirstTime = false;
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
        if (health - MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.HitDamage) >= 10) {

            if (_cantBreakThisGeodeFeedback == null || _cantBreakThisGeodeFeedback.Deactivated) {
                _cantBreakThisGeodeFeedback = FeedbackController.instance.PlayParticle(ParticleType.CantBreakGeode, transform.position + Vector3.up * 1.1f, Vector3.forward);
            }


            return;
        }

        Cross.instance.CrossAnimation();

        ReduceHealth();

        if (_currentHealth <= 0) {
            Break();
            if (!manualHit) { SaveSystem.statistics.geodesBrokenWithAutomaticGear++; }
            AudioManager.instance.PlayRockBreak();

        } else {
            CameraHolder.instance.HitShake();
            AudioManager.instance.PlayRockHit();
        }

        if (!hitWall && Interactor.instance.HitArea == 1) { return; }

        for (int i = 0; i < Active.geodes.Count; i++) {
            Geode geode = Active.geodes[i];
            if (geode == this) { continue; }

            if (hitPoint.DistanceSquared(geode.transform.position) < Interactor.instance.HitArea * Interactor.instance.HitArea) {
                geode.HitByArea();
            }
        }
    }

    public void HitByArea() {
        if (health - MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.HitDamage) >= 10) { return; }

        SaveSystem.statistics.CollateralHit();
        ReduceHealth();

        if (_currentHealth <= 0) {
            Break();
            SaveSystem.statistics.geodesBrokenByCollateralDamage++;
        }
    }

    private void ReduceHealth() {
        if (!_wasHitForTheFirstTime) {
            _wasHitForTheFirstTime = true;
            _currentHealth -= Interactor.instance.FirstHitDamage;

        } else {
            _currentHealth--;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (isLaunched) {
            Hit(Vector3.zero, hitWall: true);
            isLaunched = false;
        }
    }
}
