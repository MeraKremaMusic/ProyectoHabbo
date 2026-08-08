using UnityEngine;

public class PlayerFacing : MonoBehaviour
{
    public enum Direccion8
    {
        Norte,
        Noreste,
        Este,
        Sureste,
        Sur,
        Suroeste,
        Oeste,
        Noroeste
    }

    [Header("Referencias")]
    public Transform visual;
    public PlayerMovement movimiento;

    [Header("Configuracion")]
    public float velocidadGiro = 720f;

    [Header("Estado")]
    public Direccion8 direccionActual =
        Direccion8.Sur;

    public bool bloqueadoExternamente;

    private Quaternion rotacionObjetivo;

    private void Awake()
    {
        // No queremos configurarlo manualmente.
        if (movimiento == null)
        {
            movimiento =
                GetComponent<PlayerMovement>();
        }
    }

    private void Start()
    {
        if (visual == null)
        {
            visual =
                transform;
        }

        rotacionObjetivo =
            visual.rotation;
    }

    private void LateUpdate()
    {
        // Cuando está sentado,
        // PlayerSitting controla la dirección.
        if (bloqueadoExternamente)
            return;

        if (movimiento == null)
            return;

        // SOLO usamos la ruta real.
        // Teletransportes o ajustes de posición
        // ya no pueden cambiar la dirección.
        if (
            movimiento
                .ObtenerDireccionMovimiento(
                    out Vector3 direccion
                )
        )
        {
            ActualizarDireccion(
                direccion
            );
        }

        GirarSuavemente();
    }

    private void ActualizarDireccion(
        Vector3 movimiento)
    {
        movimiento.y = 0f;

        if (
            movimiento.sqrMagnitude <
            0.000001f
        )
        {
            return;
        }

        movimiento.Normalize();

        float angulo =
            Mathf.Atan2(
                movimiento.x,
                movimiento.z
            ) * Mathf.Rad2Deg;

        if (angulo < 0f)
        {
            angulo += 360f;
        }

        int indice =
            Mathf.RoundToInt(
                angulo / 45f
            ) % 8;

        direccionActual =
            (Direccion8)indice;

        float rotacionY =
            indice * 45f;

        rotacionObjetivo =
            Quaternion.Euler(
                0f,
                rotacionY,
                0f
            );
    }

    private void GirarSuavemente()
    {
        if (visual == null)
            return;

        visual.rotation =
            Quaternion.RotateTowards(
                visual.rotation,
                rotacionObjetivo,
                velocidadGiro *
                Time.deltaTime
            );
    }

    /// <summary>
    /// Bloquea el giro automatico.
    /// Se usa mientras el personaje
    /// esta sentado.
    /// </summary>
    public void Bloquear(
        bool bloquear)
    {
        bloqueadoExternamente =
            bloquear;
    }

    /// <summary>
    /// Fuerza al personaje a mirar
    /// hacia una direccion concreta.
    /// FurnitureSeat usa esto
    /// cuando se sienta.
    /// </summary>
    public void MirarHacia(
        Vector3 direccion,
        bool instantaneo = true)
    {
        direccion.y = 0f;

        if (
            direccion.sqrMagnitude <
            0.000001f
        )
        {
            return;
        }

        ActualizarDireccion(
            direccion
        );

        if (
            instantaneo &&
            visual != null
        )
        {
            visual.rotation =
                rotacionObjetivo;
        }
    }
}