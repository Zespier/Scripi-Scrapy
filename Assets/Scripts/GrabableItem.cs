using System.Collections.Generic;
using UnityEngine;

public class GrabableItem : MonoBehaviour {

    public Rigidbody rb;
    public GrabableItem geodeParent;
    public Vector3 pointOfInteraction;
    public List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

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
