using System;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;

public sealed class WorldTimeService : MonoBehaviour
{
    public static WorldTimeService Instance
    {
        get;
        private set;
    }

    public bool EstaSincronizado
    {
        get;
        private set;
    }

    public string ClimaActual
    {
        get;
        private set;
    } = "clear";

    public double TiempoNormalizado
    {
        get
        {
            if (!EstaSincronizado || cycleSeconds <= 0d)
                return 0.5d;

            double servidorEstimado =
                serverUnixAlSincronizar +
                (
                    Time.realtimeSinceStartupAsDouble -
                    tiempoLocalAlSincronizar
                );

            double transcurrido =
                servidorEstimado -
                cycleEpoch;

            double posicion =
                ModuloPositivo(
                    transcurrido,
                    cycleSeconds
                );

            return posicion / cycleSeconds;
        }
    }

    public double HoraJuego =>
        TiempoNormalizado * 24d;

    public event Action EstadoActualizado;

    private const float IntervaloSincronizacion = 20f;
    private const float ReintentoSinConexion = 3f;

    private double serverUnixAlSincronizar;
    private double tiempoLocalAlSincronizar;
    private double cycleSeconds;
    private double cycleEpoch;

    private float siguienteSincronizacion;
    private bool sincronizando;
    private string usuarioSincronizado;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.BeforeSceneLoad
    )]
    private static void CrearAutomaticamente()
    {
        if (Instance != null)
            return;

        GameObject objeto =
            new GameObject(
                "WorldTimeService"
            );

        objeto.AddComponent<
            WorldTimeService>();

        DontDestroyOnLoad(
            objeto
        );
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

    private void Update()
    {
        if (
            NakamaAuthService.Instance == null ||
            !NakamaAuthService
                .Instance
                .EstaAutenticado
        )
        {
            ReiniciarEstado();
            return;
        }

        string usuarioActual =
            NakamaAuthService
                .Instance
                .Session
                .UserId;

        if (
            usuarioSincronizado !=
            usuarioActual
        )
        {
            EstaSincronizado = false;
            usuarioSincronizado =
                usuarioActual;

            siguienteSincronizacion =
                0f;
        }

        if (
            sincronizando ||
            Time.unscaledTime <
            siguienteSincronizacion
        )
        {
            return;
        }

        _ = Sincronizar();
    }

    public async Task<bool> Sincronizar()
    {
        if (
            sincronizando ||
            NakamaConnection.Instance == null ||
            NakamaConnection
                .Instance
                .Client == null ||
            NakamaAuthService.Instance == null ||
            !NakamaAuthService
                .Instance
                .EstaAutenticado
        )
        {
            return false;
        }

        sincronizando = true;

        try
        {
            double inicioPeticion =
                Time.realtimeSinceStartupAsDouble;

            IApiRpc respuesta =
                await NakamaConnection
                    .Instance
                    .Client
                    .RpcAsync(
                        NakamaAuthService
                            .Instance
                            .Session,

                        "world_get_state",

                        "{}"
                    );

            double finPeticion =
                Time.realtimeSinceStartupAsDouble;

            WorldTimeData datos =
                JsonUtility.FromJson<
                    WorldTimeData>(
                    respuesta.Payload
                );

            if (
                datos == null ||
                datos.cycle_seconds <= 0d
            )
            {
                Debug.LogError(
                    "Respuesta de hora mundial invalida."
                );

                ProgramarReintento();
                return false;
            }

            // Compensacion simple del tiempo que tardó
            // la solicitud en ir y volver.
            double mitadLatencia =
                (finPeticion - inicioPeticion) /
                2d;

            serverUnixAlSincronizar =
                datos.server_unix +
                mitadLatencia;

            tiempoLocalAlSincronizar =
                finPeticion;

            cycleSeconds =
                datos.cycle_seconds;

            cycleEpoch =
                datos.cycle_epoch;

            ClimaActual =
                string.IsNullOrWhiteSpace(
                    datos.weather
                )
                    ? "clear"
                    : datos.weather;

            EstaSincronizado = true;

            siguienteSincronizacion =
                Time.unscaledTime +
                IntervaloSincronizacion;

            EstadoActualizado?.Invoke();

            Debug.Log(
                "HORA MUNDIAL SINCRONIZADA -> " +
                HoraJuego.ToString("0.00") +
                "h | clima: " +
                ClimaActual
            );

            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning(
                "No se pudo sincronizar la hora mundial: " +
                e.Message
            );

            ProgramarReintento();
            return false;
        }
        finally
        {
            sincronizando = false;
        }
    }

    private void ProgramarReintento()
    {
        siguienteSincronizacion =
            Time.unscaledTime +
            ReintentoSinConexion;
    }

    private void ReiniciarEstado()
    {
        EstaSincronizado = false;
        usuarioSincronizado = null;
        siguienteSincronizacion = 0f;
    }

    private static double ModuloPositivo(
        double valor,
        double divisor
    )
    {
        double resultado =
            valor % divisor;

        if (resultado < 0d)
            resultado += divisor;

        return resultado;
    }
}
