using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class FurniturePlacementConfirm : MonoBehaviour
{
    [Header("Referencias")]
    public FurniturePlacement placement;
    public FurniturePlacementValidator validator;
    public FurniturePreview preview;
    public GridManager grid;
    public GridOccupancy occupancy;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (
            placement == null ||
            !placement.EstaColocando
        )
        {
            return;
        }

        if (
            Mouse.current.leftButton
                .wasPressedThisFrame
        )

        if (
    EventSystem.current != null &&
    EventSystem.current.IsPointerOverGameObject()
)
{
    return;
}
        {
            IntentarColocar();
        }
    }

    private void IntentarColocar()
    {
        placement.RefrescarPosicionDesdeMouse();

        if (!validator.PuedeColocarActual())
        {
            Debug.Log(
                "No se puede colocar el mueble aqui."
            );

            return;
        }

        if (
            !placement.ObtenerCasillaAncla(
                out Vector2Int ancla
            )
        )
        {
            return;
        }

        GameObject mueble =
            placement.muebleActual;

        GridObstacle obstaculo =
            mueble.GetComponent<GridObstacle>();

        if (obstaculo == null)
        {
            obstaculo =
                mueble.AddComponent<GridObstacle>();
        }

        obstaculo.grid = grid;
        obstaculo.occupancy = occupancy;
        obstaculo.datos =
            mueble.GetComponent<FurnitureData>();

        obstaculo.registrarAlIniciar = false;
        obstaculo.enabled = true;

        if (!obstaculo.RegistrarDesdeAncla(ancla))
            return;

        if (preview != null)
        {
            preview.LimpiarPreview();
        }

        placement.FinalizarColocacion();

        Debug.Log("Mueble colocado correctamente.");
    }
}