using System.Collections.Generic;
using UnityEngine;

public class GrabableItem : Interactable {

    public ItemType type;
    public Rigidbody rb;
    public Vector3 pointOfInteraction;
    public Sprite itemSprite;
    public int stackWeight = 1;
    public GrabableState state = GrabableState.None;

    //Este va a ser el proceso, quiero desabilitar el item por completo, voy a ver en qué partes hace falta que esté activo el objeto, solo hacen falta para la aspiradora, el sell spot, y para el daño en area de las geodas, así qeu me da igual si no están enActive, porque ninguna de esas tres cosas quiero que interactuen con los objetos del inventario.
    //Lo único malo es cómo hago que aparezca el item en la mano, porque es verdad que ese "está" en el inventario.

    public override bool Interact() {
        if (state != GrabableState.None) { return false; }

        //TODO: THere is no other grabable than Valuables for now
        return true;
    }
}

public enum GrabableState {
    None = 0,
    Suck = 1,
    Falling = 2,
}

public enum ItemType {
    Meteor1 = 0,
    Meteor2 = 1,
    Meteor3 = 2,
    Meteor4 = 3,
    Meteor5 = 4,
    Meteor6 = 5,
    Meteor7 = 6,
    Meteor8 = 7,
    Meteor9 = 8,
    Meteor10 = 9,

    Calcio = 10,
    Magnetita = 11,
    Topacio = 12,
    Malaquita = 13,
    Peridoto = 14,
    CuarzoRosa = 15,
    Opalo = 16,
    Agate = 17,
    Amatista = 18,
    Zafiro = 19,
    Rubi = 20,
    Halita = 21,
    Bismuto = 22,
}
