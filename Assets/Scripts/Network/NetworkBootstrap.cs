using UnityEngine;

public static class NetworkBootstrap
{
    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.BeforeSceneLoad
    )]
    private static void CrearNetworkManager()
    {
        GameObject objeto;


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


        objeto =
            NakamaConnection
                .Instance
                .gameObject;


        if (
            objeto.GetComponent<
                NakamaAuthService>() ==
            null
        )
        {
            objeto.AddComponent<
                NakamaAuthService>();
        }


        if (
            objeto.GetComponent<
                NakamaPlayerProfileService>() ==
            null
        )
        {
            objeto.AddComponent<
                NakamaPlayerProfileService>();
        }


        if (
            objeto.GetComponent<
                PlayerWalletService>() ==
            null
        )
        {
            objeto.AddComponent<
                PlayerWalletService>();
        }


        if (
            objeto.GetComponent<
                FurnitureShopCatalogService>() ==
            null
        )
        {
            objeto.AddComponent<
                FurnitureShopCatalogService>();
        }


        if (
            objeto.GetComponent<
                FurniturePurchaseService>() ==
            null
        )
        {
            objeto.AddComponent<
                FurniturePurchaseService>();
        }


        if (
            objeto.GetComponent<
                PlayerInventoryService>() ==
            null
        )
        {
            objeto.AddComponent<
                PlayerInventoryService>();
        }


        if (
            objeto.GetComponent<
                GameFlowService>() ==
            null
        )
        {
            objeto.AddComponent<
                GameFlowService>();
        }


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