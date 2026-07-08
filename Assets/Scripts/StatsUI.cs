using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsUI : MonoBehaviour {
    public CanvasGroup canvasGroup;
    public List<TMP_Text> texts;

    private void Awake() {
        ActiveCanvasGroup(false);
    }

    private void Update() {

        if (InputManager.GameControls.Character.ToggleStats.WasPressedThisFrame()) {
            ActiveCanvasGroup(true);

        } else if (InputManager.GameControls.Interface.ToggleStats.WasPressedThisFrame()) {
            ActiveCanvasGroup(false);
        }

        for (int i = 0; i < texts.Count; i++) {
            switch (i) {
                case 0:
                    texts[i].text = $"Manual Hits: {SaveSystem.statistics.manualHits}";
                    break;
                case 1:
                    texts[i].text = $"Automatic Hits: {SaveSystem.statistics.automaticHits}";
                    break;
                case 2:
                    texts[i].text = $"Collateral Hits: {SaveSystem.statistics.collateralHits}";
                    break;
                case 3:
                    texts[i].text = $"Total Hits: {SaveSystem.statistics.totalHits}";
                    break;
                case 4:
                    texts[i].text = $"Geodes Generated: {SaveSystem.statistics.geodesGenerated}";
                    break;
                case 5:
                    texts[i].text = $"Geodes Broken: {SaveSystem.statistics.geodesBroken}";
                    break;
                case 6:
                    texts[i].text = $"Geodes Broken by collateral damage: {SaveSystem.statistics.geodesBrokenByCollateralDamage}";
                    break;
                case 7:
                    texts[i].text = $"Geodes Broken with automatic gear: {SaveSystem.statistics.geodesBrokenWithAutomaticGear}";
                    break;
                case 8:
                    texts[i].text = $"Items selled: {SaveSystem.statistics.itemsSelled}";
                    break;
                case 9:
                    texts[i].text = $"Money Gained: {SaveSystem.statistics.moneyGained}";
                    break;
                case 10:
                    texts[i].text = $"Money spent: {SaveSystem.statistics.moneySpent}";
                    break;
                default:
                    break;
            }
        }
    }

    private void ActiveCanvasGroup(bool active) {
        canvasGroup.alpha = active ? 1.0f : 0;
        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }
}
