using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginAutoUI : MonoBehaviour
{
    private TMP_InputField emailInput;
    private TMP_InputField passwordInput;
    private TMP_InputField usernameInput;

    private GameObject usernameZona;

    private TMP_Text titulo;
    private TMP_Text mensaje;
    private TMP_Text cambiarModoTexto;

    private Button botonPrincipal;
    private Button cambiarModoBoton;

    private bool modoRegistro;
    private bool procesando;

    private readonly Color fondo =
        new Color32(18, 20, 25, 255);

    private readonly Color panel =
        new Color32(29, 32, 39, 250);

    private readonly Color campo =
        new Color32(43, 47, 57, 255);

    private readonly Color boton =
        new Color32(41, 163, 112, 255);

    private readonly Color textoSecundario =
        new Color32(175, 181, 193, 255);

    private void Start()
    {
        CrearEventSystemSiHaceFalta();
        CrearInterfaz();
        CambiarModo(false);
    }

    private void CrearInterfaz()
    {
        GameObject canvasObjeto =
            new GameObject(
                "LoginUI",
                typeof(RectTransform)
            );

        Canvas canvas =
            canvasObjeto.AddComponent<Canvas>();

        canvas.renderMode =
            RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler =
            canvasObjeto.AddComponent<CanvasScaler>();

        scaler.uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;

        scaler.referenceResolution =
            new Vector2(1920f, 1080f);

        scaler.matchWidthOrHeight =
            0.5f;

        canvasObjeto.AddComponent<GraphicRaycaster>();

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
    }

    private void CrearPanel(
        Transform padre)
    {
        GameObject objeto =
            CrearObjetoUI(
                "PanelLogin",
                padre
            );

        RectTransform rect =
            objeto.GetComponent<RectTransform>();

        rect.anchorMin =
            new Vector2(0.5f, 0.5f);

        rect.anchorMax =
            new Vector2(0.5f, 0.5f);

        rect.pivot =
            new Vector2(0.5f, 0.5f);

        rect.sizeDelta =
            new Vector2(520f, 660f);

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
        titulo =
            CrearTexto(
                padre,
                "PROYECTO HABBO",
                34f,
                FontStyles.Bold
            );

        ConfigurarRect(
            titulo.rectTransform,
            40f,
            -40f,
            -40f,
            -110f
        );

        TMP_Text subtitulo =
            CrearTexto(
                padre,
                "Tu mundo empieza aqui",
                18f,
                FontStyles.Normal
            );

        subtitulo.color =
            textoSecundario;

        ConfigurarRect(
            subtitulo.rectTransform,
            40f,
            -105f,
            -40f,
            -155f
        );

        emailInput =
            CrearCampo(
                padre,
                "Correo",
                "correo@ejemplo.com",
                false
            );

        ConfigurarRect(
            emailInput.GetComponent<RectTransform>(),
            50f,
            -190f,
            -50f,
            -250f
        );

        passwordInput =
            CrearCampo(
                padre,
                "Contrasena",
                "Minimo 8 caracteres",
                true
            );

        ConfigurarRect(
            passwordInput.GetComponent<RectTransform>(),
            50f,
            -275f,
            -50f,
            -335f
        );

        usernameZona =
            CrearObjetoUI(
                "ZonaNombreUsuario",
                padre
            );

        ConfigurarRect(
            usernameZona.GetComponent<RectTransform>(),
            0f,
            -360f,
            0f,
            -435f
        );

        usernameInput =
            CrearCampo(
                usernameZona.transform,
                "Nombre de usuario",
                "Elige tu nombre",
                false
            );

        ConfigurarRect(
            usernameInput.GetComponent<RectTransform>(),
            50f,
            0f,
            -50f,
            -60f
        );

        botonPrincipal =
            CrearBoton(
                padre,
                "INICIAR SESION",
                EjecutarAccionPrincipal
            );

        ConfigurarRect(
            botonPrincipal.GetComponent<RectTransform>(),
            50f,
            -465f,
            -50f,
            -530f
        );

        cambiarModoBoton =
            CrearBotonSecundario(
                padre,
                CambiarModoDesdeBoton
            );

        ConfigurarRect(
            cambiarModoBoton.GetComponent<RectTransform>(),
            50f,
            -545f,
            -50f,
            -600f
        );

        cambiarModoTexto =
            cambiarModoBoton
                .GetComponentInChildren<TMP_Text>();

        mensaje =
            CrearTexto(
                padre,
                "",
                15f,
                FontStyles.Normal
            );

        mensaje.alignment =
            TextAlignmentOptions.Center;

        ConfigurarRect(
            mensaje.rectTransform,
            40f,
            -605f,
            -40f,
            -650f
        );
    }

    private void CambiarModoDesdeBoton()
    {
        CambiarModo(
            !modoRegistro
        );
    }

    private void CambiarModo(
        bool registro)
    {
        modoRegistro =
            registro;

        usernameZona.SetActive(
            modoRegistro
        );

        titulo.text =
            modoRegistro
                ? "CREAR CUENTA"
                : "INICIAR SESION";

        botonPrincipal
            .GetComponentInChildren<TMP_Text>()
            .text =
            modoRegistro
                ? "CREAR CUENTA"
                : "INICIAR SESION";

        cambiarModoTexto.text =
            modoRegistro
                ? "Ya tengo una cuenta"
                : "Crear una cuenta";

        MostrarMensaje(
            ""
        );
    }

    private async void EjecutarAccionPrincipal()
    {
        if (procesando)
            return;

        if (
            NakamaAuthService.Instance == null
        )
        {
            MostrarMensaje(
                "No se encontro el servicio de conexion.",
                true
            );

            return;
        }

        procesando =
            true;

        botonPrincipal.interactable =
            false;

        MostrarMensaje(
            modoRegistro
                ? "Creando cuenta..."
                : "Iniciando sesion..."
        );

        bool correcto;

        try
        {
            if (modoRegistro)
            {
                correcto =
                    await NakamaAuthService
                        .Instance
                        .Registrar(
                            emailInput.text,
                            passwordInput.text,
                            usernameInput.text
                        );
            }
            else
            {
                correcto =
                    await NakamaAuthService
                        .Instance
                        .IniciarSesion(
                            emailInput.text,
                            passwordInput.text
                        );
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);

            correcto =
                false;
        }

        procesando =
            false;

        botonPrincipal.interactable =
            true;

        if (!correcto)
        {
            MostrarMensaje(
                modoRegistro
                    ? "No se pudo crear la cuenta."
                    : "Correo o contrasena incorrectos.",
                true
            );

            return;
        }

        MostrarMensaje(
            modoRegistro
                ? "Cuenta creada correctamente."
                : "Sesion iniciada correctamente.",
            false
        );

        Debug.Log(
            "Usuario autenticado. ID: " +
            NakamaAuthService
                .Instance
                .Session
                .UserId
        );
    }

    private TMP_InputField CrearCampo(
        Transform padre,
        string etiqueta,
        string placeholder,
        bool password)
    {
        GameObject contenedor =
            CrearObjetoUI(
                "Campo" + etiqueta,
                padre
            );

        Image fondoCampo =
            contenedor.AddComponent<Image>();

        fondoCampo.color =
            campo;

        TMP_InputField input =
            contenedor.AddComponent<TMP_InputField>();

        GameObject textoObjeto =
            CrearObjetoUI(
                "Text",
                contenedor.transform
            );

        TextMeshProUGUI texto =
            textoObjeto.AddComponent<TextMeshProUGUI>();

        texto.fontSize =
            18f;

        texto.color =
            Color.white;

        texto.alignment =
            TextAlignmentOptions.MidlineLeft;

        ConfigurarRect(
            texto.rectTransform,
            18f,
            0f,
            -18f,
            0f
        );

        GameObject placeholderObjeto =
            CrearObjetoUI(
                "Placeholder",
                contenedor.transform
            );

        TextMeshProUGUI placeholderTexto =
            placeholderObjeto
                .AddComponent<TextMeshProUGUI>();

        placeholderTexto.text =
            placeholder;

        placeholderTexto.fontSize =
            17f;

        placeholderTexto.color =
            new Color32(
                135,
                140,
                150,
                255
            );

        placeholderTexto.alignment =
            TextAlignmentOptions.MidlineLeft;

        ConfigurarRect(
            placeholderTexto.rectTransform,
            18f,
            0f,
            -18f,
            0f
        );

        input.textComponent =
            texto;

        input.placeholder =
            placeholderTexto;

        input.lineType =
            TMP_InputField.LineType.SingleLine;

        if (password)
        {
            input.contentType =
                TMP_InputField.ContentType.Password;

            input.asteriskChar =
                '•';
        }

        return input;
    }

    private Button CrearBoton(
        Transform padre,
        string texto,
        UnityEngine.Events.UnityAction accion)
    {
        GameObject objeto =
            CrearObjetoUI(
                "BotonPrincipal",
                padre
            );

        Image imagen =
            objeto.AddComponent<Image>();

        imagen.color =
            boton;

        Button button =
            objeto.AddComponent<Button>();

        button.onClick.AddListener(
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

        return button;
    }

    private Button CrearBotonSecundario(
        Transform padre,
        UnityEngine.Events.UnityAction accion)
    {
        GameObject objeto =
            CrearObjetoUI(
                "CambiarModo",
                padre
            );

        Button button =
            objeto.AddComponent<Button>();

        button.transition =
            Selectable.Transition.ColorTint;

        button.onClick.AddListener(
            accion
        );

        TMP_Text texto =
            CrearTexto(
                objeto.transform,
                "",
                16f,
                FontStyles.Normal
            );

        texto.color =
            boton;

        texto.alignment =
            TextAlignmentOptions.Center;

        Estirar(
            texto.rectTransform
        );

        return button;
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
            objeto.AddComponent<TextMeshProUGUI>();

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
        string texto,
        bool error = false)
    {
        if (mensaje == null)
            return;

        mensaje.text =
            texto;

        mensaje.color =
            error
                ? new Color32(
                    235,
                    90,
                    90,
                    255
                )
                : textoSecundario;
    }

    private void ConfigurarRect(
        RectTransform rect,
        float izquierda,
        float arriba,
        float derecha,
        float abajo)
    {
        rect.anchorMin =
            new Vector2(0f, 1f);

        rect.anchorMax =
            new Vector2(1f, 1f);

        rect.pivot =
            new Vector2(0.5f, 1f);

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

    private void CrearEventSystemSiHaceFalta()
    {
        if (
            FindFirstObjectByType<EventSystem>()
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