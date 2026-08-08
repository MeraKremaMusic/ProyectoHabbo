using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 4f;

    private readonly Queue<Vector3> ruta =
        new Queue<Vector3>();

    public bool EstaMoviendose
    {
        get
        {
            return ruta.Count > 0;
        }
    }

    private void Update()
    {
        MoverPorRuta();
    }

    public void SeguirRuta(
        List<Vector3> nuevaRuta)
    {
        ruta.Clear();

        if (nuevaRuta == null)
            return;

        foreach (
            Vector3 punto
            in nuevaRuta
        )
        {
            ruta.Enqueue(punto);
        }
    }

    /// <summary>
    /// Devuelve la direccion REAL hacia
    /// el siguiente punto de la ruta.
    ///
    /// PlayerFacing usa esto para saber
    /// hacia donde debe mirar.
    /// </summary>
    public bool ObtenerDireccionMovimiento(
        out Vector3 direccion)
    {
        direccion =
            Vector3.zero;

        if (ruta.Count == 0)
            return false;

        Vector3 destino =
            ruta.Peek();

        direccion =
            destino -
            transform.position;

        direccion.y = 0f;

        if (
            direccion.sqrMagnitude <
            0.000001f
        )
        {
            return false;
        }

        direccion.Normalize();

        return true;
    }

    public void Detener()
    {
        ruta.Clear();
    }

    private void MoverPorRuta()
    {
        if (ruta.Count == 0)
            return;

        Vector3 destino =
            ruta.Peek();

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                destino,
                velocidad *
                Time.deltaTime
            );

        if (
            Vector3.Distance(
                transform.position,
                destino
            ) < 0.01f
        )
        {
            transform.position =
                destino;

            ruta.Dequeue();
        }
    }
}