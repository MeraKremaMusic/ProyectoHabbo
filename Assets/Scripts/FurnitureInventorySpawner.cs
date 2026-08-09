using UnityEngine;

public class FurnitureInventorySpawner :
    MonoBehaviour
{
    private FurniturePlacement placement;

    private Transform contenedorMuebles;


    private void Awake()
    {
        BuscarReferencias();
    }


    private void BuscarReferencias()
    {
        FurnitureSpawner spawnerBase =
            Object.FindAnyObjectByType<
                FurnitureSpawner>();


        if (spawnerBase == null)
        {
            Debug.LogError(
                "FurnitureInventorySpawner: no se encontro FurnitureSpawner."
            );

            return;
        }


        placement =
            spawnerBase.placement;


        contenedorMuebles =
            spawnerBase
                .contenedorMuebles;
    }


    public bool CrearDesdeInventario(
        FurnitureInventoryItemData item)
    {
        if (item == null)
        {
            return false;
        }


        if (item.placed)
        {
            Debug.LogWarning(
                "Este mueble ya esta colocado: " +
                item.item_id
            );

            return false;
        }


        if (
            string.IsNullOrWhiteSpace(
                item.item_id
            )
            ||
            string.IsNullOrWhiteSpace(
                item.product_id
            )
        )
        {
            Debug.LogError(
                "El item del inventario no tiene identidad valida."
            );

            return false;
        }


        if (placement == null)
        {
            BuscarReferencias();

            if (placement == null)
                return false;
        }


        if (placement.EstaColocando)
        {
            Debug.LogWarning(
                "Ya estas colocando otro mueble."
            );

            return false;
        }


        if (
            YaExisteEnEscena(
                item.item_id
            )
        )
        {
            Debug.LogWarning(
                "El item ya existe en la habitacion: " +
                item.item_id
            );

            return false;
        }


        GameObject prefab =
            FurniturePrefabResolver
                .ObtenerPrefab(
                    item.product_id
                );


        if (prefab == null)
        {
            Debug.LogError(
                "No existe prefab para el producto: " +
                item.product_id
            );

            return false;
        }


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
                Instantiate(
                    prefab
                );
        }


        FurnitureInventoryInstance
            identidad =
                nuevoMueble
                    .GetComponent<
                        FurnitureInventoryInstance>();


        if (identidad == null)
        {
            identidad =
                nuevoMueble
                    .AddComponent<
                        FurnitureInventoryInstance>();
        }


        identidad.Configurar(
            item.item_id,
            item.product_id
        );


        placement.muebleActual =
            nuevoMueble;


        placement
            .RefrescarPosicionDesdeMouse();


        Debug.Log(
            "MUEBLE SACADO DEL INVENTARIO -> " +
            item.item_id +
            " | " +
            item.product_id
        );


        return true;
    }


    private bool YaExisteEnEscena(
        string itemId)
    {
        FurnitureInventoryInstance[]
            muebles =
                Object.FindObjectsByType<
                    FurnitureInventoryInstance>(
                    FindObjectsSortMode.None
                );


        foreach (
            FurnitureInventoryInstance mueble
            in muebles
        )
        {
            if (
                mueble != null &&
                mueble.ItemId ==
                itemId
            )
            {
                return true;
            }
        }


        return false;
    }
}