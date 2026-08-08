using System.Collections.Generic;
using UnityEngine;

public class GridOccupancy : MonoBehaviour
{
    private readonly Dictionary<Vector2Int, int> casillasOcupadas =
        new Dictionary<Vector2Int, int>();

    public bool EstaOcupada(Vector2Int casilla)
    {
        return casillasOcupadas.ContainsKey(casilla);
    }

    public void Ocupar(Vector2Int casilla)
    {
        if (casillasOcupadas.ContainsKey(casilla))
        {
            casillasOcupadas[casilla]++;
        }
        else
        {
            casillasOcupadas.Add(casilla, 1);
        }
    }

    public void Liberar(Vector2Int casilla)
    {
        if (!casillasOcupadas.ContainsKey(casilla))
            return;

        casillasOcupadas[casilla]--;

        if (casillasOcupadas[casilla] <= 0)
        {
            casillasOcupadas.Remove(casilla);
        }
    }

    public void Limpiar()
    {
        casillasOcupadas.Clear();
    }
}