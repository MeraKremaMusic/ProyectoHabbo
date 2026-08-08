using UnityEngine;
using UnityEngine.InputSystem;

public class FurnitureMove : MonoBehaviour
{
    [Header("Referencias")]
    public FurnitureSelection selection;
    public FurniturePlacement placement;

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (
            selection == null ||
            placement == null
        )
        {
            return;
        }

        if (placement.EstaColocando)
            return;

        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            MoverSeleccionado();
        }
    }

    private void MoverSeleccionado()
    {
        GameObject mueble =
            selection.muebleSeleccionado;

        if (mueble == null)
            return;

        GridObstacle obstaculo =
            mueble.GetComponent<GridObstacle>();

        // Primero liberamos las casillas antiguas.
        if (obstaculo != null)
        {
            obstaculo.LiberarCasillas();
        }

        // El mismo mueble vuelve al sistema
        // de colocacion.
        placement.muebleActual = mueble;

        // Ya no está seleccionado como mueble colocado.
        selection.muebleSeleccionado = null;

        // Lo colocamos inmediatamente bajo el mouse.
        placement.RefrescarPosicionDesdeMouse();

        Debug.Log(
            "Moviendo mueble: " +
            mueble.name
        );
    }
}