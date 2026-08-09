using UnityEngine;

public static class NetworkBootstrap
{
    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.BeforeSceneLoad
    )]
    private static void CrearNetworkManager()
    {
        GameObject objeto;


        // =====================================================
        // CREAR NETWORK MANAGER SI NO EXISTE
        // =====================================================

        if (
            NakamaConnection.Instance ==
            null
        )
        {
            objeto =
                new GameObject(
                    "NetworkManager"
                );


            objeto.AddComponent<
                NakamaConnection>();

            objeto.AddComponent<
                NakamaAuthService>();

            objeto.AddComponent<
                NakamaPlayerProfileService>();

            objeto.AddComponent<
                PlayerWalletService>();

            objeto.AddComponent<
                FurnitureShopCatalogService>();

            objeto.AddComponent<
                FurniturePurchaseService>();

            objeto.AddComponent<
                FurniturePlacementSyncService>();

            objeto.AddComponent<
                FurniturePickupService>();

            objeto.AddComponent<
                PlayerInventoryService>();

            objeto.AddComponent<
                GameFlowService>();

            objeto.AddComponent<
                GameFlowNavigator>();


            Object.DontDestroyOnLoad(
                objeto
            );


            Debug.Log(
                "NetworkManager creado automaticamente."
            );


            return;
        }


        // =====================================================
        // REUTILIZAR NETWORK MANAGER EXISTENTE
        // =====================================================

        objeto =
            NakamaConnection
                .Instance
                .gameObject;


        // =====================================================
        // AUTENTICACION
        // =====================================================

        if (
            objeto.GetComponent<
                NakamaAuthService>() ==
            null
        )
        {
            objeto.AddComponent<
                NakamaAuthService>();
        }


        // =====================================================
        // PERFIL
        // =====================================================

        if (
            objeto.GetComponent<
                NakamaPlayerProfileService>() ==
            null
        )
        {
            objeto.AddComponent<
                NakamaPlayerProfileService>();
        }


        // =====================================================
        // MONEDAS
        // =====================================================

        if (
            objeto.GetComponent<
                PlayerWalletService>() ==
            null
        )
        {
            objeto.AddComponent<
                PlayerWalletService>();
        }


        // =====================================================
        // CATALOGO DE TIENDA
        // =====================================================

        if (
            objeto.GetComponent<
                FurnitureShopCatalogService>() ==
            null
        )
        {
            objeto.AddComponent<
                FurnitureShopCatalogService>();
        }


        // =====================================================
        // COMPRAS
        // =====================================================

        if (
            objeto.GetComponent<
                FurniturePurchaseService>() ==
            null
        )
        {
            objeto.AddComponent<
                FurniturePurchaseService>();
        }


        // =====================================================
        // GUARDAR COLOCACION DE MUEBLES
        // =====================================================

        if (
            objeto.GetComponent<
                FurniturePlacementSyncService>() ==
            null
        )
        {
            objeto.AddComponent<
                FurniturePlacementSyncService>();
        }


        // =====================================================
        // RECOGER MUEBLES
        // =====================================================

        if (
            objeto.GetComponent<
                FurniturePickupService>() ==
            null
        )
        {
            objeto.AddComponent<
                FurniturePickupService>();
        }


        // =====================================================
        // INVENTARIO
        // =====================================================

        if (
            objeto.GetComponent<
                PlayerInventoryService>() ==
            null
        )
        {
            objeto.AddComponent<
                PlayerInventoryService>();
        }


        // =====================================================
        // FLUJO DEL JUEGO
        // =====================================================

        if (
            objeto.GetComponent<
                GameFlowService>() ==
            null
        )
        {
            objeto.AddComponent<
                GameFlowService>();
        }


        // =====================================================
        // NAVEGACION ENTRE ESCENAS
        // =====================================================

        if (
            objeto.GetComponent<
                GameFlowNavigator>() ==
            null
        )
        {
            objeto.AddComponent<
                GameFlowNavigator>();
        }


        // =====================================================
        // CONSERVAR ENTRE ESCENAS
        // =====================================================

        Object.DontDestroyOnLoad(
            objeto
        );


        Debug.Log(
            "NetworkManager verificado y completado."
        );
    }
}