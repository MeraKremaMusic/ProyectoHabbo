using UnityEngine;

public class FurniturePlacementValidator : MonoBehaviour
{
    [Header("Referencias")]
    public GridManager grid;
    public GridOccupancy occupancy;
    public FurniturePlacement placement;

    public bool PuedeColocarActual()
    {
        if (
            grid == null ||
            occupancy == null ||
            placement == null ||
            placement.muebleActual == null
        )
        {
            return false;
        }

        if (
            !placement.ObtenerCasillaAncla(
                out Vector2Int ancla
            )
        )
        {
            return false;
        }

        FurnitureData datos =
            placement.muebleActual
                .GetComponent<FurnitureData>();

        if (datos == null)
            return false;

        for (int x = 0; x < datos.AnchoActual; x++)
        {
            for (int z = 0; z < datos.LargoActual; z++)
            {
                Vector2Int casilla =
                    ancla +
                    new Vector2Int(x, z);

                if (!EstaDentroDelGrid(casilla))
                    return false;

                if (occupancy.EstaOcupada(casilla))
                    return false;
            }
        }

        return true;
    }

    private bool EstaDentroDelGrid(
        Vector2Int casilla)
    {
        return
            casilla.x >= 0 &&
            casilla.x < grid.ancho &&
            casilla.y >= 0 &&
            casilla.y < grid.largo;
    }
}