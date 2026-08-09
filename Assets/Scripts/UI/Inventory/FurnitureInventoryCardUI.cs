using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class FurnitureInventoryCardUI :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler
{
    private Image fondo;
    private Outline borde;
    private RectTransform rect;

    private FurniturePreview3DUI preview3D;
    private Action accionSeleccionar;

    private bool seleccionada;
    private bool hover;

    private readonly Color colorNormal =
        new Color32(27, 31, 37, 255);

    private readonly Color colorHover =
        new Color32(32, 37, 44, 255);

    private Vector3 escalaObjetivo =
        Vector3.one;

    public void Construir(
        GameObject prefab,
        string nombre,
        string categoria,
        string tamano,
        int cantidad,
        Action alSeleccionar)
    {
        accionSeleccionar =
            alSeleccionar;

        rect =
            GetComponent<RectTransform>();

        CrearSombra();

        fondo =
            gameObject.AddComponent<Image>();

        fondo.sprite =
            UIRoundedSpriteFactory.Obtener(17f);

        fondo.type =
            Image.Type.Sliced;

        fondo.color =
            colorNormal;

        borde =
            gameObject.AddComponent<Outline>();

        borde.effectColor =
            new Color32(
                255,
                255,
                255,
                18
            );

        borde.effectDistance =
            new Vector2(1f, -1f);

        CrearPreview(
            prefab
        );

        CrearNombre(
            nombre
        );

        // La cantidad se crea al final para que siempre quede
        // visualmente por encima del preview y de la tarjeta.
        CrearCantidad(
            cantidad
        );

        AplicarEstadoVisual();
    }

    private void Update()
    {
        if (rect == null)
            return;

        rect.localScale =
            Vector3.Lerp(
                rect.localScale,
                escalaObjetivo,
                Time.unscaledDeltaTime *
                16f
            );
    }

    private void CrearSombra()
    {
        GameObject sombra =
            CrearObjeto(
                "Sombra",
                transform
            );

        sombra.transform.SetAsFirstSibling();

        RectTransform r =
            sombra.GetComponent<RectTransform>();

        r.anchorMin =
            Vector2.zero;

        r.anchorMax =
            Vector2.one;

        r.offsetMin =
            new Vector2(-3f, -7f);

        r.offsetMax =
            new Vector2(3f, -1f);

        Image imagen =
            sombra.AddComponent<Image>();

        imagen.sprite =
            UIRoundedSpriteFactory.Obtener(17f);

        imagen.type =
            Image.Type.Sliced;

        imagen.color =
            new Color32(
                0,
                0,
                0,
                72
            );

        imagen.raycastTarget =
            false;
    }

    private void CrearCantidad(
        int cantidad)
    {
        GameObject objeto =
            CrearObjeto(
                "Cantidad",
                transform
            );

        // Garantiza que el badge xN se dibuje delante de todo.
        objeto.transform.SetAsLastSibling();

        RectTransform r =
            objeto.GetComponent<RectTransform>();

        r.anchorMin =
            new Vector2(1f, 1f);

        r.anchorMax =
            new Vector2(1f, 1f);

        r.pivot =
            new Vector2(1f, 1f);

        r.anchoredPosition =
            new Vector2(
                -10f,
                -10f
            );

        r.sizeDelta =
            new Vector2(
                48f,
                30f
            );

        Image bg =
            objeto.AddComponent<Image>();

        bg.sprite =
            UIRoundedSpriteFactory.Obtener(13f);

        bg.type =
            Image.Type.Sliced;

        bg.color =
            new Color32(
                9,
                11,
                14,
                235
            );

        Outline outline =
            objeto.AddComponent<Outline>();

        outline.effectColor =
            new Color32(
                255,
                255,
                255,
                24
            );

        outline.effectDistance =
            new Vector2(1f, -1f);

        TMP_Text texto =
            CrearTexto(
                objeto.transform,
                "x" + cantidad,
                14f,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        texto.color =
            Color.white;
    }

    private void CrearPreview(
        GameObject prefab)
    {
        GameObject contenedor =
            CrearObjeto(
                "Preview3DContainer",
                transform
            );

        RectTransform r =
            contenedor.GetComponent<RectTransform>();

        r.anchorMin =
            new Vector2(0f, 0f);

        r.anchorMax =
            new Vector2(1f, 1f);

        r.offsetMin =
            new Vector2(
                13f,
                55f
            );

        r.offsetMax =
            new Vector2(
                -13f,
                -18f
            );

        Image bg =
            contenedor.AddComponent<Image>();

        bg.sprite =
            UIRoundedSpriteFactory.Obtener(13f);

        bg.type =
            Image.Type.Sliced;

        bg.color =
            new Color32(
                21,
                25,
                30,
                255
            );

        GameObject render =
            CrearObjeto(
                "Render3D",
                contenedor.transform
            );

        RectTransform rRender =
            render.GetComponent<RectTransform>();

        rRender.anchorMin =
            Vector2.zero;

        rRender.anchorMax =
            Vector2.one;

        rRender.offsetMin =
            new Vector2(5f, 5f);

        rRender.offsetMax =
            new Vector2(-5f, -5f);

        RawImage raw =
            render.AddComponent<RawImage>();

        raw.color =
            Color.white;

        raw.raycastTarget =
            false;

        preview3D =
            render.AddComponent<
                FurniturePreview3DUI>();

        preview3D.Inicializar(
            prefab
        );

        if (!preview3D.TienePreview)
        {
            raw.enabled =
                false;

            GameObject fallback =
                CrearObjeto(
                    "FallbackMueble",
                    contenedor.transform
                );

            RectTransform rFallback =
                fallback.GetComponent<RectTransform>();

            rFallback.anchorMin =
                new Vector2(0.5f, 0.5f);

            rFallback.anchorMax =
                new Vector2(0.5f, 0.5f);

            rFallback.pivot =
                new Vector2(0.5f, 0.5f);

            rFallback.sizeDelta =
                new Vector2(48f, 48f);

            Image icono =
                fallback.AddComponent<Image>();

            icono.sprite =
                GameUIIconFactory.Obtener(
                    GameUIIconFactory.Tipo.Mueble
                );

            icono.color =
                GameUITheme.TextoSecundario;

            icono.raycastTarget =
                false;
        }
    }

    private void CrearNombre(
        string nombre)
    {
        TMP_Text texto =
            CrearTexto(
                transform,
                nombre,
                16f,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        texto.color =
            GameUITheme.TextoPrincipal;

        RectTransform r =
            texto.rectTransform;

        r.anchorMin =
            new Vector2(0f, 0f);

        r.anchorMax =
            new Vector2(1f, 0f);

        r.pivot =
            new Vector2(0.5f, 0f);

        r.anchoredPosition =
            new Vector2(
                0f,
                12f
            );

        r.sizeDelta =
            new Vector2(
                -24f,
                32f
            );

        texto.enableAutoSizing =
            true;

        texto.fontSizeMin =
            12f;

        texto.fontSizeMax =
            16f;

        texto.overflowMode =
            TextOverflowModes.Ellipsis;
    }

    public void OnPointerEnter(
        PointerEventData eventData)
    {
        hover =
            true;

        escalaObjetivo =
            new Vector3(
                1.018f,
                1.018f,
                1f
            );

        AplicarEstadoVisual();

        preview3D?.ActivarHover();
    }

    public void OnPointerExit(
        PointerEventData eventData)
    {
        hover =
            false;

        escalaObjetivo =
            Vector3.one;

        AplicarEstadoVisual();

        preview3D?.DesactivarHover();
    }

    public void OnPointerDown(
        PointerEventData eventData)
    {
        escalaObjetivo =
            new Vector3(
                0.988f,
                0.988f,
                1f
            );
    }

    public void OnPointerUp(
        PointerEventData eventData)
    {
        escalaObjetivo =
            hover
                ? new Vector3(
                    1.018f,
                    1.018f,
                    1f
                )
                : Vector3.one;
    }

    public void OnPointerClick(
        PointerEventData eventData)
    {
        if (
            eventData.button !=
            PointerEventData.InputButton.Left
        )
        {
            return;
        }

        accionSeleccionar?.Invoke();
    }

    public void EstablecerSeleccionada(
        bool valor)
    {
        seleccionada =
            valor;

        AplicarEstadoVisual();
    }

    private void AplicarEstadoVisual()
    {
        if (fondo != null)
        {
            fondo.color =
                seleccionada
                    ? new Color32(
                        24,
                        47,
                        39,
                        255
                    )
                    : (
                        hover
                            ? colorHover
                            : colorNormal
                    );
        }

        if (borde != null)
        {
            borde.effectColor =
                seleccionada
                    ? new Color32(
                        35,
                        214,
                        139,
                        230
                    )
                    : (
                        hover
                            ? new Color32(
                                255,
                                255,
                                255,
                                38
                            )
                            : new Color32(
                                255,
                                255,
                                255,
                                18
                            )
                    );

            borde.effectDistance =
                seleccionada
                    ? new Vector2(2f, -2f)
                    : new Vector2(1f, -1f);
        }
    }

    private GameObject CrearObjeto(
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

    private TMP_Text CrearTexto(
        Transform padre,
        string contenido,
        float tamano,
        FontStyles estilo,
        TextAlignmentOptions alineacion)
    {
        GameObject objeto =
            CrearObjeto(
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

        texto.alignment =
            alineacion;

        texto.color =
            GameUITheme.TextoPrincipal;

        texto.raycastTarget =
            false;

        RectTransform rectTexto =
            texto.rectTransform;

        rectTexto.anchorMin =
            Vector2.zero;

        rectTexto.anchorMax =
            Vector2.one;

        rectTexto.offsetMin =
            new Vector2(6f, 3f);

        rectTexto.offsetMax =
            new Vector2(-6f, -3f);

        return texto;
    }
}
