using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    [Header("Referencias")]
    public GridManager grid;
    public GridOccupancy occupancy;

    // 4 direcciones rectas
    private readonly Vector2Int[] direccionesRectas =
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };

    // 4 direcciones diagonales
    private readonly Vector2Int[] direccionesDiagonales =
    {
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1)
    };

    public List<Vector2Int> EncontrarRuta(
        Vector2Int inicio,
        Vector2Int destino)
    {
        List<Vector2Int> rutaVacia =
            new List<Vector2Int>();

        if (grid == null || occupancy == null)
            return rutaVacia;

        if (!EstaDentro(inicio) || !EstaDentro(destino))
            return rutaVacia;

        if (occupancy.EstaOcupada(destino))
        {
            Debug.Log("La casilla destino esta ocupada.");
            return rutaVacia;
        }

        if (inicio == destino)
            return rutaVacia;

        List<Vector2Int> abiertos =
            new List<Vector2Int>();

        HashSet<Vector2Int> cerrados =
            new HashSet<Vector2Int>();

        Dictionary<Vector2Int, Vector2Int> vinoDesde =
            new Dictionary<Vector2Int, Vector2Int>();

        Dictionary<Vector2Int, int> costoG =
            new Dictionary<Vector2Int, int>();

        abiertos.Add(inicio);
        costoG[inicio] = 0;

        while (abiertos.Count > 0)
        {
            Vector2Int actual =
                ObtenerMejorNodo(
                    abiertos,
                    costoG,
                    destino
                );

            if (actual == destino)
            {
                return ReconstruirRuta(
                    vinoDesde,
                    actual,
                    inicio
                );
            }

            abiertos.Remove(actual);
            cerrados.Add(actual);

            RevisarVecinosRectos(
                actual,
                destino,
                abiertos,
                cerrados,
                vinoDesde,
                costoG
            );

            RevisarVecinosDiagonales(
                actual,
                destino,
                abiertos,
                cerrados,
                vinoDesde,
                costoG
            );
        }

        Debug.Log("No existe una ruta disponible.");

        return rutaVacia;
    }

    private void RevisarVecinosRectos(
        Vector2Int actual,
        Vector2Int destino,
        List<Vector2Int> abiertos,
        HashSet<Vector2Int> cerrados,
        Dictionary<Vector2Int, Vector2Int> vinoDesde,
        Dictionary<Vector2Int, int> costoG)
    {
        foreach (Vector2Int direccion in direccionesRectas)
        {
            Vector2Int vecino = actual + direccion;

            ProcesarVecino(
                actual,
                vecino,
                10,
                abiertos,
                cerrados,
                vinoDesde,
                costoG
            );
        }
    }

    private void RevisarVecinosDiagonales(
        Vector2Int actual,
        Vector2Int destino,
        List<Vector2Int> abiertos,
        HashSet<Vector2Int> cerrados,
        Dictionary<Vector2Int, Vector2Int> vinoDesde,
        Dictionary<Vector2Int, int> costoG)
    {
        foreach (Vector2Int direccion in direccionesDiagonales)
        {
            Vector2Int vecino = actual + direccion;

            if (!PuedeMoverDiagonal(actual, direccion))
                continue;

            ProcesarVecino(
                actual,
                vecino,
                14,
                abiertos,
                cerrados,
                vinoDesde,
                costoG
            );
        }
    }

    private void ProcesarVecino(
        Vector2Int actual,
        Vector2Int vecino,
        int costoMovimiento,
        List<Vector2Int> abiertos,
        HashSet<Vector2Int> cerrados,
        Dictionary<Vector2Int, Vector2Int> vinoDesde,
        Dictionary<Vector2Int, int> costoG)
    {
        if (!EstaDentro(vecino))
            return;

        if (occupancy.EstaOcupada(vecino))
            return;

        if (cerrados.Contains(vecino))
            return;

        int nuevoCosto =
            costoG[actual] + costoMovimiento;

        if (
            !costoG.ContainsKey(vecino) ||
            nuevoCosto < costoG[vecino]
        )
        {
            costoG[vecino] = nuevoCosto;
            vinoDesde[vecino] = actual;

            if (!abiertos.Contains(vecino))
                abiertos.Add(vecino);
        }
    }

    private bool PuedeMoverDiagonal(
        Vector2Int actual,
        Vector2Int direccion)
    {
        Vector2Int horizontal =
            actual + new Vector2Int(
                direccion.x,
                0
            );

        Vector2Int vertical =
            actual + new Vector2Int(
                0,
                direccion.y
            );

        if (!EstaDentro(horizontal))
            return false;

        if (!EstaDentro(vertical))
            return false;

        // Evita atravesar la esquina de un mueble.
        if (occupancy.EstaOcupada(horizontal))
            return false;

        if (occupancy.EstaOcupada(vertical))
            return false;

        return true;
    }

    private Vector2Int ObtenerMejorNodo(
        List<Vector2Int> abiertos,
        Dictionary<Vector2Int, int> costoG,
        Vector2Int destino)
    {
        Vector2Int mejor = abiertos[0];

        int mejorCosto =
            costoG[mejor] +
            Heuristica(mejor, destino);

        for (int i = 1; i < abiertos.Count; i++)
        {
            Vector2Int candidato = abiertos[i];

            int costo =
                costoG[candidato] +
                Heuristica(candidato, destino);

            if (costo < mejorCosto)
            {
                mejor = candidato;
                mejorCosto = costo;
            }
        }

        return mejor;
    }

    private int Heuristica(
        Vector2Int desde,
        Vector2Int hasta)
    {
        int dx =
            Mathf.Abs(desde.x - hasta.x);

        int dy =
            Mathf.Abs(desde.y - hasta.y);

        int diagonal =
            Mathf.Min(dx, dy);

        int recto =
            Mathf.Max(dx, dy) - diagonal;

        return (diagonal * 14) + (recto * 10);
    }

    private List<Vector2Int> ReconstruirRuta(
        Dictionary<Vector2Int, Vector2Int> vinoDesde,
        Vector2Int actual,
        Vector2Int inicio)
    {
        List<Vector2Int> ruta =
            new List<Vector2Int>();

        ruta.Add(actual);

        while (vinoDesde.ContainsKey(actual))
        {
            actual = vinoDesde[actual];
            ruta.Add(actual);
        }

        ruta.Reverse();

        if (
            ruta.Count > 0 &&
            ruta[0] == inicio
        )
        {
            ruta.RemoveAt(0);
        }

        return ruta;
    }

    private bool EstaDentro(Vector2Int casilla)
    {
        return
            casilla.x >= 0 &&
            casilla.x < grid.ancho &&
            casilla.y >= 0 &&
            casilla.y < grid.largo;
    }
}