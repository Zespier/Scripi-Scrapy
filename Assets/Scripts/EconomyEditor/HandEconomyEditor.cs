using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HandEconomyCreation : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler {

    public Transform nodeContainer;
    public float additionalSelectionSizePerRatio = 1f;
    public Image hiddenSurfaceToDetectMouseActions;
    public int gridSize = 20;
    public Node nodePrefab;
    public StarNode starNodePrefab;
    public List<UpgradeDataSO> allUpgradesSO;
    public List<Node> allNodes = new();

    [HideInInspector] public Vector2 _defaultScreenSizeOffset = new Vector2(1920 / 2f, 1080 / 2f);
    private bool _isDragging;

    public static HandEconomyCreation instance;
    private void Awake() {
        if (!instance) { instance = this; }

        SpawnAllUpgradesSO();
    }

    private void Update() {

        float currentGlobalHeight = EconomyCreationCamera.instance.cam.orthographicSize * 2f;
        float ratio = currentGlobalHeight / 1080f;
        hiddenSurfaceToDetectMouseActions.rectTransform.sizeDelta = 100f * ratio * Vector2.one;
        hiddenSurfaceToDetectMouseActions.rectTransform.position = (Vector3)(ratio * (Mouse.current.position.ReadValue() - new Vector2(1920 / 2f, 1080 / 2f)) + EconomyCreationCamera.instance.Position);

        if (Keyboard.current.deleteKey.wasPressedThisFrame) {
            DeleteAllSelectedElements();
        } else if (Keyboard.current.nKey.wasPressedThisFrame) {
            CreateAnswer();
        } else if (Keyboard.current.fKey.wasPressedThisFrame) {
            PlaceVisualNodesOnGrid(onlySelected: false);
        }
    }

    private void SpawnAllUpgradesSO() {
        for (int i = 0; i < allUpgradesSO.Count; i++) {
            Node newNode = Instantiate(nodePrefab, nodeContainer);
            newNode.upgradeDataSO = allUpgradesSO[i];
            newNode.rectTransform.position = allUpgradesSO[i].savedPosition;
        }

        StarNode starNode = Instantiate(starNodePrefab, nodeContainer);
        starNode.rectTransform.localPosition = Vector3.zero;
    }

    public void OnPointerDown(PointerEventData eventData) {

        if (eventData.button == PointerEventData.InputButton.Right) {
            Node visualNode = GetVisualNodeOnMouse();

            Destroy(visualNode.gameObject);
        }
    }

    public void OnPointerUp(PointerEventData eventData) {

        if (eventData.button != PointerEventData.InputButton.Left) { return; }


        Node visualNodeOnMouse = GetVisualNodeOnMouse();

        if (!_isDragging) {

            UnSelectAll();
            if (visualNodeOnMouse != null) {
                visualNodeOnMouse.Select();
            }

        } else {
            PlaceVisualNodesOnGrid();
        }

        _isDragging = false;
    }

    public void OnBeginDrag(PointerEventData eventData) {

        if (eventData.button != PointerEventData.InputButton.Left) { return; }

        Node visualNodeOnMouse = GetVisualNodeOnMouse();

        if (visualNodeOnMouse != null) {

            UnSelectAll();
            visualNodeOnMouse.Select();
            _isDragging = true;
            return;

        }

        _isDragging = false;
    }

    public void OnDrag(PointerEventData eventData) {

        if (!_isDragging && !SelectionBox.instance.isSelecting) {
            EconomyCreationCamera.instance.SetDelta(eventData.delta);
            return;
        }

        if (_isDragging) {

            for (int i = 0; i < allNodes.Count; i++) {
                if (allNodes[i].IsSelected) {
                    allNodes[i].rectTransform.position += EconomyCreationCamera.instance.cam.orthographicSize * 2f / 1080f * (Vector3)eventData.delta;
                }
            }
        }
    }

    public Vector2 GetMouseGlobalPosition() {
        return (EconomyCreationCamera.instance.cam.orthographicSize * 2f / 1080f) * (Mouse.current.position.ReadValue() - _defaultScreenSizeOffset) + EconomyCreationCamera.instance.Position;
    }

    public void UnSelectAll() {
        for (int i = 0; i < allNodes.Count; i++) {
            allNodes[i].Unselect();
        }
    }

    public int GetNumberOfPanelsSelected() {
        int total = 0;
        for (int i = 0; i < allNodes.Count; i++) {
            if (allNodes[i].IsSelected) {
                total++;
            }
        }

        return total;
    }

    private void PlaceVisualNodesOnGrid(bool onlySelected = true) {
        Vector3 newPosition;
        for (int i = 0; i < allNodes.Count; i++) {
            if (onlySelected && !allNodes[i].IsSelected) {
                continue;
            }

            newPosition = allNodes[i].rectTransform.position;
            newPosition.x = RoundToMultipleOf(newPosition.x, gridSize);
            newPosition.y = RoundToMultipleOf(newPosition.y, gridSize);
            newPosition.z = 0;
            allNodes[i].rectTransform.position = newPosition;
        }
    }

    private float RoundToMultipleOf(float value, float roundingValue) {
        if (roundingValue == 0f) {
            return value;
        }

        return Mathf.Round(value / roundingValue) * roundingValue;
    }

    private void DeleteAllSelectedElements() {
        for (int i = 0; i < allNodes.Count; i++) {
            if (allNodes[i].IsSelected) {
                Destroy(allNodes[i].gameObject);
                i--;
            }
        }
    }

    public void CreateAnswer() {
        Node answer = Instantiate(nodePrefab, transform.parent);
        answer.rectTransform.position = EconomyCreationCamera.instance.Position;
    }

    public void AddNode(Node visualNode) {
        if (!allNodes.Contains(visualNode)) {
            allNodes.Add(visualNode);
        }
    }

    public void RemoveNode(Node visualNode) {
        if (allNodes.Contains(visualNode)) {
            allNodes.Remove(visualNode);
        }
    }

    private Node GetVisualNodeOnMouse() {
        for (int i = 0; i < allNodes.Count; i++) {
            if (allNodes[i].Bounds.Contains(GetMouseGlobalPosition())) {

                return allNodes[i];
            }
        }
        return null;
    }
}
//TODO: Task list shows the number of this line, I'm interested in seeing the total amount of lines my game has, so I will put this in the last line of every script I find.
