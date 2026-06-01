using System.Collections;
using UnityEngine;

public class GeodePart : MonoBehaviour {

    public float timeAlive = 5f;
    public float timeOfIncreaseSize = 0.2f;
    public float increseSizeAmount = 1.2f;
    public float dissapearTime = 1f;

    private bool _dissapearing;

    public void StartDissapearTimer() {
        if (_dissapearing) { return; }
        StartCoroutine(C_Dissapear());
    }

    private IEnumerator C_Dissapear() {
        _dissapearing = true;

        float timer = Time.time;
        while (Time.time - timer < timeAlive) {
            yield return null;
        }

        Vector3 initialSize = transform.localScale;

        timer = Time.time;
        while (Time.time - timer < timeOfIncreaseSize) {

            transform.localScale = Vector3.Lerp(initialSize, initialSize * increseSizeAmount, (Time.time - timer) / timeOfIncreaseSize);
            yield return null;
        }

        transform.localScale = initialSize * increseSizeAmount;

        initialSize = transform.localScale;

        timer = Time.time;
        while (Time.time - timer < dissapearTime) {

            transform.localScale = Vector3.Lerp(initialSize, Vector3.zero, (Time.time - timer) / dissapearTime);
            yield return null;
        }

        transform.localScale = Vector3.zero;

        Destroy(gameObject);
        _dissapearing = false;
    }
}
