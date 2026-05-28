using UnityEngine;

public class AudioManager : MonoBehaviour {

    public AudioSource rockHitSoft;
    public AudioClip rockHitSotfClip;
    public AudioSource rockHitHeavy;
    public AudioClip rockHitHeavyClip;
    public AudioSource rickHitAgudo;
    public AudioClip rickHitAgudoClip;
    public AudioSource rockBreak1;
    public AudioClip rockBreak1Clip;
    public AudioSource rockBreak2;
    public AudioClip rockBreak2Clip;
    public AudioSource pop;
    public AudioClip popClip;

    public bool playBreakVariant;
    public bool useAgudo;

    public static AudioManager instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    public void PlayPop() {
        pop.pitch = Random.Range(0.91f, 1.09f);
        pop.PlayOneShot(popClip);
    }

    public void PlayRockHit() {
        rockHitSoft.pitch = Random.Range(0.91f, 1.09f);
        rockHitSoft.PlayOneShot(rockHitSotfClip);
        rockHitHeavy.pitch = Random.Range(0.91f, 1.09f);
        rockHitHeavy.PlayOneShot(rockHitHeavyClip);
        if (useAgudo) {
            rickHitAgudo.pitch = Random.Range(0.91f, 1.09f);
            rickHitAgudo.PlayOneShot(rickHitAgudoClip);
        }
    }

    public void PlayRockBreak() {
        if (!playBreakVariant) {
            rockBreak1.pitch = Random.Range(0.91f, 1.09f);
            rockBreak1.PlayOneShot(rockBreak1Clip);

        } else {
            rockBreak2.pitch = Random.Range(0.91f, 1.09f);
            rockBreak2.PlayOneShot(rockBreak2Clip);
        }
    }
}
