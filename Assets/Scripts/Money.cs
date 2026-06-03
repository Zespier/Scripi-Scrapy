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

    public bool CanBuy(float amount) {
        return currentMoney >= amount;
    }

    public void AddMoney(float amount) {
        AudioManager.instance.PlayPop();
        currentMoney += amount;
        SaveSystem.statistics.moneyGained += amount;
        moneyText.text = $"{currentMoney.ToString("F0")}$";
    }

    public void SpendMoney(float amount) {
        currentMoney -= amount;
        SaveSystem.statistics.moneySpent += amount;
    }

    public void LoadMoney(SaveData saveData) {
        currentMoney = saveData.money;
    }
}
