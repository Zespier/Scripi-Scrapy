using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ParticleFeedback : MonoBehaviour {

    public ParticleType type;
    public List<ParticleSystem> particleSystems;
    public List<VisualEffect> visualEffects;
    public bool isText;
    [Tooltip("Only neccesary if it's a visualEffect")] public float particleDuration = 1f;
    public int amountNeededInPool = 1;
    public bool activated;

    private Coroutine c_Control;

    public bool Deactivated {
        get {

            if (particleSystems.Count > 0) {
                bool isPlaying = false;

                for (int i = 0; i < particleSystems.Count; i++) {
                    if (particleSystems[i].isPlaying) {
                        isPlaying = true;
                        break;
                    }
                }

                return !isPlaying;

            } else if (visualEffects.Count > 0) {
                return !activated;

            } else {
                return false;
            }
        }
    }

    public virtual void Play() {
        if (particleSystems.Count > 0) {
            for (int i = 0; i < particleSystems.Count; i++) {
                particleSystems[i].Play();
            }
        }

        if (visualEffects.Count > 0) {
            for (int i = 0; i < visualEffects.Count; i++) {
                visualEffects[i].Play();
            }
            ActivateControl();
        }

        if (isText) {
            ActivateControl();
        }
    }

    public void ActivateControl() {
        activated = true;
        if (c_Control != null) {
            StopCoroutine(c_Control);
        }
        c_Control = StartCoroutine(C_CustomUpdate());
    }

    private IEnumerator C_CustomUpdate() {

        yield return new WaitForSeconds(particleDuration);

        activated = false;
        for (int i = 0; i < visualEffects.Count; i++) {
            visualEffects[i].Stop();
        }

        if (isText) {
            gameObject.SetActive(false);
        }
    }

    public void Stop() {
        Debug.Log("stop");

        if (particleSystems.Count > 0) {
            for (int i = 0; i < particleSystems.Count; i++) {
                particleSystems[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        if (visualEffects.Count > 0) {
            for (int i = 0; i < visualEffects.Count; i++) {
                visualEffects[i].Stop();
            }
            activated = false;
            StopCoroutine(c_Control);
        }
    }
}

public enum ParticleType : byte {
    None = 0,
    CantBreakGeode = 1,
}