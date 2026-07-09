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

    //Este va a ser el proceso, quiero desabilitar el item por completo, voy a ver en qué partes hace falta que esté activo el objeto, solo hacen falta para la aspiradora, el sell spot, y para el daño en area de las geodas, así qeu me da igual si no están enActive, porque ninguna de esas tres cosas quiero que interactuen con los objetos del inventario.
    //Lo único malo es cómo hago que aparezca el item en la mano, porque es verdad que ese "está" en el inventario.

    public void RemoveFunctionality() {
        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        canBeGrabbed = false;

        if (this is SellableItem sellable) {
            Active.RemoveSellableItem(sellable);
            sellable.canBeSelled = false;

        } else if (this is Geode geode) {
            Active.RemoveGeode(geode);
        }
    }

    public void RestoreFunctionality() {
        rb.isKinematic = false;

        canBeGrabbed = true;

        if (this is SellableItem sellable) {
            Active.AddSellableItem(sellable);
            sellable.canBeSelled = true;

        } else if (this is Geode geode) {
            Active.AddGeode(geode);
        }
    }

    public void Hide() {
        gameObject.SetActive(false);
        //for (int i = 0; i < meshRenderers.Count; i++) {
        //    meshRenderers[i].enabled = false;
        //}

        //if (rb != null) {
        //    rb.isKinematic = true;
        //}

        //canBeGrabbed = false;
    }

    public void Show() {
        //Entonces, deasctivamos el resto, el que esta en la mano lo activamos, pero lo ponemos como que no se puede interactuar ni nada conel, metodo de desabilitar todas sus funciones, pero cuando vuelve al inventario o sale, se vuelve a activar sus funciones. Y ya no?
        //Esto además vuelve a meterlo en la lista de activeItems, pero sería un problema para el objeto que tenemos en la mano.
        gameObject.SetActive(true);

        //for (int i = 0; i < meshRenderers.Count; i++) {
        //    meshRenderers[i].enabled = true;
        //}

        //if (rb != null) {
        //    rb.isKinematic = false;
        //}

        //canBeGrabbed = true;
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
