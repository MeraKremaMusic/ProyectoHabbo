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
    public FurnitureMove furnitureMove;

    private GameObject muebleDetectado;
    private bool listoParaConfirmar;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (
            placement == null ||
            !placement.EstaColocando
        )
        {
            muebleDetectado = null;
            listoParaConfirmar = false;

            return;
        }

        // Detectamos cuándo acaba de aparecer
        // un mueble nuevo en el cursor.
        if (
            muebleDetectado !=
            placement.muebleActual
        )
        {
            muebleDetectado =
                placement.muebleActual;

            listoParaConfirmar =
                false;

            return;
        }

        // Evita que el clic utilizado para
        // elegir el mueble también lo coloque.
        if (!listoParaConfirmar)
        {
            if (
                !Mouse.current.leftButton
                    .isPressed
            )
            {
                listoParaConfirmar =
                    true;
            }

            return;
        }

        // No colocar muebles al hacer
        // clic sobre la interfaz.
        if (
            EventSystem.current != null &&
            EventSystem.current
                .IsPointerOverGameObject()
        )
        {
            return;
        }

        if (
            Mouse.current.leftButton
                .wasPressedThisFrame
        )
        {
            IntentarColocar();
        }
    }

    private void IntentarColocar()
    {
        if (
            placement == null ||
            validator == null
        )
        {
            return;
        }

        placement.RefrescarPosicionDesdeMouse();

        if (!validator.PuedeColocarActual())
        {
            Debug.Log(
                "No se puede colocar " +
                "el mueble aqui."
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

        if (mueble == null)
            return;

        GridObstacle obstaculo =
            mueble.GetComponent<GridObstacle>();

        if (obstaculo == null)
        {
            obstaculo =
                mueble.AddComponent<GridObstacle>();
        }

        obstaculo.grid =
            grid;

        obstaculo.occupancy =
            occupancy;

        obstaculo.datos =
            mueble.GetComponent<FurnitureData>();

        obstaculo.registrarAlIniciar =
            false;

        obstaculo.enabled =
            true;

        if (
            !obstaculo.RegistrarDesdeAncla(
                ancla
            )
        )
        {
            return;
        }

        if (preview != null)
        {
            preview.LimpiarPreview();
        }

        // Si estábamos moviendo un mueble
        // existente, confirmamos su nueva posición.
        if (furnitureMove != null)
        {
            furnitureMove
                .ConfirmarMovimiento(mueble);
        }

        placement.FinalizarColocacion();

        muebleDetectado =
            null;

        listoParaConfirmar =
            false;

        Debug.Log(
            "Mueble colocado correctamente."
        );
    }
}