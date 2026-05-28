using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cross : MonoBehaviour {

    public List<RectTransform> parts;
    public float lerpSpeed = 10f;

    private Coroutine c_CrossAnimation;

    public static Cross instance;
    private void Awake() {
        if (!instance) { instance = this; }
        ResetCross();
    }

    private void ResetCross() {
        for (int i = 0; i < parts.Count; i++) {
            Vector3 newPosition = new Vector3(Mathf.Sign(parts[i].localPosition.x), Mathf.Sign(parts[i].localPosition.y), 0) * 14.45f;
            parts[i].localPosition = newPosition;
        }
    }

    public void CrossAnimation() {
        if (c_CrossAnimation != null) {
            StopCoroutine(c_CrossAnimation);
            ResetCross();
        }

        c_CrossAnimation = StartCoroutine(C_CrossAnimation());
    }

    private IEnumerator C_CrossAnimation() {

        List<Vector3> initialPoints = new List<Vector3>();
        for (int i = 0; i < parts.Count; i++) {
            Vector3 initialPosition = new Vector3(Mathf.Sign(parts[i].localPosition.x), Mathf.Sign(parts[i].localPosition.y), 0) * 14.45f;
            initialPoints.Add(initialPosition);

            parts[i].localPosition = initialPosition * Random.Range(2.5f, 3f);
        }

        while (parts[0].localPosition != initialPoints[0]) {
            yield return null;

            for (int i = 0; i < parts.Count; i++) {
                parts[i].localPosition = Vector3.Lerp(parts[i].localPosition, initialPoints[i], Time.unscaledDeltaTime * lerpSpeed);
            }
        }

        for (int i = 0; i < parts.Count; i++) {
            parts[i].localPosition = initialPoints[i];
        }
    }
}
