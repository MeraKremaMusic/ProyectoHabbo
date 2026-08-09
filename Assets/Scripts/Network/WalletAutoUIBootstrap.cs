using UnityEngine;
using UnityEngine.SceneManagement;

public static class WalletAutoUIBootstrap
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
        if (
            escena.name !=
            "HabitacionPrincipal"
        )
        {
            return;
        }


        WalletAutoUI existente =
            Object.FindAnyObjectByType<
                WalletAutoUI>();

        if (existente != null)
            return;


        GameObject objeto =
            new GameObject(
                "WalletUIManager"
            );

        objeto.AddComponent<
            WalletAutoUI>();


        Debug.Log(
            "WalletAutoUI creado automaticamente."
        );
    }
}