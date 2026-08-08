using UnityEngine;
using UnityEngine.InputSystem;

public class FurnitureDelete : MonoBehaviour
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

        if (
            Keyboard.current.deleteKey
                .wasPressedThisFrame
        )
        {
            EliminarSeleccionado();
        }
    }

    public void EliminarSeleccionado()
    {
        GameObject mueble =
            selection.muebleSeleccionado;

        if (mueble == null)
            return;

        // Si es un asiento ocupado,
        // ponemos al jugador de pie
        // exactamente donde estaba el mueble.
        FurnitureSeat asiento =
            mueble.GetComponent<FurnitureSeat>();

        if (
            asiento != null &&
            asiento.EstaOcupado
        )
        {
            asiento
                .LevantarOcupanteEnPosicionDelMueble();
        }

        GridObstacle obstaculo =
            mueble.GetComponent<GridObstacle>();

        // Liberamos inmediatamente
        // las casillas del grid.
        if (obstaculo != null)
        {
            obstaculo.LiberarCasillas();
        }

        selection.muebleSeleccionado =
            null;

        Destroy(mueble);

        Debug.Log(
            "Mueble eliminado correctamente."
        );
    }
}