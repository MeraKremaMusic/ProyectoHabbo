using UnityEngine;

public class FurnitureSeat : MonoBehaviour
{
    [Header("Configuracion del asiento")]
    public bool invertirFrente = false;

    [Range(-0.5f, 0.5f)]
    public float ajusteProfundidad = 0f;

    [Range(-0.5f, 0.5f)]
    public float ajusteAlturaJugador = 0f;

    [Header("Visualizacion")]
    public float distanciaVisualAproximacion = 1f;

    public PlayerSitting Ocupante
    {
        get;
        private set;
    }

    public bool EstaOcupado
    {
        get
        {
            return Ocupante != null;
        }
    }

    public Vector3 DireccionFrontal
    {
        get
        {
            Vector3 direccion =
                invertirFrente
                    ? -transform.forward
                    : transform.forward;

            direccion.y = 0f;

            if (
                direccion.sqrMagnitude <
                0.001f
            )
            {
                direccion =
                    Vector3.forward;
            }

            return direccion.normalized;
        }
    }

    public Vector3 ObtenerPosicionSentado(
        float alturaBaseJugador)
    {
        Vector3 posicion =
            transform.position;

        posicion +=
            DireccionFrontal *
            ajusteProfundidad;

        posicion.y =
            alturaBaseJugador +
            ajusteAlturaJugador;

        return posicion;
    }

    public bool ObtenerCasillaAproximacion(
        GridManager grid,
        out Vector2Int casilla)
    {
        casilla =
            Vector2Int.zero;

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

        if (
            casilla.x < 0 ||
            casilla.x >= grid.ancho ||
            casilla.y < 0 ||
            casilla.y >= grid.largo
        )
        {
            return false;
        }

        return true;
    }

    public bool IntentarOcupar(
        PlayerSitting jugador)
    {
        if (
            jugador == null
        )
        {
            return false;
        }

        if (
            Ocupante != null &&
            Ocupante != jugador
        )
        {
            return false;
        }

        Ocupante =
            jugador;

        return true;
    }

    public void Liberar(
        PlayerSitting jugador)
    {
        if (Ocupante == jugador)
        {
            Ocupante =
                null;
        }
    }

    public void SincronizarOcupante()
    {
        if (Ocupante == null)
            return;

        Ocupante.SincronizarConAsiento(
            this
        );
    }

    public PlayerSitting LevantarOcupanteEnPosicionDelMueble()
    {
        if (Ocupante == null)
            return null;

        PlayerSitting jugador =
            Ocupante;

        Vector3 posicionDePie =
            transform.position;

        jugador.ForzarLevantarseEnPosicion(
            posicionDePie
        );

        return jugador;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 centro =
            transform.position +
            Vector3.up * 0.15f;

        Gizmos.color =
            Color.green;

        Gizmos.DrawSphere(
            centro +
            DireccionFrontal *
            ajusteProfundidad,
            0.08f
        );

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