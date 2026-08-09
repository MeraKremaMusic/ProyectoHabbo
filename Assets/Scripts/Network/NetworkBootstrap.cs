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
        // CREAR NETWORK MANAGER
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
        // REUTILIZAR NETWORK MANAGER
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
        // FLUJO
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
        // NAVEGACION
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


        Object.DontDestroyOnLoad(
            objeto
        );


        Debug.Log(
            "NetworkManager verificado y completado."
        );
    }
}