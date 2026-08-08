using System.Collections.Generic;
using UnityEngine;

public class PlayerSitting : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerMovement movimiento;
    public PlayerFacing facing;
    public GridManager grid;
    public Pathfinding pathfinding;

    public bool EstaSentado
    {
        get;
        private set;
    }

    public bool EstaYendoASentarse
    {
        get;
        private set;
    }

    private FurnitureSeat asientoObjetivo;
    private FurnitureSeat asientoActual;

    private float alturaBaseJugador;

    private void Awake()
    {
        if (movimiento == null)
        {
            movimiento =
                GetComponent<PlayerMovement>();
        }

        if (facing == null)
        {
            facing =
                GetComponent<PlayerFacing>();
        }

        if (grid == null)
        {
            grid =
                FindFirstObjectByType<GridManager>();
        }

        if (pathfinding == null)
        {
            pathfinding =
                FindFirstObjectByType<Pathfinding>();
        }
    }

    private void Start()
    {
        alturaBaseJugador =
            transform.position.y;
    }

    private void Update()
    {
        if (
            !EstaYendoASentarse ||
            movimiento == null
        )
        {
            return;
        }

        if (!movimiento.EstaMoviendose)
        {
            SentarseAhora();
        }
    }

    public bool IrASentarse(
        FurnitureSeat asiento)
    {
        if (
            asiento == null ||
            movimiento == null ||
            grid == null ||
            pathfinding == null
        )
        {
            return false;
        }

        if (
            asiento.EstaOcupado &&
            asiento.Ocupante != this
        )
        {
            Debug.Log(
                "Ese asiento ya esta ocupado."
            );

            return false;
        }

        if (
            EstaSentado &&
            asientoActual == asiento
        )
        {
            return true;
        }

        if (EstaSentado)
        {
            LevantarseInmediatamente();
        }

        asientoObjetivo =
            asiento;

        if (
            !asiento.ObtenerCasillaAproximacion(
                grid,
                out Vector2Int destino
            )
        )
        {
            asientoObjetivo =
                null;

            Debug.Log(
                "No hay una casilla valida " +
                "delante de la silla."
            );

            return false;
        }

        if (
            !grid.ObtenerCasilla(
                transform.position,
                out Vector2Int inicio
            )
        )
        {
            asientoObjetivo =
                null;

            return false;
        }

        List<Vector2Int> rutaCasillas =
            pathfinding.EncontrarRuta(
                inicio,
                destino
            );

        if (
            rutaCasillas.Count == 0 &&
            inicio != destino
        )
        {
            asientoObjetivo =
                null;

            Debug.Log(
                "El jugador no puede llegar " +
                "hasta esa silla."
            );

            return false;
        }

        List<Vector3> rutaMundo =
            new List<Vector3>();

        foreach (
            Vector2Int casilla
            in rutaCasillas
        )
        {
            Vector3 punto =
                grid.ObtenerCentroCasilla(
                    casilla,
                    alturaBaseJugador
                );

            rutaMundo.Add(
                punto
            );
        }

        EstaSentado =
            false;

        EstaYendoASentarse =
            true;

        if (facing != null)
        {
            facing.Bloquear(false);
        }

        movimiento.SeguirRuta(
            rutaMundo
        );

        Debug.Log(
            "Caminando hacia el asiento."
        );

        if (rutaMundo.Count == 0)
        {
            SentarseAhora();
        }

        return true;
    }

    private void SentarseAhora()
    {
        if (asientoObjetivo == null)
        {
            EstaYendoASentarse =
                false;

            return;
        }

        FurnitureSeat asiento =
            asientoObjetivo;

        asientoObjetivo =
            null;

        SentarseDirectamente(
            asiento
        );
    }

    public bool SentarseDirectamente(
        FurnitureSeat asiento)
    {
        if (asiento == null)
            return false;

        if (
            asiento.EstaOcupado &&
            asiento.Ocupante != this
        )
        {
            return false;
        }

        if (
            asientoActual != null &&
            asientoActual != asiento
        )
        {
            asientoActual.Liberar(
                this
            );
        }

        if (
            movimiento != null
        )
        {
            movimiento.Detener();
        }

        asientoObjetivo =
            null;

        EstaYendoASentarse =
            false;

        if (
            !asiento.IntentarOcupar(
                this
            )
        )
        {
            return false;
        }

        asientoActual =
            asiento;

        transform.position =
            asiento.ObtenerPosicionSentado(
                alturaBaseJugador
            );

        if (facing != null)
        {
            facing.MirarHacia(
                asiento.DireccionFrontal,
                true
            );

            facing.Bloquear(true);
        }

        EstaSentado =
            true;

        Debug.Log(
            "Jugador sentado correctamente."
        );

        return true;
    }

    public void SincronizarConAsiento(
        FurnitureSeat asiento)
    {
        if (
            !EstaSentado ||
            asiento == null ||
            asientoActual != asiento
        )
        {
            return;
        }

        transform.position =
            asiento.ObtenerPosicionSentado(
                alturaBaseJugador
            );

        if (facing != null)
        {
            facing.MirarHacia(
                asiento.DireccionFrontal,
                true
            );

            facing.Bloquear(true);
        }
    }

    public void ForzarLevantarseEnPosicion(
        Vector3 posicionMundo)
    {
        if (movimiento != null)
        {
            movimiento.Detener();
        }

        asientoObjetivo =
            null;

        EstaYendoASentarse =
            false;

        if (asientoActual != null)
        {
            FurnitureSeat asientoAnterior =
                asientoActual;

            asientoActual =
                null;

            asientoAnterior.Liberar(
                this
            );
        }

        EstaSentado =
            false;

        if (facing != null)
        {
            facing.Bloquear(false);
        }

        posicionMundo.y =
            alturaBaseJugador;

        transform.position =
            posicionMundo;

        Debug.Log(
            "Jugador puesto de pie."
        );
    }

    public void PrepararParaCaminar()
    {
        if (EstaYendoASentarse)
        {
            EstaYendoASentarse =
                false;

            asientoObjetivo =
                null;

            if (movimiento != null)
            {
                movimiento.Detener();
            }
        }

        if (!EstaSentado)
            return;

        LevantarseInmediatamente();
    }

    private void LevantarseInmediatamente()
    {
        if (
            !EstaSentado ||
            asientoActual == null
        )
        {
            EstaSentado =
                false;

            return;
        }

        FurnitureSeat asiento =
            asientoActual;

        Vector3 posicionDestino =
            transform.position;

        if (
            grid != null &&
            asiento.ObtenerCasillaAproximacion(
                grid,
                out Vector2Int casilla
            )
        )
        {
            posicionDestino =
                grid.ObtenerCentroCasilla(
                    casilla,
                    alturaBaseJugador
                );
        }

        ForzarLevantarseEnPosicion(
            posicionDestino
        );

        Debug.Log(
            "Jugador levantado."
        );
    }
}