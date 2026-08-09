using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Oculta visualmente el Skybox en la habitación y deja un fondo plano.
/// El Skybox sigue existiendo en RenderSettings para aportar iluminación
/// y reflejos al sistema de día y noche; la cámara simplemente no lo dibuja.
/// </summary>
public sealed class RoomBackgroundController : MonoBehaviour
{
    public static RoomBackgroundController Instance
    {
        get;
        private set;
    }

    private const string EscenaHabitacion = "HabitacionPrincipal";

    private static readonly Color FondoHabitacion =
        new Color32(16, 18, 22, 255);

    private Camera camaraPrincipal;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.BeforeSceneLoad
    )]
    private static void CrearAutomaticamente()
    {
        if (Instance != null)
            return;

        GameObject objeto =
            new GameObject("RoomBackgroundController");

        objeto.AddComponent<RoomBackgroundController>();

        DontDestroyOnLoad(objeto);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private void Start()
    {
        ConfigurarEscena(SceneManager.GetActiveScene());
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= AlCargarEscena;
            Instance = null;
        }
    }

    private void AlCargarEscena(
        Scene escena,
        LoadSceneMode modo
    )
    {
        ConfigurarEscena(escena);
    }

    private void ConfigurarEscena(Scene escena)
    {
        camaraPrincipal = null;

        if (escena.name != EscenaHabitacion)
            return;

        camaraPrincipal = Camera.main;

        if (camaraPrincipal == null)
        {
            camaraPrincipal =
                FindFirstObjectByType<Camera>();
        }

        if (camaraPrincipal == null)
        {
            Debug.LogWarning(
                "RoomBackgroundController: no se encontro una camara."
            );
            return;
        }

        camaraPrincipal.clearFlags =
            CameraClearFlags.SolidColor;

        camaraPrincipal.backgroundColor =
            FondoHabitacion;

        Debug.Log(
            "Fondo de la habitacion limpio: Skybox oculto para la camara."
        );
    }
}