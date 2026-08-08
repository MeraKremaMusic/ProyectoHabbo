using UnityEngine;

public class FurnitureSeat : MonoBehaviour
{
    [Header("Configuracion del asiento")]

    [Tooltip(
        "Activa esto si el frente detectado " +
        "queda apuntando hacia el respaldo."
    )]
    public bool invertirFrente = false;

    [Tooltip(
        "Pequeno ajuste del jugador hacia " +
        "adelante o atras sobre el asiento."
    )]
    [Range(-0.5f, 0.5f)]
    public float ajusteProfundidad = 0f;

    [Header("Visualizacion")]
    public float distanciaVisualAproximacion = 1f;

    public Vector3 DireccionFrontal
    {
        get
        {
            Vector3 direccion =
                transform.forward;

            if (invertirFrente)
            {
                direccion = -direccion;
            }

            direccion.y = 0f;

            return direccion.normalized;
        }
    }

    public Vector3 ObtenerPosicionSentado(
        float alturaJugador)
    {
        Vector3 posicion =
            transform.position;

        posicion +=
            DireccionFrontal *
            ajusteProfundidad;

        posicion.y =
            alturaJugador;

        return posicion;
    }

    public bool ObtenerCasillaAproximacion(
        GridManager grid,
        out Vector2Int casilla)
    {
        casilla = Vector2Int.zero;

        if (grid == null)
            return false;

        if (
            !grid.ObtenerCasilla(
                transform.position,
                out Vector2Int casillaSilla
            )
        )
        {
            return false;
        }

        Vector3 frente =
            DireccionFrontal;

        Vector2Int direccionCasilla;

        // Elegimos el eje dominante.
        if (
            Mathf.Abs(frente.x) >
            Mathf.Abs(frente.z)
        )
        {
            direccionCasilla =
                new Vector2Int(
                    frente.x >= 0f ? 1 : -1,
                    0
                );
        }
        else
        {
            direccionCasilla =
                new Vector2Int(
                    0,
                    frente.z >= 0f ? 1 : -1
                );
        }

        casilla =
            casillaSilla +
            direccionCasilla;

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 centro =
            transform.position +
            Vector3.up * 0.15f;

        // Punto donde quedara el jugador.
        Gizmos.color =
            Color.green;

        Gizmos.DrawSphere(
            centro +
            DireccionFrontal *
            ajusteProfundidad,
            0.08f
        );

        // Direccion frontal de la silla.
        Gizmos.color =
            Color.blue;

        Gizmos.DrawLine(
            centro,
            centro +
            DireccionFrontal *
            distanciaVisualAproximacion
        );

        Gizmos.DrawSphere(
            centro +
            DireccionFrontal *
            distanciaVisualAproximacion,
            0.07f
        );
    }
}