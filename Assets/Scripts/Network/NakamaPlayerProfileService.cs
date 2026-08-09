using System;
using System.Linq;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;

public class NakamaPlayerProfileService : MonoBehaviour
{
    public static NakamaPlayerProfileService Instance
    {
        get;
        private set;
    }

    public PlayerProfileData PerfilActual
    {
        get;
        private set;
    }

    private const string Coleccion =
        "player_profile";

    private const string Clave =
        "main";

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

    public async Task<PlayerProfileData> CargarPerfil()
    {
        if (
            NakamaConnection.Instance == null ||
            NakamaAuthService.Instance == null ||
            !NakamaAuthService.Instance.EstaAutenticado
        )
        {
            return null;
        }

        try
        {
            ISession sesion =
                NakamaAuthService.Instance.Session;

            StorageObjectId id =
                new StorageObjectId
                {
                    Collection = Coleccion,
                    Key = Clave,
                    UserId = sesion.UserId
                };

            IApiStorageObjects resultado =
                await NakamaConnection
                    .Instance
                    .Client
                    .ReadStorageObjectsAsync(
                        sesion,
                        new[] { id }
                    );

            if (
                resultado.Objects == null ||
                !resultado.Objects.Any()
            )
            {
                PerfilActual = null;
                return null;
            }

            IApiStorageObject objeto =
                resultado.Objects.First();

            PerfilActual =
                JsonUtility.FromJson<PlayerProfileData>(
                    objeto.Value
                );

            return PerfilActual;
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Error cargando perfil: " +
                e.Message
            );

            return null;
        }
    }

    public async Task<bool> CrearPersonaje(
        string avatarId)
    {
        if (
            string.IsNullOrWhiteSpace(avatarId) ||
            NakamaConnection.Instance == null ||
            NakamaAuthService.Instance == null ||
            !NakamaAuthService.Instance.EstaAutenticado
        )
        {
            return false;
        }

        try
        {
            PlayerProfileData perfil =
                new PlayerProfileData
                {
                    personajeCreado = true,
                    avatarId = avatarId
                };

            string json =
                JsonUtility.ToJson(perfil);

            WriteStorageObject objeto =
                new WriteStorageObject
                {
                    Collection = Coleccion,
                    Key = Clave,
                    Value = json,

                    // Otros jugadores podrán leer
                    // nuestro avatar en el futuro.
                    PermissionRead = 2,

                    // Solo el dueño puede modificarlo.
                    PermissionWrite = 1
                };

            await NakamaConnection
                .Instance
                .Client
                .WriteStorageObjectsAsync(
                    NakamaAuthService.Instance.Session,
                    new[] { objeto }
                );

            PerfilActual = perfil;

            Debug.Log(
                "Personaje guardado. Avatar: " +
                avatarId
            );

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Error guardando personaje: " +
                e.Message
            );

            return false;
        }
    }
}