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

        // CASO 1:
        // El mueble ya existía y solamente
        // lo estábamos cambiando de posición.
        if (
            furnitureMove != null &&
            furnitureMove.EsMuebleEnMovimiento(
                mueble
            )
        )
        {
            furnitureMove.CancelarMovimiento();

            placement.FinalizarColocacion();

            return;
        }

        // CASO 2:
        // Es un mueble recién sacado
        // del inventario.
        placement.FinalizarColocacion();

        Destroy(mueble);

        Debug.Log(
            "Colocacion de mueble nuevo cancelada."
        );
    }
}