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

    [Header("Referencia visual")]
    public Transform visual;

    [Header("Configuracion")]
    public float umbralMovimiento = 0.001f;
    public float velocidadGiro = 720f;

    [Header("Estado")]
    public Direccion8 direccionActual =
        Direccion8.Sur;

    public bool bloqueadoExternamente;

    private Vector3 posicionAnterior;
    private Quaternion rotacionObjetivo;

    private void Start()
    {
        posicionAnterior =
            transform.position;

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
        if (bloqueadoExternamente)
        {
            posicionAnterior =
                transform.position;

            return;
        }

        DetectarDireccion();
        GirarSuavemente();
    }

    private void DetectarDireccion()
    {
        Vector3 movimiento =
            transform.position -
            posicionAnterior;

        movimiento.y = 0f;

        if (
            movimiento.sqrMagnitude >
            umbralMovimiento *
            umbralMovimiento
        )
        {
            ActualizarDireccion(
                movimiento
            );
        }

        posicionAnterior =
            transform.position;
    }

    private void ActualizarDireccion(
        Vector3 movimiento)
    {
        movimiento.y = 0f;

        if (
            movimiento.sqrMagnitude <
            0.001f
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

    public void Bloquear(
        bool bloquear)
    {
        bloqueadoExternamente =
            bloquear;

        posicionAnterior =
            transform.position;
    }

    public void MirarHacia(
        Vector3 direccion,
        bool instantaneo = true)
    {
        direccion.y = 0f;

        if (
            direccion.sqrMagnitude <
            0.001f
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