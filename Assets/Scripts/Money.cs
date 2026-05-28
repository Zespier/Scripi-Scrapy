using TMPro;
using UnityEngine;

public class Money : MonoBehaviour {

    public TMP_Text moneyText;
    public float currentMoney = 100;

    public static Money instance;

    private void Awake() {
        if (!instance) { instance = this; }
    }

    public void AddMoney(float amount) {
        AudioManager.instance.PlayPop();
        currentMoney += amount;
        moneyText.text = $"{currentMoney.ToString("F0")}$";
    }
}
