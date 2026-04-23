using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectableItemForController : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler {

    public bool canJumpToTheOtherSide = true;
    public MenuWithSelectableItems menu;
    public RectTransform rectTransform;
    public Image imageThatChangesWithHover;
    public Sprite spriteBase;
    public Sprite spriteHovered;
    public Vector3 defaultScale = Vector3.one;
    public float hoveredScale = 1.05f;
    public bool unHoverWhenUsed = false;

    protected Coroutine c_Scaling;
    protected bool _wasHoveredLastFrame;

    public virtual bool isHovered => menu.hoveredItem == this;

    protected virtual void Update() {

        if (isHovered && !_wasHoveredLastFrame) {
            _wasHoveredLastFrame = true;
            imageThatChangesWithHover.sprite = spriteHovered;
            Scaling(moreScale: true);

        } else if (!isHovered && _wasHoveredLastFrame) {
            _wasHoveredLastFrame = false;
            imageThatChangesWithHover.sprite = spriteBase;
            Scaling(moreScale: false);
        }
    }

    public virtual void Use() {
        if (unHoverWhenUsed) {
            UnHover();
        }
    }

    public virtual void Hover() {
        menu.HoverItem(this);
    }

    public virtual void UnHover() {
        menu.hoveredItem = null;
    }

    #region Interface

    public virtual void OnPointerDown(PointerEventData eventData) {
        Use();
    }

    public virtual void OnPointerEnter(PointerEventData eventData) {
        Hover();
    }

    public virtual void OnPointerExit(PointerEventData eventData) {
        UnHover();
    }

    #endregion

    protected void Scaling(bool moreScale) {
        if (c_Scaling != null) {
            StopCoroutine(c_Scaling);
        }

        c_Scaling = StartCoroutine(C_Scaling(moreScale));
    }

    protected IEnumerator C_Scaling(bool moreScale) {

        if (moreScale) {

            rectTransform.localScale = defaultScale;
            while (rectTransform.localScale != defaultScale * hoveredScale) {
                rectTransform.localScale = Vector3.MoveTowards(rectTransform.localScale, defaultScale * hoveredScale, Time.unscaledDeltaTime * 5);
                yield return null;
            }

        } else {
            while (rectTransform.localScale != defaultScale) {
                rectTransform.localScale = Vector3.MoveTowards(rectTransform.localScale, defaultScale, Time.unscaledDeltaTime * 5);
                yield return null;
            }
        }
    }

    public void OnSelect(BaseEventData eventData) {
        menu.HoverItem(this);
    }

    public void OnDeselect(BaseEventData eventData) {
        menu.hoveredItem = null;
    }
}
