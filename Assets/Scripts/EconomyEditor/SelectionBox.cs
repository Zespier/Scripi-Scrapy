using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectionBox : MonoBehaviour {

    public Camera cam;
    public Image visuals;
    public RectTransform rectTransform;
    public bool isSelecting;

    private Vector2 initialPosition;
    private Vector2 _defaultScreenSizeOffset = new Vector2(1920 / 2f, 1080 / 2f);

    public Vector2 MouseGlobalPosition => GetMouseGlobalPosition();

    public static SelectionBox instance;
    private void Awake() {
        if (!instance) { instance = this; }
    }

    private void Update() {
        if (Keyboard.current.leftShiftKey.isPressed) {
            if (!isSelecting) {

                initialPosition = MouseGlobalPosition;
                isSelecting = true;
            }

            ExtendSelectionBox();


        } else if (isSelecting) {

            isSelecting = false;
            visuals.enabled = false;
            ConfirmSelection();
        }
    }

    private void ExtendSelectionBox() {
        visuals.enabled = true;

        rectTransform.position = initialPosition + (MouseGlobalPosition - initialPosition) / 2f;

        rectTransform.sizeDelta = new Vector2(Mathf.Abs(MouseGlobalPosition.x - initialPosition.x), Mathf.Abs(MouseGlobalPosition.y - initialPosition.y));
    }

    private Vector2 GetMouseGlobalPosition() {
        return (cam.orthographicSize * 2f / 1080f) * (Mouse.current.position.ReadValue() - _defaultScreenSizeOffset) + EconomyCreationCamera.instance.Position;
    }

    private void ConfirmSelection() {
        List<Node> allNodes = HandEconomyCreation.instance.allNodes;

        Bounds selectionBounds = new Bounds(rectTransform.position, rectTransform.sizeDelta);

        for (int i = 0; i < allNodes.Count; i++) {
            Node node = allNodes[i];

            if (selectionBounds.Intersects(node.Bounds)) {
                node.Select();
            }
        }
    }
}
