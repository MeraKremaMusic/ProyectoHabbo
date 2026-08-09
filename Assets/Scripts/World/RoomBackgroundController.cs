using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Mantiene el exterior de HabitacionPrincipal completamente negro.
/// No usa Skybox visible: solo la geometria de la habitacion se renderiza
/// sobre un fondo negro solido.
/// </summary>
public sealed class RoomBackgroundController : MonoBehaviour
{
    public static RoomBackgroundController Instance
    {
        get;
        private set;
    }

    private const string EscenaHabitacion = "HabitacionPrincipal";

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

        AplicarFondoNegro();

        Debug.Log(
            "Fondo exterior de la habitacion configurado en negro solido."
        );
    }

    private void LateUpdate()
    {
        if (
            SceneManager.GetActiveScene().name != EscenaHabitacion ||
            camaraPrincipal == null
        )
        {
            return;
        }

        // Se reafirma al final de cada frame para impedir que otro
        // componente o configuracion vuelva a mostrar el Skybox.
        AplicarFondoNegro();
    }

    private void AplicarFondoNegro()
    {
        camaraPrincipal.clearFlags =
            CameraClearFlags.SolidColor;

        camaraPrincipal.backgroundColor =
            Color.black;

        // El cielo deja de existir visualmente en esta escena.
        RenderSettings.skybox = null;
        RenderSettings.fog = false;
    }
}
