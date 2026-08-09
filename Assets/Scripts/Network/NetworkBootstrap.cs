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
        // SI NO EXISTE NETWORKMANAGER, LO CREAMOS
        // =====================================================

        if (NakamaConnection.Instance == null)
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
        // SI YA EXISTE, REUTILIZAMOS EL MISMO
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
                NakamaAuthService>() == null
        )
        {
            objeto.AddComponent<
                NakamaAuthService>();
        }


        // =====================================================
        // PERFIL DEL JUGADOR
        // =====================================================

        if (
            objeto.GetComponent<
                NakamaPlayerProfileService>() == null
        )
        {
            objeto.AddComponent<
                NakamaPlayerProfileService>();
        }


        // =====================================================
        // MONEDAS / WALLET
        // =====================================================

        if (
            objeto.GetComponent<
                PlayerWalletService>() == null
        )
        {
            objeto.AddComponent<
                PlayerWalletService>();
        }


        // =====================================================
        // CATALOGO DE LA TIENDA
        // =====================================================

        if (
            objeto.GetComponent<
                FurnitureShopCatalogService>() == null
        )
        {
            objeto.AddComponent<
                FurnitureShopCatalogService>();
        }


        // =====================================================
        // FLUJO DEL JUEGO
        // =====================================================

        if (
            objeto.GetComponent<
                GameFlowService>() == null
        )
        {
            objeto.AddComponent<
                GameFlowService>();
        }


        // =====================================================
        // CAMBIO ENTRE ESCENAS
        // =====================================================

        if (
            objeto.GetComponent<
                GameFlowNavigator>() == null
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