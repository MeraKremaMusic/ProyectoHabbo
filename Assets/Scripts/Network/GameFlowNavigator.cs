using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowNavigator : MonoBehaviour
{
    public static GameFlowNavigator Instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        if (
            Instance != null &&
            Instance != this
        )
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void IrA(
        GameFlowDestination destino)
    {
        switch (destino)
        {
            case GameFlowDestination.CrearPersonaje:
                MostrarCreacionPersonaje();
                break;

            case GameFlowDestination.HabitacionPrincipal:
                IrAHabitacion();
                break;
        }
    }

    private void MostrarCreacionPersonaje()
    {
        GameObject loginUI =
            GameObject.Find("LoginUI");

        if (loginUI != null)
        {
            Destroy(loginUI);
        }

        CharacterCreationAutoUI existente =
            FindAnyObjectByType<CharacterCreationAutoUI>();

        if (existente != null)
            return;

        GameObject objeto =
            new GameObject(
                "CharacterCreationManager"
            );

        objeto.AddComponent<
            CharacterCreationAutoUI>();

        Debug.Log(
            "Mostrando creacion de personaje."
        );
    }

    public void IrAHabitacion()
    {
        Debug.Log(
            "Entrando a HabitacionPrincipal."
        );

        SceneManager.LoadScene(
            "HabitacionPrincipal"
        );
    }
}