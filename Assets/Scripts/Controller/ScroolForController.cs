using Unity.VisualScripting;
using UnityEngine;

public class ScroolForController : SelectableItemForController {

    public float maxContainerSize = 600;
    public override bool isHovered => false;

    public bool IsHovered() {
        //Con mouse que pase por encima,
        //Con mando que se esté moviendo y ya.

        return false;
    }

    public void CreateSliderIfDescriptionIsTooLong() {
        //description.ForceMeshUpdate();
        //Vector2 size = description.GetRenderedValues();
        //if (size.y > maxContainerSize) {
        //    //Hacer que el slider aparezca, y que el container se mueva con el joystick y el click del rat�n. Preferir�a que no sea de Unity el scroll
        //}
    }

    public void MoverElContainer() {
        //Po si se mueve la imagen se mueve el container xd.
        //Para calcular tengo que ver la altura total que ocuparía el slider en su máximo esplendor, y todo lo que se aleje la parte de arriba del slider de su máxima altura, es todo lo que se mueve el container.
    }
}
