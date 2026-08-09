using UnityEngine;
using UnityEngine.SceneManagement;

public static class PlayerNameTagBootstrap
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
            Object.FindAnyObjectByType<PlayerMovement>();

        if (jugador == null)
            return;

        PlayerNameTag nombre =
            jugador.GetComponent<PlayerNameTag>();

        if (nombre != null)
            return;

        jugador.gameObject
            .AddComponent<PlayerNameTag>();

        Debug.Log(
            "Nombre de usuario agregado " +
            "automaticamente al jugador."
        );
    }
}