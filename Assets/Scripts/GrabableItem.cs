using System.Collections.Generic;
using UnityEngine;

public class GrabableItem : MonoBehaviour {

    public ItemType type;
    public Rigidbody rb;
    public GrabableItem geodeParent;
    public Vector3 pointOfInteraction;
    public List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    public Sprite itemSprite;
    public float stackWeight = 1;
    public virtual bool CanBeGrabbed => true;

    [ContextMenu("Get Mesh References")]
    public void GetMeshReferences() {
        meshRenderers.Clear();
        meshRenderers.Add(GetComponent<MeshRenderer>());
    }

    public void Hide() {
        for (int i = 0; i < meshRenderers.Count; i++) {
            meshRenderers[i].enabled = false;
        }
    }

    public void Show() {
        for (int i = 0; i < meshRenderers.Count; i++) {
            meshRenderers[i].enabled = true;
        }
    }
}

public enum ItemType {
    Geode1 = 0, Geode2 = 1, Geode3 = 2, Geode4 = 3, Geode5 = 4, Geode6 = 5, Geode7 = 6, Geode8 = 7, Geode9 = 8, Geode10 = 9,
    Valuable1 = 10, Valuable2 = 11, Valuable3 = 12, Valuable4 = 13, Valuable5 = 14, Valuable6 = 15, Valuable7 = 16, Valuable8 = 17, Valuable9 = 18, Valuable10 = 19,
}
