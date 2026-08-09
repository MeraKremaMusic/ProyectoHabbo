using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public sealed class DayNightLightingController :
    MonoBehaviour
{
    public static DayNightLightingController Instance
    {
        get;
        private set;
    }

    private const string EscenaHabitacion =
        "HabitacionPrincipal";

    private Light luzSolar;

    private float siguienteActualizacionGI;

    private readonly Color colorNoche =
        new Color(
            0.34f,
            0.40f,
            0.58f
        );

    private readonly Color colorAmanecer =
        new Color(
            1.00f,
            0.58f,
            0.34f
        );

    private readonly Color colorDia =
        new Color(
            1.00f,
            0.97f,
            0.90f
        );

    private readonly Color ambienteCieloNoche =
        new Color(
            0.18f,
            0.22f,
            0.32f
        );

    private readonly Color ambienteEcuadorNoche =
        new Color(
            0.11f,
            0.14f,
            0.21f
        );

    private readonly Color ambienteSueloNoche =
        new Color(
            0.07f,
            0.08f,
            0.12f
        );

    private readonly Color ambienteCieloDia =
        new Color(
            0.72f,
            0.77f,
            0.84f
        );

    private readonly Color ambienteEcuadorDia =
        new Color(
            0.48f,
            0.50f,
            0.53f
        );

    private readonly Color ambienteSueloDia =
        new Color(
            0.26f,
            0.24f,
            0.22f
        );

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.BeforeSceneLoad
    )]
    private static void CrearAutomaticamente()
    {
        if (Instance != null)
            return;

        GameObject objeto =
            new GameObject(
                "DayNightLightingController"
            );

        objeto.AddComponent<
            DayNightLightingController>();

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

        SceneManager.sceneLoaded +=
            AlCargarEscena;
    }

    private void Start()
    {
        PrepararEscena(
            SceneManager.GetActiveScene()
        );
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -=
                AlCargarEscena;

            Instance = null;
        }
    }

    private void AlCargarEscena(
        Scene escena,
        LoadSceneMode modo
    )
    {
        PrepararEscena(
            escena
        );
    }

    private void PrepararEscena(
        Scene escena
    )
    {
        luzSolar = null;

        if (
            escena.name !=
            EscenaHabitacion
        )
        {
            return;
        }

        Light[] luces =
            FindObjectsByType<Light>(
                FindObjectsSortMode.None
            );

        foreach (
            Light luz in luces
        )
        {
            if (
                luz != null &&
                luz.type ==
                LightType.Directional
            )
            {
                luzSolar = luz;
                break;
            }
        }

        if (luzSolar == null)
        {
            Debug.LogWarning(
                "No se encontro una Directional Light para el ciclo dia/noche."
            );

            return;
        }

        RenderSettings.sun =
            luzSolar;

        // La habitacion usa iluminacion ambiental propia.
        // No dependemos del Skybox, porque el exterior debe ser negro.
        RenderSettings.ambientMode =
            AmbientMode.Trilight;

        RenderSettings.skybox = null;
        RenderSettings.fog = false;

        Debug.Log(
            "Iluminacion dia/noche preparada con noche visible y fondo negro."
        );
    }

    private void Update()
    {
        if (
            luzSolar == null ||
            SceneManager
                .GetActiveScene()
                .name !=
            EscenaHabitacion ||
            WorldTimeService.Instance == null ||
            !WorldTimeService
                .Instance
                .EstaSincronizado
        )
        {
            return;
        }

        float tiempo =
            (float)WorldTimeService
                .Instance
                .TiempoNormalizado;

        AplicarIluminacion(
            tiempo
        );
    }

    private void AplicarIluminacion(
        float tiempo
    )
    {
        // 0.00 = 00:00
        // 0.25 = 06:00
        // 0.50 = 12:00
        // 0.75 = 18:00
        float hora =
            tiempo * 24f;

        float anguloSol =
            tiempo * 360f -
            90f;

        luzSolar.transform.rotation =
            Quaternion.Euler(
                anguloSol,
                -30f,
                0f
            );

        float alturaSol =
            Mathf.Sin(
                (
                    tiempo -
                    0.25f
                ) *
                Mathf.PI *
                2f
            );

        float luzDia =
            Mathf.SmoothStep(
                0f,
                1f,
                Mathf.InverseLerp(
                    -0.12f,
                    0.18f,
                    alturaSol
                )
            );

        float cercaniaAmanecer =
            Mathf.Clamp01(
                1f -
                Mathf.Abs(
                    hora -
                    6f
                ) /
                2.2f
            );

        float cercaniaAtardecer =
            Mathf.Clamp01(
                1f -
                Mathf.Abs(
                    hora -
                    18f
                ) /
                2.2f
            );

        float calidez =
            Mathf.Max(
                cercaniaAmanecer,
                cercaniaAtardecer
            );

        Color colorDiurno =
            Color.Lerp(
                colorDia,
                colorAmanecer,
                calidez
            );

        luzSolar.color =
            Color.Lerp(
                colorNoche,
                colorDiurno,
                luzDia
            );

        // Antes bajaba hasta 0.025 y la habitacion desaparecia.
        // Ahora la noche sigue siendo oscura, pero los muebles,
        // paredes, piso y jugador permanecen visibles.
        luzSolar.intensity =
            Mathf.Lerp(
                0.22f,
                2f,
                luzDia
            );

        RenderSettings.ambientSkyColor =
            Color.Lerp(
                ambienteCieloNoche,
                ambienteCieloDia,
                luzDia
            );

        RenderSettings.ambientEquatorColor =
            Color.Lerp(
                ambienteEcuadorNoche,
                ambienteEcuadorDia,
                luzDia
            );

        RenderSettings.ambientGroundColor =
            Color.Lerp(
                ambienteSueloNoche,
                ambienteSueloDia,
                luzDia
            );

        // El fondo debe seguir negro independientemente de la hora.
        RenderSettings.skybox = null;
        RenderSettings.fog = false;

        RenderSettings.reflectionIntensity =
            Mathf.Lerp(
                0.45f,
                1f,
                luzDia
            );

        if (
            Time.unscaledTime >=
            siguienteActualizacionGI
        )
        {
            siguienteActualizacionGI =
                Time.unscaledTime +
                2f;

            DynamicGI.UpdateEnvironment();
        }
    }
}
