using UnityEngine;

public class FurnitureInteraction :
    MonoBehaviour
{
    private PlayerSitting jugador;

    private void Awake()
    {
        jugador =
            FindFirstObjectByType<PlayerSitting>();
    }

    public bool IntentarInteractuar(
        Vector2 posicionPantalla)
    {
        Camera camara =
            Camera.main;

        if (camara == null)
            return false;

        Ray ray =
            camara.ScreenPointToRay(
                posicionPantalla
            );

        if (
            !Physics.Raycast(
                ray,
                out RaycastHit hit,
                1000f
            )
        )
        {
            return false;
        }

        FurnitureSeat asiento =
            hit.collider
                .GetComponentInParent<FurnitureSeat>();

        if (asiento == null)
        {
            return false;
        }

        // Encontramos un mueble interactuable.
        // Por eso consumimos el clic aunque
        // por alguna razón no pueda sentarse.
        if (jugador != null)
        {
            jugador.IrASentarse(
                asiento
            );
        }

        return true;
    }
}