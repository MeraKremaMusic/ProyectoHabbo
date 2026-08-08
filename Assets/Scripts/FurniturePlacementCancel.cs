using UnityEngine;
using UnityEngine.InputSystem;

public class FurniturePlacementCancel : MonoBehaviour
{
    [Header("Referencias")]
    public FurniturePlacement placement;
    public FurniturePreview preview;
    public FurnitureMove furnitureMove;

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (
            placement == null ||
            !placement.EstaColocando
        )
        {
            return;
        }

        if (
            Keyboard.current.escapeKey
                .wasPressedThisFrame
        )
        {
            Cancelar();
        }
    }

    private void Cancelar()
    {
        GameObject mueble =
            placement.muebleActual;

        if (mueble == null)
            return;

        if (preview != null)
        {
            preview.LimpiarPreview();
        }

        // Si era un mueble que ya existía,
        // ESC lo devuelve a su posición original.
        if (
            furnitureMove != null &&
            furnitureMove.EsMuebleEnMovimiento(mueble)
        )
        {
            furnitureMove.CancelarMovimiento();

            placement.FinalizarColocacion();

            return;
        }

        // Si era un mueble NUEVO del inventario,
        // ESC sí lo elimina.
        placement.FinalizarColocacion();

        Destroy(mueble);

        Debug.Log(
            "Colocacion de mueble nuevo cancelada."
        );
    }
}