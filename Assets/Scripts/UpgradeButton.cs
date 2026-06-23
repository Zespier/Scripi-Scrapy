using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

    public RectTransform rectTransform;
    public UpgradeDataSO upgradeData;
    public Vector3 defaultScale = Vector3.one;
    public Image image;
    public Sprite lockedSprite;
    public bool beingChecked;

    private Coroutine c_Scaling;
    private Sprite defaultSprite;

    public bool IsUpgraded => upgradeData.isUpgraded;

    private void Awake() {
        defaultSprite = image.sprite;
    }

    private void Update() {
        image.sprite = IsUpgraded ? lockedSprite : defaultSprite;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (IsUpgraded) { return; }

        OnPointerExit(default);
        MenuUpgrades.instance.Upgrade(upgradeData);
    }

    public void OnPointerEnter(PointerEventData eventData) {

        MenuUpgrades.instance.outline.position = rectTransform.position;
        MenuUpgrades.instance.ShowDetails(upgradeData);
        beingChecked = true;

        Scaling(true);
    }

    public void OnPointerExit(PointerEventData eventData) {

        MenuUpgrades.instance.outline.position = new Vector3(1000000000, 0, 0);
        MenuUpgrades.instance.HideDetails();
        beingChecked = false;

        Scaling(false);
    }

    private void Scaling(bool moreScale) {
        if (c_Scaling != null) {
            StopCoroutine(c_Scaling);
        }

        c_Scaling = StartCoroutine(C_Scaling(moreScale));
    }

    private IEnumerator C_Scaling(bool moreScale) {
        if (moreScale) {

            MenuUpgrades.instance.outline.localScale = defaultScale;
            while (MenuUpgrades.instance.outline.localScale != defaultScale * 1.3f) {
                MenuUpgrades.instance.outline.localScale = Vector3.MoveTowards(MenuUpgrades.instance.outline.localScale, defaultScale * 1.3f, Time.unscaledDeltaTime * 10);
                yield return null;
            }

            rectTransform.localScale = defaultScale;
            while (rectTransform.localScale != defaultScale * 1.5f) {
                rectTransform.localScale = Vector3.MoveTowards(rectTransform.localScale, defaultScale * 1.5f, Time.unscaledDeltaTime * 23);
                yield return null;
            }

        } else {

            while (rectTransform.localScale != defaultScale) {
                rectTransform.localScale = Vector3.MoveTowards(rectTransform.localScale, defaultScale, Time.unscaledDeltaTime * 23);
                yield return null;
            }
        }
    }
}
