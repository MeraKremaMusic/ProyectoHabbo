using UnityEngine;

public class FurnitureSpawner : MonoBehaviour
{
    [Header("Referencias")]
    public FurnitureCatalog catalogo;
    public FurniturePlacement placement;

    [Header("Organizacion")]
    public Transform contenedorMuebles;

    public void CrearMueble(int indice)
    {
        if (
            catalogo == null ||
            placement == null
        )
        {
            return;
        }

        if (placement.EstaColocando)
            return;

        GameObject prefab =
            catalogo.ObtenerMueble(indice);

        if (prefab == null)
            return;

        GameObject nuevoMueble;

        if (contenedorMuebles != null)
        {
            nuevoMueble =
                Instantiate(
                    prefab,
                    contenedorMuebles
                );
        }
        else
        {
            nuevoMueble =
                Instantiate(prefab);
        }

        placement.muebleActual =
            nuevoMueble;

        placement.RefrescarPosicionDesdeMouse();
    }
}