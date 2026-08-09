using UnityEngine;
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
    private Material skyboxRuntime;

    private float siguienteActualizacionGI;

    private readonly Color colorNoche =
        new Color(
            0.22f,
            0.30f,
            0.50f
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

        if (skyboxRuntime != null)
        {
            Destroy(
                skyboxRuntime
            );
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

        if (skyboxRuntime != null)
        {
            Destroy(
                skyboxRuntime
            );

            skyboxRuntime = null;
        }

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

        if (
            RenderSettings.skybox != null
        )
        {
            skyboxRuntime =
                new Material(
                    RenderSettings.skybox
                );

            skyboxRuntime.name =
                "Skybox Runtime Dia Noche";

            RenderSettings.skybox =
                skyboxRuntime;
        }

        Debug.Log(
            "Iluminacion dia/noche preparada automaticamente."
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

        luzSolar.intensity =
            Mathf.Lerp(
                0.025f,
                2f,
                luzDia
            );

        RenderSettings.ambientIntensity =
            Mathf.Lerp(
                0.14f,
                1f,
                luzDia
            );

        RenderSettings.reflectionIntensity =
            Mathf.Lerp(
                0.25f,
                1f,
                luzDia
            );

        AplicarSkybox(
            luzDia,
            calidez
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

    private void AplicarSkybox(
        float luzDia,
        float calidez
    )
    {
        if (skyboxRuntime == null)
            return;

        if (
            skyboxRuntime.HasProperty(
                "_Exposure"
            )
        )
        {
            skyboxRuntime.SetFloat(
                "_Exposure",
                Mathf.Lerp(
                    0.08f,
                    1f,
                    luzDia
                )
            );
        }

        if (
            skyboxRuntime.HasProperty(
                "_Tint"
            )
        )
        {
            Color tinteDia =
                Color.Lerp(
                    Color.white,
                    colorAmanecer,
                    calidez * 0.35f
                );

            Color tinte =
                Color.Lerp(
                    new Color(
                        0.22f,
                        0.28f,
                        0.45f
                    ),
                    tinteDia,
                    luzDia
                );

            skyboxRuntime.SetColor(
                "_Tint",
                tinte
            );
        }
    }
}
