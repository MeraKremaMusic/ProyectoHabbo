using System.Threading.Tasks;
using UnityEngine;

public enum GameFlowDestination
{
    Ninguno,
    CrearPersonaje,
    HabitacionPrincipal
}

public class GameFlowService : MonoBehaviour
{
    public static GameFlowService Instance
    {
        get;
        private set;
    }

    public GameFlowDestination UltimoDestino
    {
        get;
        private set;
    }

    public bool Procesando
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

        DontDestroyOnLoad(
            gameObject
        );
    }

    public async Task<GameFlowDestination>
        DeterminarDestinoDespuesDeLogin()
    {
        if (Procesando)
        {
            return UltimoDestino;
        }

        if (
            NakamaAuthService.Instance == null ||
            !NakamaAuthService.Instance.EstaAutenticado
        )
        {
            Debug.LogError(
                "GameFlow: no hay una sesion autenticada."
            );

            UltimoDestino =
                GameFlowDestination.Ninguno;

            return UltimoDestino;
        }

        if (
            NakamaPlayerProfileService.Instance == null
        )
        {
            Debug.LogError(
                "GameFlow: no existe " +
                "NakamaPlayerProfileService."
            );

            UltimoDestino =
                GameFlowDestination.Ninguno;

            return UltimoDestino;
        }

        Procesando =
            true;

        try
        {
            PlayerProfileData perfil =
                await NakamaPlayerProfileService
                    .Instance
                    .CargarPerfil();

            if (
                perfil == null ||
                !perfil.personajeCreado
            )
            {
                UltimoDestino =
                    GameFlowDestination
                        .CrearPersonaje;

                Debug.Log(
                    "GAME FLOW -> CREAR PERSONAJE"
                );

                return UltimoDestino;
            }

            UltimoDestino =
                GameFlowDestination
                    .HabitacionPrincipal;

            Debug.Log(
                "GAME FLOW -> HABITACION PRINCIPAL" +
                " | Avatar: " +
                perfil.avatarId
            );

            return UltimoDestino;
        }
        finally
        {
            Procesando =
                false;
        }
    }
}