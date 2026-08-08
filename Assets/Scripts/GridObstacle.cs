using System.Collections.Generic;
using UnityEngine;

public class GridObstacle : MonoBehaviour
{
    [Header("Referencias")]
    public GridManager grid;
    public GridOccupancy occupancy;

    [Header("Espacio ocupado")]
    [Min(1)]
    public int anchoCasillas = 1;

    [Min(1)]
    public int largoCasillas = 1;

    private readonly List<Vector2Int> casillasRegistradas =
        new List<Vector2Int>();

    private bool registrado;

    private void Start()
    {
        RegistrarObstaculo();
    }

    private void OnDestroy()
    {
        LiberarCasillas();
    }

    private void RegistrarObstaculo()
    {
        if (grid == null || occupancy == null)
            return;

        casillasRegistradas.Clear();

        Vector3 posicionInicial = transform.position;

        posicionInicial.x -=
            ((anchoCasillas - 1) * grid.tamanoCasilla) / 2f;

        posicionInicial.z -=
            ((largoCasillas - 1) * grid.tamanoCasilla) / 2f;

        if (
            !grid.ObtenerCasilla(
                posicionInicial,
                out Vector2Int casillaInicial
            )
        )
        {
            return;
        }

        for (int x = 0; x < anchoCasillas; x++)
        {
            for (int z = 0; z < largoCasillas; z++)
            {
                Vector2Int casilla =
                    casillaInicial +
                    new Vector2Int(x, z);

                if (
                    casilla.x < 0 ||
                    casilla.x >= grid.ancho ||
                    casilla.y < 0 ||
                    casilla.y >= grid.largo
                )
                {
                    continue;
                }

                occupancy.Ocupar(casilla);
                casillasRegistradas.Add(casilla);
            }
        }

        registrado = true;

        Debug.Log(
            gameObject.name +
            " ocupa " +
            casillasRegistradas.Count +
            " casillas."
        );
    }

    private void LiberarCasillas()
    {
        if (!registrado || occupancy == null)
            return;

        foreach (Vector2Int casilla in casillasRegistradas)
        {
            occupancy.Liberar(casilla);
        }

        casillasRegistradas.Clear();
        registrado = false;
    }
}