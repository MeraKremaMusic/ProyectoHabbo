using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class LogoutAutoUI : MonoBehaviour
{
    private static LogoutAutoUI instance;

    private GameObject logoutCanvas;
    private Button logoutButton;
    private bool procesando;

    private readonly Color fondoBoton = new Color32(24, 27, 33, 235);
    private readonly Color fondoHover = new Color32(43, 47, 57, 255);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InicializarAutomaticamente()
    {
        if (instance != null)
            return;

        GameObject objeto = new GameObject("LogoutAutoUIManager");
        DontDestroyOnLoad(objeto);
        objeto.AddComponent<LogoutAutoUI>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += AlCargarEscena;
    }

    private void Start()
    {
        ActualizarParaEscena(SceneManager.GetActiveScene());
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= AlCargarEscena;
            instance = null;
        }
    }

    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        ActualizarParaEscena(escena);
    }

    private void ActualizarParaEscena(Scene escena)
    {
        bool mostrar =
            escena.name == "HabitacionPrincipal" &&
            NakamaAuthService.Instance != null &&
            NakamaAuthService.Instance.EstaAutenticado;

        if (mostrar)
        {
            CrearInterfazSiHaceFalta();
        }
        else
        {
            DestruirInterfaz();
        }
    }

    private void CrearInterfazSiHaceFalta()
    {
        if (logoutCanvas != null)
            return;

        CrearEventSystemSiHaceFalta();

        logoutCanvas = new GameObject(
            "LogoutUI",
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        logoutCanvas.transform.SetParent(transform, false);

        Canvas canvas = logoutCanvas.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 2000;

        CanvasScaler scaler = logoutCanvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject botonObjeto = new GameObject(
            "BotonCerrarSesion",
            typeof(RectTransform),
            typeof(Image),
            typeof(Button)
        );

        botonObjeto.transform.SetParent(logoutCanvas.transform, false);

        RectTransform rect = botonObjeto.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-24f, -24f);
        rect.sizeDelta = new Vector2(190f, 52f);

        Image imagen = botonObjeto.GetComponent<Image>();
        imagen.color = fondoBoton;

        logoutButton = botonObjeto.GetComponent<Button>();

        ColorBlock colores = logoutButton.colors;
        colores.normalColor = Color.white;
        colores.highlightedColor = fondoHover;
        colores.pressedColor = new Color32(15, 17, 21, 255);
        colores.selectedColor = Color.white;
        colores.disabledColor = new Color32(90, 90, 90, 180);
        colores.colorMultiplier = 1f;
        logoutButton.colors = colores;

        logoutButton.onClick.AddListener(() => _ = CerrarSesion());

        GameObject textoObjeto = new GameObject(
            "Texto",
            typeof(RectTransform),
            typeof(TextMeshProUGUI)
        );

        textoObjeto.transform.SetParent(botonObjeto.transform, false);

        RectTransform textoRect = textoObjeto.GetComponent<RectTransform>();
        textoRect.anchorMin = Vector2.zero;
        textoRect.anchorMax = Vector2.one;
        textoRect.offsetMin = Vector2.zero;
        textoRect.offsetMax = Vector2.zero;

        TextMeshProUGUI texto = textoObjeto.GetComponent<TextMeshProUGUI>();
        texto.text = "Cerrar sesion";
        texto.fontSize = 20f;
        texto.color = Color.white;
        texto.alignment = TextAlignmentOptions.Center;
        texto.raycastTarget = false;

        Debug.Log("Boton de cerrar sesion creado automaticamente.");
    }

    private async Task CerrarSesion()
    {
        if (procesando)
            return;

        procesando = true;

        if (logoutButton != null)
            logoutButton.interactable = false;

        NakamaAuthService auth = NakamaAuthService.Instance;

        try
        {
            if (
                auth != null &&
                auth.Session != null &&
                NakamaConnection.Instance != null &&
                NakamaConnection.Instance.Client != null
            )
            {
                await NakamaConnection.Instance.Client.SessionLogoutAsync(
                    auth.Session
                );

                Debug.Log("Sesion cerrada correctamente en Nakama.");
            }
        }
        catch (Exception e)
        {
            // Aunque el servidor no responda, permitimos salir del juego
            // limpiando la sesion local.
            Debug.LogWarning(
                "No se pudo cerrar la sesion en el servidor: " +
                e.Message
            );
        }
        finally
        {
            if (auth != null)
                auth.CerrarSesionLocal();

            procesando = false;
            DestruirInterfaz();

            SceneManager.LoadScene("InicioSesion");
        }
    }

    private void DestruirInterfaz()
    {
        if (logoutCanvas != null)
        {
            Destroy(logoutCanvas);
            logoutCanvas = null;
            logoutButton = null;
        }
    }

    private void CrearEventSystemSiHaceFalta()
    {
        EventSystem eventSystem =
            FindFirstObjectByType<EventSystem>();

        if (eventSystem != null)
            return;

        GameObject objeto = new GameObject("EventSystem");
        objeto.AddComponent<EventSystem>();
        objeto.AddComponent<
            UnityEngine.InputSystem.UI.InputSystemUIInputModule
        >();

        Debug.Log("EventSystem creado automaticamente para LogoutUI.");
    }
}