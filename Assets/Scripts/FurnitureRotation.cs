using UnityEngine;
using UnityEngine.InputSystem;

public class FurnitureRotation : MonoBehaviour
{
    [Header("Referencias")]
    public FurniturePlacement placement;

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (
            placement == null ||
            placement.muebleActual == null
        )
        {
            return;
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            RotarMuebleActual();
        }
    }

    private void RotarMuebleActual()
    {
        FurnitureData datos =
            placement.muebleActual
                .GetComponent<FurnitureData>();

        if (datos == null)
            return;

        datos.Rotar();

        placement.RefrescarPosicionDesdeMouse();
    }
}