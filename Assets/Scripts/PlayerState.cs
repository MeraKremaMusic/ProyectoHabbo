using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public enum Estado
    {
        Quieto,
        Caminando
    }

    [Header("Referencias")]
    public PlayerMovement movimiento;

    [Header("Estado actual")]
    public Estado estadoActual = Estado.Quieto;

    private Estado estadoAnterior;

    private void Start()
    {
        estadoAnterior = estadoActual;
    }

    private void Update()
    {
        ActualizarEstado();
    }

    private void ActualizarEstado()
    {
        if (movimiento == null)
            return;

        if (movimiento.EstaMoviendose)
        {
            estadoActual = Estado.Caminando;
        }
        else
        {
            estadoActual = Estado.Quieto;
        }

        if (estadoActual != estadoAnterior)
        {
            Debug.Log(
                "Estado del jugador: " +
                estadoActual
            );

            estadoAnterior = estadoActual;
        }
    }
}