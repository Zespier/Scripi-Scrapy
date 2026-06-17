using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour {

    public RectTransform rectTransform;
    public UpgradeDataSO upgradeData;
    public Image image;
    public TMP_Text costText;

    private void Update() {
        image.sprite = upgradeData.sprite;
        costText.text = upgradeData.cost.ToString();
    }
}
