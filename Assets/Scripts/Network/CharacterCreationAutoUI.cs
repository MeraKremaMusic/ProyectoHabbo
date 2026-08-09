using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterCreationAutoUI :
    MonoBehaviour
{
    private Button botonCrear;
    private TMP_Text mensaje;

    private CharacterPreview3D preview3D;

    private bool procesando;

    private const string AvatarSeleccionado =
        "personaje_base";

    private readonly Color fondo =
        new Color32(
            18,
            20,
            25,
            255
        );

    private readonly Color panel =
        new Color32(
            29,
            32,
            39,
            250
        );

    private readonly Color tarjeta =
        new Color32(
            43,
            47,
            57,
            255
        );

    private readonly Color verde =
        new Color32(
            41,
            163,
            112,
            255
        );

    private readonly Color gris =
        new Color32(
            175,
            181,
            193,
            255
        );

    private void Start()
    {
        CrearEventSystemSiHaceFalta();

        CrearInterfaz();
    }

    private void CrearInterfaz()
    {
        GameObject canvasObjeto =
            new GameObject(
                "CharacterCreationUI",
                typeof(RectTransform)
            );

        Canvas canvas =
            canvasObjeto.AddComponent<Canvas>();

        canvas.renderMode =
            RenderMode.ScreenSpaceOverlay;

        canvas.sortingOrder =
            200;

        CanvasScaler scaler =
            canvasObjeto
                .AddComponent<CanvasScaler>();

        scaler.uiScaleMode =
            CanvasScaler
                .ScaleMode
                .ScaleWithScreenSize;

        scaler.referenceResolution =
            new Vector2(
                1920f,
                1080f
            );

        scaler.matchWidthOrHeight =
            0.5f;

        canvasObjeto.AddComponent<
            GraphicRaycaster>();

        CrearFondo(
            canvasObjeto.transform
        );

        CrearPanel(
            canvasObjeto.transform
        );
    }

    private void CrearFondo(
        Transform padre)
    {
        GameObject objeto =
            CrearObjetoUI(
                "Fondo",
                padre
            );

        RectTransform rect =
            objeto.GetComponent<RectTransform>();

        rect.anchorMin =
            Vector2.zero;

        rect.anchorMax =
            Vector2.one;

        rect.offsetMin =
            Vector2.zero;

        rect.offsetMax =
            Vector2.zero;

        Image imagen =
            objeto.AddComponent<Image>();

        imagen.color =
            fondo;

        imagen.raycastTarget =
            false;
    }

    private void CrearPanel(
        Transform padre)
    {
        GameObject objeto =
            CrearObjetoUI(
                "PanelCrearPersonaje",
                padre
            );

        RectTransform rect =
            objeto.GetComponent<RectTransform>();

        rect.anchorMin =
            new Vector2(
                0.5f,
                0.5f
            );

        rect.anchorMax =
            new Vector2(
                0.5f,
                0.5f
            );

        rect.pivot =
            new Vector2(
                0.5f,
                0.5f
            );

        rect.anchoredPosition =
            Vector2.zero;

        rect.sizeDelta =
            new Vector2(
                650f,
                760f
            );

        Image imagen =
            objeto.AddComponent<Image>();

        imagen.color =
            panel;

        CrearContenido(
            objeto.transform
        );
    }

    private void CrearContenido(
        Transform padre)
    {
        TMP_Text titulo =
            CrearTexto(
                padre,
                "CREA TU PERSONAJE",
                36f,
                FontStyles.Bold
            );

        ConfigurarRect(
            titulo.rectTransform,
            40f,
            -35f,
            -40f,
            -90f
        );

        string username =
            "Jugador";

        if (
            NakamaAuthService.Instance != null &&
            NakamaAuthService.Instance
                .EstaAutenticado
        )
        {
            username =
                NakamaAuthService
                    .Instance
                    .NombreUsuarioActual;
        }

        TMP_Text usuario =
            CrearTexto(
                padre,
                username,
                19f,
                FontStyles.Normal
            );

        usuario.color =
            gris;

        ConfigurarRect(
            usuario.rectTransform,
            40f,
            -95f,
            -40f,
            -140f
        );

        CrearPreview3D(
            padre
        );

        TMP_Text seleccionado =
            CrearTexto(
                padre,
                "✓ PERSONAJE SELECCIONADO",
                16f,
                FontStyles.Bold
            );

        seleccionado.color =
            verde;

        ConfigurarRect(
            seleccionado.rectTransform,
            50f,
            -555f,
            -50f,
            -600f
        );

        botonCrear =
            CrearBoton(
                padre,
                "CREAR PERSONAJE",
                CrearPersonaje
            );

        ConfigurarRect(
            botonCrear
                .GetComponent<RectTransform>(),
            70f,
            -615f,
            -70f,
            -685f
        );

        mensaje =
            CrearTexto(
                padre,
                "",
                15f,
                FontStyles.Normal
            );

        mensaje.color =
            gris;

        ConfigurarRect(
            mensaje.rectTransform,
            40f,
            -695f,
            -40f,
            -735f
        );
    }

    private void CrearPreview3D(
        Transform padre)
    {
        GameObject contenedor =
            CrearObjetoUI(
                "ContenedorPreview3D",
                padre
            );

        Image fondoPreview =
            contenedor.AddComponent<Image>();

        fondoPreview.color =
            tarjeta;

        ConfigurarRect(
            contenedor.GetComponent<RectTransform>(),
            100f,
            -165f,
            -100f,
            -540f
        );

        GameObject rawObjeto =
            CrearObjetoUI(
                "Personaje3D",
                contenedor.transform
            );

        RawImage rawImage =
            rawObjeto.AddComponent<RawImage>();

        rawImage.color =
            Color.white;

        rawImage.raycastTarget =
            false;

        RectTransform rawRect =
            rawObjeto.GetComponent<RectTransform>();

        rawRect.anchorMin =
            Vector2.zero;

        rawRect.anchorMax =
            Vector2.one;

        rawRect.offsetMin =
            new Vector2(
                20f,
                10f
            );

        rawRect.offsetMax =
            new Vector2(
                -20f,
                -10f
            );

        preview3D =
            gameObject.GetComponent<
                CharacterPreview3D>();

        if (preview3D == null)
        {
            preview3D =
                gameObject.AddComponent<
                    CharacterPreview3D>();
        }

        preview3D.Inicializar(
            rawImage
        );
    }

    private async void CrearPersonaje()
    {
        if (procesando)
            return;

        if (
            NakamaPlayerProfileService
                .Instance == null
        )
        {
            MostrarMensaje(
                "No se encontro el servicio de perfil.",
                true
            );

            return;
        }

        procesando =
            true;

        botonCrear.interactable =
            false;

        MostrarMensaje(
            "Guardando personaje..."
        );

        bool correcto =
            await NakamaPlayerProfileService
                .Instance
                .CrearPersonaje(
                    AvatarSeleccionado
                );

        if (!correcto)
        {
            procesando =
                false;

            botonCrear.interactable =
                true;

            MostrarMensaje(
                "No se pudo guardar el personaje.",
                true
            );

            return;
        }

        MostrarMensaje(
            "Personaje creado correctamente."
        );

        Debug.Log(
            "PERSONAJE CREADO -> " +
            AvatarSeleccionado
        );

        if (
            GameFlowNavigator.Instance != null
        )
        {
            GameFlowNavigator
                .Instance
                .IrAHabitacion();
        }
    }

    private Button CrearBoton(
        Transform padre,
        string texto,
        UnityEngine.Events.UnityAction accion)
    {
        GameObject objeto =
            CrearObjetoUI(
                "BotonCrearPersonaje",
                padre
            );

        Image imagen =
            objeto.AddComponent<Image>();

        imagen.color =
            verde;

        imagen.raycastTarget =
            true;

        Button boton =
            objeto.AddComponent<Button>();

        boton.targetGraphic =
            imagen;

        boton.onClick.AddListener(
            accion
        );

        TMP_Text textoBoton =
            CrearTexto(
                objeto.transform,
                texto,
                18f,
                FontStyles.Bold
            );

        textoBoton.raycastTarget =
            false;

        textoBoton.alignment =
            TextAlignmentOptions.Center;

        Estirar(
            textoBoton.rectTransform
        );

        return boton;
    }

    private TMP_Text CrearTexto(
        Transform padre,
        string contenido,
        float tamano,
        FontStyles estilo)
    {
        GameObject objeto =
            CrearObjetoUI(
                "Texto",
                padre
            );

        TextMeshProUGUI texto =
            objeto.AddComponent<
                TextMeshProUGUI>();

        texto.text =
            contenido;

        texto.fontSize =
            tamano;

        texto.fontStyle =
            estilo;

        texto.color =
            Color.white;

        texto.alignment =
            TextAlignmentOptions.Center;

        texto.raycastTarget =
            false;

        return texto;
    }

    private void MostrarMensaje(
        string contenido,
        bool error = false)
    {
        if (mensaje == null)
            return;

        mensaje.text =
            contenido;

        mensaje.color =
            error
                ? new Color32(
                    235,
                    90,
                    90,
                    255
                )
                : gris;
    }

    private GameObject CrearObjetoUI(
        string nombre,
        Transform padre)
    {
        GameObject objeto =
            new GameObject(
                nombre,
                typeof(RectTransform)
            );

        objeto.transform.SetParent(
            padre,
            false
        );

        return objeto;
    }

    private void ConfigurarRect(
        RectTransform rect,
        float izquierda,
        float arriba,
        float derecha,
        float abajo)
    {
        rect.anchorMin =
            new Vector2(
                0f,
                1f
            );

        rect.anchorMax =
            new Vector2(
                1f,
                1f
            );

        rect.pivot =
            new Vector2(
                0.5f,
                1f
            );

        rect.offsetMin =
            new Vector2(
                izquierda,
                abajo
            );

        rect.offsetMax =
            new Vector2(
                derecha,
                arriba
            );
    }

    private void Estirar(
        RectTransform rect)
    {
        rect.anchorMin =
            Vector2.zero;

        rect.anchorMax =
            Vector2.one;

        rect.offsetMin =
            Vector2.zero;

        rect.offsetMax =
            Vector2.zero;
    }

    private void CrearEventSystemSiHaceFalta()
    {
        if (
            FindAnyObjectByType<EventSystem>()
            != null
        )
        {
            return;
        }

        GameObject objeto =
            new GameObject(
                "EventSystem"
            );

        objeto.AddComponent<EventSystem>();

        objeto.AddComponent<
            UnityEngine.InputSystem.UI
                .InputSystemUIInputModule>();
    }
}