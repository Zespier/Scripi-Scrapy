using TMPro;
using UnityEngine;

public class Money : MonoBehaviour {

    //Se me ocurre para la animacion, que 

    public TMP_Text moneyText;
    public float currentMoney;

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
