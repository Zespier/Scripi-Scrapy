using System.Collections;
using TMPro;
using UnityEngine;

public class Money : MonoBehaviour {

    public TMP_Text moneyText;
    public TMP_Text comboText;
    public TMP_Text moneyMultiplierText;
    public long currentMoney;
    public float timeToStopCombo = 2;
    public float scaleAmount = 1.6f;

    private int _combo;
    private float _comboTimer;
    private long _moneyGainedThisCombo;
    private Coroutine c_Bounce;

    public static Money instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    private void Update() {
        moneyText.text = $"{currentMoney.FormatNumber()}$";

        comboText.text = _combo > 0 ? $"Combo {_combo}" : "";
        moneyMultiplierText.text = _combo >= 10 ? $"x{(_combo / 10) + 1}" : "";

        if (_combo > 0 && Time.time > _comboTimer + timeToStopCombo) {
            ApplyComboMultiplier();
        }
    }

    private void ApplyComboMultiplier() {
        if (_combo >= 10) {
            int multiplier = _combo / 10;
            AddMoney(_moneyGainedThisCombo * multiplier);
        }

        _combo = 0;
        _moneyGainedThisCombo = 0;
    }

    public bool CanBuy(long amount) {
        return currentMoney >= amount;
    }

    public void AddMoney(long amount) {
        currentMoney += amount;
        SaveSystem.statistics.moneyGained += amount;
        Bounce(moneyText);
    }

    public void AddMoneyBySellableItem(long amount) {
        AudioManager.instance.PlayPop();

        _combo++;
        Bounce(comboText);
        _comboTimer = Time.time;

        _moneyGainedThisCombo += amount;

        AddMoney(amount);
    }

    public void SpendMoney(long amount) {
        currentMoney -= amount;
        SaveSystem.statistics.moneySpent += amount;
    }

    public void LoadMoney(SaveData saveData) {
        currentMoney = saveData.money;
    }

    private void Bounce(TMP_Text text) {
        if (c_Bounce != null) {
            StopCoroutine(c_Bounce);
        }

        c_Bounce = StartCoroutine(C_Bounce(text));
    }

    private IEnumerator C_Bounce(TMP_Text text) {

        float timeOfExpansion = 0.07f;

        float timer = Time.time;

        while (Time.time < timer + timeOfExpansion) {
            text.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * scaleAmount, (Time.time - timer) / timeOfExpansion);
            yield return null;
        }

        float timeToRecover = 0.2f;

        timer = Time.time;

        while (Time.time < timer + timeOfExpansion) {
            text.transform.localScale = Vector3.Lerp(Vector3.one * scaleAmount, Vector3.one, (Time.time - timer) / timeToRecover);
            yield return null;
        }

        text.transform.localScale = Vector3.one;
    }
}
