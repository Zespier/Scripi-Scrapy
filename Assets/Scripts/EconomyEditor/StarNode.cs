using System.Collections.Generic;
using TMPro;

public class StarNode : Node {

    public TMP_Text moneyPS;
    public TMP_Text timeNeeded;
    public float baseMoneyPS;

    public static StarNode instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    protected override void Update() {
        GetNextUpgrade();
    }

    public void GetNextUpgrade() {
        List<Node> allNodes = HandEconomyCreation.instance.allNodes;

        //float maxXLeft = float.MinValue;
        //float minXRight = float.MaxValue;
        //Node leftNode = null;
        //Node rightNode = allNodes[0];

        //for (int i = 0; i < count; i++) {
        //    float nodeX = allNodes[i].rectTransform.position.x;
        //    //La idea es coger el que tenga mayor X, pero tenga menor X que la estrella. Si no hay pues na
        //    if (nodeX > maxXLeft && nodeX < rectTransform.position.x) {
        //        maxXLeft = nodeX;
        //        leftNode = allNodes[i];
        //    }
        //    //La idea es coger el que tenga menor X, pero tenga más X que la estrella, TIENE QUE HABER
        //    if (nodeX < minXRight && nodeX > rectTransform.position.x) {
        //        minXRight = nodeX;
        //        rightNode = allNodes[i];
        //    }
        //}

        bool ordered = false;
        int count = allNodes.Count;
        int securityCounter = 0;
        while (!ordered && securityCounter < 1000) {
            for (int i = 0; i < count - 1; i++) {
                if (allNodes[i].rectTransform.position.x > allNodes[i + 1].rectTransform.position.x) {
                    Node aux = allNodes[i + 1];
                    allNodes[i + 1] = allNodes[i];
                    allNodes[i] = aux;
                    ordered = false;
                }
            }

            securityCounter++;
        }

        float totalMPS = baseMoneyPS;
        int starIndex = 0;
        for (int i = 0; i < count; i++) {
            if (allNodes[i].rectTransform.position.x < rectTransform.position.x) {
                totalMPS *= allNodes[i].upgradeDataSO.mpsMultiplierEstimated;
                totalMPS += allNodes[i].upgradeDataSO.mpsAddedEstimated;
            }
            if (allNodes[i] == this) {
                starIndex = i;
                break;
            }
        }


        if (starIndex + 1 < allNodes.Count) {
            float timeNeeded = allNodes[starIndex + 1].upgradeDataSO.cost / totalMPS;
            this.moneyPS.text =$"m/s: {totalMPS.ToString("F1")}";
            this.timeNeeded.text =$"Tiempo para la siguiente mejora: {timeNeeded.ToString("F0")} s";
        }

        //Hay dos cosas para lo que querría ver esto, es para ver dónde estoy, calcular cuánto producimos teóricamente por segundo, y cuánto tiempo me haría falta para comprar la siguiente mejora.
        //Para calcular cuánto produzco ahora, necesito saber el valor base, y cuánto espero que multiplique el moneyPerSecond cada mejora, así que sería coger todas las mejoras, ordenarlas por precio (no voy a calcular cuánto sería si ignoras algunas mejoras), y multiplicar/sumar la cantidad que vayamos a conseguir por segundo de cada una.
    }
}
