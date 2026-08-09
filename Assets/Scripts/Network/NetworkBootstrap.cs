using UnityEngine;

public static class NetworkBootstrap
{
    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.BeforeSceneLoad
    )]
    private static void CrearNetworkManager()
    {
        if (
            NakamaConnection.Instance != null
        )
        {
            return;
        }

        GameObject objeto =
            new GameObject(
                "NetworkManager"
            );

        objeto.AddComponent<
            NakamaConnection>();

        objeto.AddComponent<
            NakamaAuthService>();

        Object.DontDestroyOnLoad(
            objeto
        );
    }
}