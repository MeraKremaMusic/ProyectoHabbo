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

            Object.DontDestroyOnLoad(
                objeto
            );

            return;
        }

        // Si el NetworkManager ya existe,
        // completamos automáticamente
        // cualquier servicio que falte.
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

        Object.DontDestroyOnLoad(
            objeto
        );
    }
}