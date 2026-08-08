using System.Collections.Generic;
using UnityEngine;

public class GridObstacle : MonoBehaviour
{
    [Header("Referencias")]
    public GridManager grid;
    public GridOccupancy occupancy;
    public FurnitureData datos;

    [Header("Configuracion")]
    public bool registrarAlIniciar = true;

    private readonly List<Vector2Int> casillasRegistradas =
        new List<Vector2Int>();

    private bool registrado;

    private void Awake()
    {
        if (datos == null)
        {
            datos =
                GetComponent<FurnitureData>();
        }
    }

    private void Start()
    {
        if (registrarAlIniciar)
        {
            RegistrarDesdePosicionActual();
        }
    }

    private void OnDestroy()
    {
        LiberarCasillas();
    }

    public bool RegistrarDesdeAncla(
        Vector2Int ancla)
    {
        if (
            grid == null ||
            occupancy == null ||
            datos == null
        )
        {
            return false;
        }

        LiberarCasillas();

        for (int x = 0; x < datos.AnchoActual; x++)
        {
            for (int z = 0; z < datos.LargoActual; z++)
            {
                Vector2Int casilla =
                    ancla +
                    new Vector2Int(x, z);

                if (!EstaDentro(casilla))
                {
                    return false;
                }
            }
        }

        for (int x = 0; x < datos.AnchoActual; x++)
        {
            for (int z = 0; z < datos.LargoActual; z++)
            {
                Vector2Int casilla =
                    ancla +
                    new Vector2Int(x, z);

                occupancy.Ocupar(casilla);
                casillasRegistradas.Add(casilla);
            }
        }

        registrado = true;

        Debug.Log(
            gameObject.name +
            " colocado. Ocupa " +
            casillasRegistradas.Count +
            " casillas."
        );

        return true;
    }

    public void LiberarCasillas()
    {
        if (!registrado || occupancy == null)
            return;

        foreach (
            Vector2Int casilla
            in casillasRegistradas
        )
        {
            occupancy.Liberar(casilla);
        }

        casillasRegistradas.Clear();
        registrado = false;
    }

    private void RegistrarDesdePosicionActual()
    {
        if (datos == null || grid == null)
            return;

        Vector3 posicionAncla =
            transform.position;

        posicionAncla.x -=
            ((datos.AnchoActual - 1) *
            grid.tamanoCasilla) / 2f;

        posicionAncla.z -=
            ((datos.LargoActual - 1) *
            grid.tamanoCasilla) / 2f;

        if (
            grid.ObtenerCasilla(
                posicionAncla,
                out Vector2Int ancla
            )
        )
        {
            RegistrarDesdeAncla(ancla);
        }
    }

    private bool EstaDentro(
        Vector2Int casilla)
    {
        return
            casilla.x >= 0 &&
            casilla.x < grid.ancho &&
            casilla.y >= 0 &&
            casilla.y < grid.largo;
    }
}