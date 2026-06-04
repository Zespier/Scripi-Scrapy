using TMPro;
using UnityEngine;

public class Money : MonoBehaviour {

    public TMP_Text moneyText;
    public float currentMoney;

    public static Money instance;

    private void Awake() {
        if (!instance) { instance = this; }
    }

    private void Update() {
        moneyText.text = $"{currentMoney.ToString("F0")}$";
    }

    public bool CanBuy(float amount) {
        return currentMoney >= amount;
    }

    public void AddMoney(float amount) {
        AudioManager.instance.PlayPop();
        currentMoney += amount;
        SaveSystem.statistics.moneyGained += amount;
    }

    public void SpendMoney(float amount) {
        currentMoney -= amount;
        SaveSystem.statistics.moneySpent += amount;
    }

    public void LoadMoney(SaveData saveData) {
        currentMoney = saveData.money;
    }
}
