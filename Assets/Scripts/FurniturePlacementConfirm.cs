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

        // Detectamos cuando acaba de aparecer un mueble nuevo.
        // Esto evita que el clic del inventario también lo coloque.
        if (muebleDetectado != placement.muebleActual)
        {
            muebleDetectado = placement.muebleActual;
            listoParaConfirmar = false;
            return;
        }

        // Esperamos a que el mouse esté completamente suelto
        // antes de aceptar el próximo clic.
        if (!listoParaConfirmar)
        {
            if (!Mouse.current.leftButton.isPressed)
            {
                listoParaConfirmar = true;
            }

            return;
        }

        // Los clics sobre la interfaz no colocan muebles.
        if (
            EventSystem.current != null &&
            EventSystem.current.IsPointerOverGameObject()
        )
        {
            return;
        }

        // SOLO aquí se coloca:
        // con un nuevo clic izquierdo del usuario.
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            IntentarColocar();
        }
    }

    private void IntentarColocar()
    {
        placement.RefrescarPosicionDesdeMouse();

        if (
            validator == null ||
            !validator.PuedeColocarActual()
        )
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

        muebleDetectado = null;
        listoParaConfirmar = false;

        Debug.Log(
            "Mueble colocado correctamente."
        );
    }
}