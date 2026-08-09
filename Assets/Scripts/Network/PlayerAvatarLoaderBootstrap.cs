using UnityEngine;
using UnityEngine.SceneManagement;

public static class
    PlayerAvatarLoaderBootstrap
{
    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.BeforeSceneLoad
    )]
    private static void Inicializar()
    {
        SceneManager.sceneLoaded -=
            AlCargarEscena;

        SceneManager.sceneLoaded +=
            AlCargarEscena;
    }

    private static void AlCargarEscena(
        Scene escena,
        LoadSceneMode modo)
    {
        PlayerMovement jugador =
            Object.FindAnyObjectByType<
                PlayerMovement>();

        if (jugador == null)
            return;

        PlayerAvatarLoader existente =
            jugador.GetComponent<
                PlayerAvatarLoader>();

        if (existente != null)
            return;

        jugador
            .gameObject
            .AddComponent<
                PlayerAvatarLoader>();

        Debug.Log(
            "PlayerAvatarLoader agregado " +
            "automaticamente."
        );
    }
}