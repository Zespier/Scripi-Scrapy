using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackController : MonoBehaviour {

    public List<ParticleFeedback> particleSystemPrefabs = new List<ParticleFeedback>();

    private List<ParticleFeedback> _particlePool = new(capacity: 64);

    public Transform MainCanvas => transform;

    public static FeedbackController instance;
    private void Awake() {
        if (!instance) {
            instance = this;
        }
    }

    private void Start() {
        InitializePools();
    }

    public void CantBreakGeode() {

    }

    private void InitializePools() {
        StartCoroutine(C_InitializePools());
    }

    private IEnumerator C_InitializePools() {

        for (int i = 0; i < particleSystemPrefabs.Count; i++) {

            for (int j = 0; j < particleSystemPrefabs[i].amountNeededInPool; j++) {
                ParticleFeedback newParticle = Instantiate(particleSystemPrefabs[i], transform);
                GetIntoThePool(newParticle);
                yield return null;
            }
        }
    }

    public ParticleFeedback GetFromPool(ParticleType particleType) {


        for (int i = 0; i < _particlePool.Count; i++) {
            if (_particlePool[i].type == particleType) {

                if (_particlePool[i].Deactivated) {
                    _particlePool[i].gameObject.SetActive(true);
                    return _particlePool[i];
                }
            }
        }

        int index = 0;
        for (int i = 0; i < particleSystemPrefabs.Count; i++) {
            if (particleSystemPrefabs[i].type == particleType) {
                index = i;
                break;
            }
        }

        ParticleFeedback newParticle = Instantiate(particleSystemPrefabs[index], transform);
        GetIntoThePool(newParticle);
        newParticle.gameObject.SetActive(true);
        return newParticle;
    }

    public void GetIntoThePool(ParticleFeedback particleFeedback) {
        if (!_particlePool.Contains(particleFeedback)) {
            _particlePool.Add(particleFeedback);
        }

        if (particleFeedback.particleSystems.Count > 0) {
            for (int i = 0; i < particleFeedback.particleSystems.Count; i++) {
                particleFeedback.particleSystems[i].Stop();
            }
        }

        if (particleFeedback.visualEffects.Count > 0) {
            for (int i = 0; i < particleFeedback.visualEffects.Count; i++) {
                particleFeedback.visualEffects[i].Stop();
            }
        }

        if (particleFeedback.isText) {
            particleFeedback.gameObject.SetActive(false);
        }
    }

    public ParticleFeedback PlayParticle(ParticleType particleType, Vector3 position, Vector3 direction, Transform newParent = default) {
        if (newParent == default) { newParent = transform; }

        ParticleFeedback particleFeedback = GetFromPool(particleType);
        particleFeedback.transform.position = position;
        particleFeedback.transform.forward = direction;
        particleFeedback.transform.parent = newParent;

        particleFeedback.Play();
        return particleFeedback;
    }

}

public static class ListExtensions {
    public static void RemoveOptimized<T>(this List<T> list, int index) {

        T temporal = list[index];
        list[index] = list[^1];
        list[^1] = temporal;

        list.Remove(temporal);
    }
}