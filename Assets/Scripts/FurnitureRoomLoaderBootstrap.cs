using UnityEngine;
using UnityEngine.SceneManagement;

public static class FurnitureRoomLoaderBootstrap
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


        FurnitureRoomLoader existente =
            Object.FindAnyObjectByType<
                FurnitureRoomLoader>();


        if (existente != null)
            return;


        GameObject objeto =
            new GameObject(
                "FurnitureRoomLoader"
            );


        objeto.AddComponent<
            FurnitureRoomLoader>();


        Debug.Log(
            "FurnitureRoomLoader creado automaticamente."
        );
    }
}