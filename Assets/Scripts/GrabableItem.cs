using System.Collections.Generic;
using UnityEngine;

public class GrabableItem : Interactable {

    public ItemType type;
    public Rigidbody rb;
    public GrabableItem geodeParent;
    public Vector3 pointOfInteraction;
    public List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    public Sprite itemSprite;
    public int stackWeight = 1;
    public bool canBeGrabbed = true;

    private void OnDisable() {
        Inventory.RemoveItemFromInventory(this);
    }

    [ContextMenu("Get Mesh References")]
    public void GetMeshReferences() {
        meshRenderers.Clear();
        meshRenderers.Add(GetComponent<MeshRenderer>());
    }

    public void Hide() {
        for (int i = 0; i < meshRenderers.Count; i++) {
            meshRenderers[i].enabled = false;
        }

        if (rb != null) {
            rb.isKinematic = true;
        }

        canBeGrabbed = false;
    }

    public void Show() {
        for (int i = 0; i < meshRenderers.Count; i++) {
            meshRenderers[i].enabled = true;
        }

        if (rb != null) {
            rb.isKinematic = false;
        }

        canBeGrabbed = true;
    }

    public override bool Interact() {
        if (!canBeGrabbed) { return false; }

        GrabableItem resultGrabable = this;

        if (geodeParent != null) { //If it's inside geode grab the geode
            resultGrabable = geodeParent;
        }

        if (!Inventory.TryAddToInventory(resultGrabable)) {
            return false;
            /* Feedback of inventory full */
        } else {
            return true;
        }
    }
}

public enum ItemType {
    Geode1 = 0, Geode2 = 1, Geode3 = 2, Geode4 = 3, Geode5 = 4, Geode6 = 5, Geode7 = 6, Geode8 = 7, Geode9 = 8, Geode10 = 9,
    Valuable1 = 10, Valuable2 = 11, Valuable3 = 12, Valuable4 = 13, Valuable5 = 14, Valuable6 = 15, Valuable7 = 16, Valuable8 = 17, Valuable9 = 18, Valuable10 = 19,
}
