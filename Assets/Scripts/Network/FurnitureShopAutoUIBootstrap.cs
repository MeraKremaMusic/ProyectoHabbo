using UnityEngine;
using UnityEngine.SceneManagement;

public static class FurnitureShopAutoUIBootstrap
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


        FurnitureShopAutoUI existente =
            Object.FindAnyObjectByType<
                FurnitureShopAutoUI>();


        if (existente != null)
            return;


        GameObject objeto =
            new GameObject(
                "FurnitureShopUIManager"
            );


        objeto.AddComponent<
            FurnitureShopAutoUI>();


        Debug.Log(
            "FurnitureShopAutoUI creado automaticamente."
        );
    }
}