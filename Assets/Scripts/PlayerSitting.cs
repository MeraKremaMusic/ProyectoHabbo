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

        // Si hacemos clic en el mismo asiento
        // donde ya estamos sentados, no hacemos nada.
        if (
            EstaSentado &&
            asientoActual == asiento
        )
        {
            return true;
        }

        // Si estábamos sentados en otro sitio,
        // primero nos levantamos.
        if (EstaSentado)
        {
            LevantarseInmediatamente();
        }

        // Cancela cualquier asiento anterior
        // al que estuviéramos caminando.
        asientoObjetivo =
            asiento;

        if (
            !asiento.ObtenerCasillaAproximacion(
                grid,
                out Vector2Int destino
            )
        )
        {
            asientoObjetivo = null;

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
            asientoObjetivo = null;

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
            asientoObjetivo = null;

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

        // Si ya estábamos en la casilla
        // de aproximación.
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

        if (
            !asientoObjetivo
                .IntentarOcupar(this)
        )
        {
            EstaYendoASentarse =
                false;

            asientoObjetivo =
                null;

            return;
        }

        asientoActual =
            asientoObjetivo;

        asientoObjetivo =
            null;

        EstaYendoASentarse =
            false;

        transform.position =
            asientoActual
                .ObtenerPosicionSentado(
                    alturaBaseJugador
                );

        if (facing != null)
        {
            facing.MirarHacia(
                asientoActual
                    .DireccionFrontal,
                true
            );

            facing.Bloquear(true);
        }

        EstaSentado =
            true;

        Debug.Log(
            "Jugador sentado correctamente."
        );
    }

    public void PrepararParaCaminar()
    {
        // Si estaba caminando hacia una silla
        // pero el usuario hizo clic en otro sitio.
        if (EstaYendoASentarse)
        {
            EstaYendoASentarse =
                false;

            asientoObjetivo =
                null;
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

        asiento.Liberar(
            this
        );

        EstaSentado =
            false;

        asientoActual =
            null;

        if (facing != null)
        {
            facing.Bloquear(false);
        }

        if (
            grid != null &&
            asiento.ObtenerCasillaAproximacion(
                grid,
                out Vector2Int casilla
            )
        )
        {
            transform.position =
                grid.ObtenerCentroCasilla(
                    casilla,
                    alturaBaseJugador
                );
        }

        Debug.Log(
            "Jugador levantado."
        );
    }
}