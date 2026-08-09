using UnityEngine;

public static class FurniturePrefabResolver
{
    /// <summary>
    /// Busca dentro del FurnitureCatalog el prefab
    /// correspondiente a un productId de Nakama.
    /// </summary>
    public static GameObject ObtenerPrefab(
        string productId)
    {
        if (
            string.IsNullOrWhiteSpace(
                productId
            )
        )
        {
            Debug.LogWarning(
                "FurniturePrefabResolver: productId vacio."
            );

            return null;
        }


        FurnitureCatalog catalogo =
            Object.FindAnyObjectByType<
                FurnitureCatalog>();


        if (catalogo == null)
        {
            Debug.LogError(
                "FurniturePrefabResolver: no se encontro FurnitureCatalog en la escena."
            );

            return null;
        }


        if (catalogo.muebles == null)
        {
            Debug.LogError(
                "FurniturePrefabResolver: FurnitureCatalog no tiene muebles."
            );

            return null;
        }


        foreach (
            GameObject prefab
            in catalogo.muebles
        )
        {
            if (prefab == null)
                continue;


            FurnitureProductLink link =
                prefab.GetComponent<
                    FurnitureProductLink>();


            if (link == null)
                continue;


            if (
                link.CoincideCon(
                    productId
                )
            )
            {
                return prefab;
            }
        }


        Debug.LogWarning(
            "FurniturePrefabResolver: no existe prefab para productId: " +
            productId
        );


        return null;
    }
}