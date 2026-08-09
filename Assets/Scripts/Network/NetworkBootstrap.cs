using UnityEngine;

public static class NetworkBootstrap
{
    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.BeforeSceneLoad
    )]
    private static void CrearNetworkManager()
    {
        GameObject objeto;

        // Si todavía no existe NetworkManager,
        // lo creamos completo automáticamente.
        if (
            NakamaConnection.Instance == null
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

        // Si ya existe NetworkManager,
        // reutilizamos el mismo objeto
        // y añadimos cualquier servicio que falte.
        objeto =
            NakamaConnection
                .Instance
                .gameObject;

        if (
            objeto.GetComponent<
                NakamaAuthService>() == null
        )
        {
            objeto.AddComponent<
                NakamaAuthService>();
        }

        if (
            objeto.GetComponent<
                NakamaPlayerProfileService>() == null
        )
        {
            objeto.AddComponent<
                NakamaPlayerProfileService>();
        }

        if (
            objeto.GetComponent<
                GameFlowService>() == null
        )
        {
            objeto.AddComponent<
                GameFlowService>();
        }

        if (
            objeto.GetComponent<
                GameFlowNavigator>() == null
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