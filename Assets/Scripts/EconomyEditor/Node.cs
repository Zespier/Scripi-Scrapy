using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour {

    public RectTransform rectTransform;
    public RectTransform boundsSize;
    public Outline outline;
    public Image image;
    public new TMP_Text name;
    public TMP_Text cost;
    public UpgradeDataSO upgradeDataSO;

    public Bounds Bounds => GetBounds();
    public bool IsSelected => outline.enabled;

    protected virtual void OnEnable() {
        HandEconomyCreation.instance.AddNode(this);
    }

    protected virtual void OnDisable() {
        HandEconomyCreation.instance.RemoveNode(this);
    }

    protected virtual void Update() {
        if (upgradeDataSO != null) {
            image.sprite = upgradeDataSO.sprite;
            name.text = upgradeDataSO.name;
            cost.text = upgradeDataSO.cost.ToString();
        }

        Vector3 newPosition = rectTransform.position;
        newPosition.y = 0;
        rectTransform.position = newPosition;
    }

    private void OnDestroy() {
        SavePosition();
    }

    private void SavePosition() {
        if (upgradeDataSO != null) {
            upgradeDataSO.savedPosition = rectTransform.position;
        }
    }

    protected virtual Bounds GetBounds() {
        if (EconomyCreationCamera.instance == null) {
            return new Bounds(rectTransform.position, new Vector3(boundsSize.rect.width, boundsSize.rect.height));
        }

        float currentGlobalHeight = EconomyCreationCamera.instance.cam.orthographicSize * 2f;
        float ratio = currentGlobalHeight / 1080f;
        float additionalBounds = HandEconomyCreation.instance.additionalSelectionSizePerRatio * ratio;

        return new Bounds(rectTransform.position, new Vector3(
            boundsSize.rect.width + additionalBounds * 2,
            boundsSize.rect.height + additionalBounds * 2));
    }

    public virtual void Select() {
        float currentGlobalHeight = EconomyCreationCamera.instance.cam.orthographicSize * 2f;
        float ratio = currentGlobalHeight / 1080f;

        outline.enabled = true;
        outline.effectDistance = ratio * new Vector2(4, 4);
    }

    protected virtual void Select(string strin) {
        Select();
    }

    public virtual void Unselect() {
        if (outline.enabled) {
            outline.enabled = false;
        }
    }
}