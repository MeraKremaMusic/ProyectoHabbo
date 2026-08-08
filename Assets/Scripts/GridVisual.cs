using UnityEngine;

public class GridVisual : MonoBehaviour
{
    private GridManager grid;

    private void OnDrawGizmos()
    {
        if (grid == null)
            grid = GetComponent<GridManager>();

        if (grid == null)
            return;

        Gizmos.color = Color.yellow;

        float altura = 0.11f;

        for (int x = 0; x <= grid.ancho; x++)
        {
            Vector3 inicio = new Vector3(
                grid.InicioX + x * grid.tamanoCasilla,
                altura,
                grid.InicioZ
            );

            Vector3 fin = new Vector3(
                grid.InicioX + x * grid.tamanoCasilla,
                altura,
                grid.InicioZ + grid.largo * grid.tamanoCasilla
            );

            Gizmos.DrawLine(inicio, fin);
        }

        for (int z = 0; z <= grid.largo; z++)
        {
            Vector3 inicio = new Vector3(
                grid.InicioX,
                altura,
                grid.InicioZ + z * grid.tamanoCasilla
            );

            Vector3 fin = new Vector3(
                grid.InicioX + grid.ancho * grid.tamanoCasilla,
                altura,
                grid.InicioZ + z * grid.tamanoCasilla
            );

            Gizmos.DrawLine(inicio, fin);
        }
    }
}