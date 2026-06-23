using TMPro;
using UnityEngine;

public class Money : MonoBehaviour {

    public TMP_Text moneyText;
    public long currentMoney;

    public static Money instance;

    private void Awake() {
        if (!instance) { instance = this; }
    }

    private void Update() {
        moneyText.text = $"{currentMoney.FormatNumber()}$";
    }

    public bool CanBuy(long amount) {
        return currentMoney >= amount;
    }

    public void AddMoney(long amount) {
        AudioManager.instance.PlayPop();
        currentMoney += amount;
        SaveSystem.statistics.moneyGained += amount;
    }

    public void SpendMoney(long amount) {
        currentMoney -= amount;
        SaveSystem.statistics.moneySpent += amount;
    }

    public void LoadMoney(SaveData saveData) {
        currentMoney = saveData.money;
    }
}
