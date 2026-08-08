using UnityEngine;
using UnityEngine.InputSystem;

public class FurniturePlacementCancel : MonoBehaviour
{
    [Header("Referencias")]
    public FurniturePlacement placement;
    public FurniturePreview preview;

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

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
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

        placement.FinalizarColocacion();

        Destroy(mueble);

        Debug.Log("Colocacion de mueble cancelada.");
    }
}