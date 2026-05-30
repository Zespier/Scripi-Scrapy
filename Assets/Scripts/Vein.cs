using System.Collections.Generic;
using UnityEngine;

public class Vein : MonoBehaviour {

    public List<Geode> possibleGeodes;
    public List<float> chances = new List<float> { 1, 0, 0 };

    public Geode DecideGeode() {
        float totalChances = 0;
        for (int i = 0; i < chances.Count; i++) {
            totalChances += chances[i];
        }
        float random = Random.value * totalChances;

        float nextRandomCeiling = 0;
        for (int i = 0; i < chances.Count; i++) {
            nextRandomCeiling += chances[i];
            if (random <= nextRandomCeiling) {
                return possibleGeodes[i];
            }
        }

        Debug.LogError(nextRandomCeiling);
        Debug.LogError(random);
        Debug.LogError("Something weird happened with chances");
        return possibleGeodes[0];
    }
}
