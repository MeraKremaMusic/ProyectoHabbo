using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterCreationAutoUI :
    MonoBehaviour
{
    private Button botonCrear;

    private Button botonAnterior;
    private Button botonSiguiente;

    private TMP_Text mensaje;
    private TMP_Text nombreAvatar;
    private TMP_Text contadorAvatar;

    private RawImage previewRawImage;

    private CharacterPreview3D preview3D;

    private AvatarDefinition[] avatares;

    private AvatarDefinition avatarSeleccionado;

    private int indiceActual;

    private bool procesando;

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
        avatares =
            AvatarRegistry
                .ObtenerAvatares();

        CrearEventSystemSiHaceFalta();

        CrearInterfaz();

        InicializarSelector();
    }

    private void CrearInterfaz()
    {
        GameObject canvasObjeto =
            new GameObject(
                "CharacterCreationUI",
                typeof(RectTransform)
            );

        Canvas canvas =
            canvasObjeto.AddComponent<
                Canvas>();

        canvas.renderMode =
            RenderMode.ScreenSpaceOverlay;

        canvas.sortingOrder =
            200;

        CanvasScaler scaler =
            canvasObjeto.AddComponent<
                CanvasScaler>();

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
            objeto.GetComponent<
                RectTransform>();

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
            objeto.GetComponent<
                RectTransform>();

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
                720f,
                820f
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
            -30f,
            -40f,
            -85f
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
            -90f,
            -40f,
            -135f
        );

        CrearPreview3D(
            padre
        );

        CrearSelector(
            padre
        );

        botonCrear =
            CrearBotonPrincipal(
                padre,
                "CREAR PERSONAJE",
                CrearPersonaje
            );

        ConfigurarRect(
            botonCrear.GetComponent<
                RectTransform>(),
            80f,
            -690f,
            -80f,
            -755f
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
            -760f,
            -40f,
            -805f
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
            contenedor.GetComponent<
                RectTransform>(),
            130f,
            -155f,
            -130f,
            -535f
        );

        GameObject rawObjeto =
            CrearObjetoUI(
                "Personaje3D",
                contenedor.transform
            );

        previewRawImage =
            rawObjeto.AddComponent<
                RawImage>();

        previewRawImage.color =
            Color.white;

        previewRawImage.raycastTarget =
            false;

        RectTransform rawRect =
            rawObjeto.GetComponent<
                RectTransform>();

        rawRect.anchorMin =
            Vector2.zero;

        rawRect.anchorMax =
            Vector2.one;

        rawRect.offsetMin =
            new Vector2(
                15f,
                10f
            );

        rawRect.offsetMax =
            new Vector2(
                -15f,
                -10f
            );

        preview3D =
            GetComponent<
                CharacterPreview3D>();

        if (preview3D == null)
        {
            preview3D =
                gameObject.AddComponent<
                    CharacterPreview3D>();
        }
    }

    private void CrearSelector(
        Transform padre)
    {
        botonAnterior =
            CrearBotonFlecha(
                padre,
                "<",
                AvatarAnterior
            );

        ConfigurarRect(
            botonAnterior.GetComponent<
                RectTransform>(),
            110f,
            -555f,
            -520f,
            -625f
        );

        botonSiguiente =
            CrearBotonFlecha(
                padre,
                ">",
                AvatarSiguiente
            );

        ConfigurarRect(
            botonSiguiente.GetComponent<
                RectTransform>(),
            520f,
            -555f,
            -110f,
            -625f
        );

        nombreAvatar =
            CrearTexto(
                padre,
                "",
                21f,
                FontStyles.Bold
            );

        ConfigurarRect(
            nombreAvatar.rectTransform,
            200f,
            -550f,
            -200f,
            -590f
        );

        contadorAvatar =
            CrearTexto(
                padre,
                "",
                15f,
                FontStyles.Normal
            );

        contadorAvatar.color =
            gris;

        ConfigurarRect(
            contadorAvatar.rectTransform,
            200f,
            -590f,
            -200f,
            -625f
        );

        TMP_Text seleccionado =
            CrearTexto(
                padre,
                "PERSONAJE SELECCIONADO",
                15f,
                FontStyles.Bold
            );

        seleccionado.color =
            verde;

        ConfigurarRect(
            seleccionado.rectTransform,
            100f,
            -635f,
            -100f,
            -675f
        );
    }

    private void InicializarSelector()
    {
        if (
            avatares == null ||
            avatares.Length == 0
        )
        {
            botonCrear.interactable =
                false;

            botonAnterior.interactable =
                false;

            botonSiguiente.interactable =
                false;

            MostrarMensaje(
                "No hay avatares disponibles.",
                true
            );

            Debug.LogError(
                "No existen prefabs en " +
                "Resources/Characters."
            );

            return;
        }

        indiceActual =
            0;

        ActualizarSeleccion();
    }

    private void AvatarAnterior()
    {
        if (
            procesando ||
            avatares == null ||
            avatares.Length <= 1
        )
        {
            return;
        }

        indiceActual--;

        if (indiceActual < 0)
        {
            indiceActual =
                avatares.Length - 1;
        }

        ActualizarSeleccion();
    }

    private void AvatarSiguiente()
    {
        if (
            procesando ||
            avatares == null ||
            avatares.Length <= 1
        )
        {
            return;
        }

        indiceActual++;

        if (
            indiceActual >=
            avatares.Length
        )
        {
            indiceActual =
                0;
        }

        ActualizarSeleccion();
    }

    private void ActualizarSeleccion()
    {
        if (
            avatares == null ||
            avatares.Length == 0
        )
        {
            return;
        }

        avatarSeleccionado =
            avatares[
                indiceActual
            ];

        nombreAvatar.text =
            avatarSeleccionado.Nombre;

        contadorAvatar.text =
            (indiceActual + 1) +
            " / " +
            avatares.Length;

        bool hayVarios =
            avatares.Length > 1;

        botonAnterior.interactable =
            hayVarios;

        botonSiguiente.interactable =
            hayVarios;

        if (
            preview3D != null &&
            previewRawImage != null
        )
        {
            if (
                string.IsNullOrWhiteSpace(
                    preview3D.AvatarActual
                )
            )
            {
                preview3D.Inicializar(
                    previewRawImage,
                    avatarSeleccionado.Id
                );
            }
            else
            {
                preview3D.MostrarAvatar(
                    avatarSeleccionado.Id
                );
            }
        }

        MostrarMensaje("");

        Debug.Log(
            "Avatar seleccionado -> " +
            avatarSeleccionado.Id
        );
    }

    private async void CrearPersonaje()
    {
        if (procesando)
            return;

        if (avatarSeleccionado == null)
        {
            MostrarMensaje(
                "Selecciona un personaje.",
                true
            );

            return;
        }

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

        botonAnterior.interactable =
            false;

        botonSiguiente.interactable =
            false;

        MostrarMensaje(
            "Guardando personaje..."
        );

        bool correcto =
            await NakamaPlayerProfileService
                .Instance
                .CrearPersonaje(
                    avatarSeleccionado.Id
                );

        if (!correcto)
        {
            procesando =
                false;

            botonCrear.interactable =
                true;

            bool hayVarios =
                avatares != null &&
                avatares.Length > 1;

            botonAnterior.interactable =
                hayVarios;

            botonSiguiente.interactable =
                hayVarios;

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
            avatarSeleccionado.Id
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

    private Button CrearBotonPrincipal(
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

        textoBoton.alignment =
            TextAlignmentOptions.Center;

        Estirar(
            textoBoton.rectTransform
        );

        return boton;
    }

    private Button CrearBotonFlecha(
        Transform padre,
        string texto,
        UnityEngine.Events.UnityAction accion)
    {
        GameObject objeto =
            CrearObjetoUI(
                "BotonFlecha",
                padre
            );

        Image imagen =
            objeto.AddComponent<Image>();

        imagen.color =
            tarjeta;

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
                30f,
                FontStyles.Bold
            );

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
            FindAnyObjectByType<
                EventSystem>() != null
        )
        {
            return;
        }

        GameObject objeto =
            new GameObject(
                "EventSystem"
            );

        objeto.AddComponent<
            EventSystem>();

        objeto.AddComponent<
            UnityEngine.InputSystem.UI
                .InputSystemUIInputModule>();
    }
}