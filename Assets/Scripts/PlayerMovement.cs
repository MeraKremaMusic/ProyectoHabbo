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
        get { return ruta.Count > 0; }
    }

    private void Update()
    {
        MoverPorRuta();
    }

    public void SeguirRuta(List<Vector3> nuevaRuta)
    {
        ruta.Clear();

        foreach (Vector3 punto in nuevaRuta)
        {
            ruta.Enqueue(punto);
        }
    }

    private void MoverPorRuta()
    {
        if (ruta.Count == 0)
            return;

        Vector3 destino = ruta.Peek();

        transform.position = Vector3.MoveTowards(
            transform.position,
            destino,
            velocidad * Time.deltaTime
        );

        if (
            Vector3.Distance(
                transform.position,
                destino
            ) < 0.01f
        )
        {
            transform.position = destino;
            ruta.Dequeue();
        }
    }
}