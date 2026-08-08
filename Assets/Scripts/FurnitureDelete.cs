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

        // No eliminar mientras estamos colocando/moviendo.
        if (placement.EstaColocando)
            return;

        if (
            Keyboard.current.deleteKey.wasPressedThisFrame
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

        GridObstacle obstaculo =
            mueble.GetComponent<GridObstacle>();

        // Primero liberamos las casillas.
        if (obstaculo != null)
        {
            obstaculo.LiberarCasillas();
        }

        selection.muebleSeleccionado = null;

        Destroy(mueble);

        Debug.Log(
            "Mueble eliminado correctamente."
        );
    }
}