using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour {

    public Vector2 breakForce = new Vector2(4, 8);
    public List<GeodePart> children;
    public int health = 4;
    public List<Valuable> the3Items;
    public MeteorTier tier;
    public List<Vector3> chances = new List<Vector3>(capacity: 3);
    public Vector3 impactPoint; //The point it falls towards
    public float speed = 20f;
    public GrabableState state = GrabableState.None;
    public float simulatedColliderRadius = 0.3f;
    public Vector3 hitPoint;

    private float _currentHealth;
    private bool _wasHitForTheFirstTime;
    private ParticleFeedback _cantBreakThisGeodeFeedback;
    private bool _isBroken;
    private List<(List<int> amounts, List<float> chances)> multipleMineralsData = new List<(List<int>, List<float>)>() {
         (new List<int>(){ 1, 2 } , new List<float>(){ 85, 15 }),
         (new List<int>(){ 1, 2, 3} , new List<float>(){ 40, 40, 20 }),
         (new List<int>(){ 2, 3, 4} , new List<float>(){ 60, 30, 10 }),
    };

    public Vector3 Chances => chances[(int)tier];

    private void OnEnable() {
        _currentHealth = health;

        Active.AddMeteor(this);
    }

    private void OnDisable() {
        Active.RemoveMeteor(this);
    }

    private void Update() {
        if (state == GrabableState.Falling) {
            transform.position += Time.deltaTime * speed * (impactPoint - transform.position).normalized;
            if ((transform.position - impactPoint).sqrMagnitude < 1 * 1) {
                transform.position = impactPoint;
                state = GrabableState.None;
            }
        } else {
            transform.position = impactPoint;
        }
    }

    public void AdjustImpactPointToGround() {
        Ray ray = new Ray(impactPoint + Vector3.up * 100f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, int.MaxValue, MeteorShower.instance.groundLayer)) {
            impactPoint = hit.point;
        } else {
            Debug.LogError($"Couldn't find ground => {this.name}");
        }
    }

    public bool CheckRay(Ray ray, out Vector3 hitPoint, float maxDistance) {
        hitPoint = ray.origin + Vector3.Dot(transform.position - ray.origin, ray.direction) * ray.direction;

        if ((ray.origin - hitPoint).sqrMagnitude >= maxDistance) {
            return false;
        }

        float distance = (hitPoint - transform.position).sqrMagnitude;

        return distance <= simulatedColliderRadius * simulatedColliderRadius;
    }

    public void ResetVariables() {
        _currentHealth = health;
        _wasHitForTheFirstTime = false;
    }

    [ContextMenu("Break")]
    public void Break() {

        if (_isBroken) {
            return;
        }
        _isBroken = true;

        int numberOfMinerals = GetNumberOfMinerals();
        List<Valuable> createdValuables = new List<Valuable>(capacity: numberOfMinerals);

        for (int i = 0; i < numberOfMinerals; i++) {
            Valuable newSellableItem = Instantiate(DecideSellableItem(), MeteorContainer.instance.transform);
            createdValuables.Add(newSellableItem);
            newSellableItem.transform.position = transform.position;
        }

        void AddForceToRB(Rigidbody rb) {

            //rb.linearVelocity = this.rb.linearVelocity;
            //rb.angularVelocity = this.rb.angularVelocity;

            Vector3 randomDirection = new Vector3(Random.Range(-1f, 1), Random.Range(-1f, 1), Random.Range(-1f, 1)).normalized;
            float randomForce = Random.Range(breakForce.x, breakForce.y);

            rb.AddForce(randomDirection * randomForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * randomForce, ForceMode.Impulse);
        }

        for (int i = 0; i < children.Count; i++) {

            children[i].transform.parent = ValuableContainer.instance.transform;
            children[i].rb = children[i].gameObject.AddComponent<Rigidbody>();

            if (children[i].TryGetComponent(out GeodePart geodePart)) {
                geodePart.StartDissapearTimer();
            }

            var rb = children[i].rb;
            AddForceToRB(rb);
        }

        for (int i = 0; i < createdValuables.Count; i++) {
            var rb = createdValuables[i].rb;
            AddForceToRB(rb);
        }

        SaveSystem.statistics.valuablesMined += numberOfMinerals;
        SaveSystem.statistics.meteoritesBroken++;
        CameraHolder.instance.BreakShake();

        MeteorShower.instance.occupiedPoints.Remove(impactPoint);

        Destroy(gameObject);
    }

    public void Hit(Vector3 hitPoint, bool hitWall = false, bool manualHit = false) {
        if (health - Interactor.instance.FirstHitDamage >= 10) {

            if (_cantBreakThisGeodeFeedback == null || _cantBreakThisGeodeFeedback.Deactivated) {
                _cantBreakThisGeodeFeedback = FeedbackController.instance.PlayParticle(ParticleType.CantBreakGeode, transform.position + Vector3.up * 1.1f, Vector3.forward);
            }


            return;
        }

        Cross.instance.CrossAnimation();

        ReduceHealth();

        if (_currentHealth <= 0) {
            Break();
            if (!manualHit) { SaveSystem.statistics.meteoritesBrokenWithAutomaticGear++; }
            AudioManager.instance.PlayRockBreak();

        } else {
            CameraHolder.instance.HitShake();
            AudioManager.instance.PlayRockHit();
        }

        if (!hitWall && Interactor.instance.HitArea == 1) { return; }

        for (int i = 0; i < Active.meteors.Count; i++) {
            Meteor geode = Active.meteors[i];
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
            SaveSystem.statistics.meteoritesBrokenByCollateralDamage++;
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

    private Valuable DecideSellableItem() {
        float random = Random.Range(0, 100);

        float totalChance = Chances.x;
        if (random <= totalChance) {
            return the3Items[0];
        }

        totalChance += Chances.y;
        if (random <= totalChance) {
            return the3Items[1];
        }

        totalChance += Chances.z;
        if (random <= totalChance) {
            return the3Items[2];
        }

        //This should never happen
        Debug.LogError("The weird thing happened iwth chances");
        return the3Items[0];
    }

    public int GetNumberOfMinerals() {

        //This separates the Lists from the tuple, I wrote it like that to group the data inside one list
        var (amounts, chances) = multipleMineralsData[MenuUpgrades.instance.GetUpgradeLevel(UpgradeType.MultipleMinerals)];

        float randomValue = Random.Range(0f, 100);

        float totalChance = 0f;

        for (int i = 0; i < chances.Count; i++) {
            totalChance += chances[i];

            if (randomValue <= totalChance) {
                return amounts[i];
            }
        }

        return amounts[amounts.Count - 1];
    }
}

public enum MeteorTier {
    Tier1,
    Tier2,
    Tier3,
    Tier4,
    Tier5,
}