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
    IPointerUpHandler
{
    private Image fondo;
    private Outline borde;
    private Button botonColocar;
    private RectTransform rect;

    private FurniturePreview3DUI
        preview3D;

    private Action accionColocar;

    private Color colorNormal;
    private Color colorHover;

    private Vector3 escalaObjetivo =
        Vector3.one;

    private bool hover;

    public void Construir(
        GameObject prefab,
        string nombre,
        string categoria,
        string tamano,
        int cantidad,
        Action alColocar)
    {
        accionColocar =
            alColocar;

        rect =
            GetComponent<RectTransform>();

        CrearSombra();

        fondo =
            gameObject.AddComponent<Image>();

        fondo.sprite =
            UIRoundedSpriteFactory.Obtener(
                GameUITheme.RadioTarjeta
            );

        fondo.type =
            Image.Type.Sliced;

        colorNormal =
            GameUITheme.Tarjeta;

        colorHover =
            GameUITheme.TarjetaHover;

        fondo.color =
            colorNormal;

        borde =
            gameObject.AddComponent<Outline>();

        borde.effectColor =
            GameUITheme.BordeSuave;

        borde.effectDistance =
            new Vector2(1f, -1f);

        CrearEtiquetaCategoria(
            categoria
        );

        CrearCantidad(
            cantidad
        );

        CrearPreview(
            prefab
        );

        CrearSeparador();

        CrearNombre(
            nombre
        );

        CrearMeta(
            tamano
        );

        CrearBoton();
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
                15f
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
            new Vector2(
                -4f,
                -9f
            );

        r.offsetMax =
            new Vector2(
                4f,
                -1f
            );

        Image imagen =
            sombra.AddComponent<Image>();

        imagen.sprite =
            UIRoundedSpriteFactory.Obtener(
                GameUITheme.RadioTarjeta
            );

        imagen.type =
            Image.Type.Sliced;

        imagen.color =
            new Color32(
                0,
                0,
                0,
                82
            );

        imagen.raycastTarget =
            false;
    }

    private void CrearEtiquetaCategoria(
        string categoria)
    {
        GameObject objeto =
            CrearObjeto(
                "Categoria",
                transform
            );

        RectTransform r =
            objeto.GetComponent<RectTransform>();

        r.anchorMin =
            new Vector2(0f, 1f);

        r.anchorMax =
            new Vector2(0f, 1f);

        r.pivot =
            new Vector2(0f, 1f);

        r.anchoredPosition =
            new Vector2(13f, -12f);

        r.sizeDelta =
            new Vector2(112f, 27f);

        Image bg =
            objeto.AddComponent<Image>();

        bg.sprite =
            UIRoundedSpriteFactory.Obtener(
                12f
            );

        bg.type =
            Image.Type.Sliced;

        bg.color =
            GameUITheme.EsmeraldaSuave;

        TMP_Text texto =
            CrearTexto(
                objeto.transform,
                NombreCategoria(
                    categoria
                ),
                11.5f,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        texto.color =
            GameUITheme.Esmeralda;
    }

    private void CrearCantidad(
        int cantidad)
    {
        GameObject objeto =
            CrearObjeto(
                "Cantidad",
                transform
            );

        RectTransform r =
            objeto.GetComponent<RectTransform>();

        r.anchorMin =
            new Vector2(1f, 1f);

        r.anchorMax =
            new Vector2(1f, 1f);

        r.pivot =
            new Vector2(1f, 1f);

        r.anchoredPosition =
            new Vector2(-12f, -12f);

        r.sizeDelta =
            new Vector2(48f, 28f);

        Image bg =
            objeto.AddComponent<Image>();

        bg.sprite =
            UIRoundedSpriteFactory.Obtener(
                12f
            );

        bg.type =
            Image.Type.Sliced;

        bg.color =
            new Color32(
                255,
                255,
                255,
                18
            );

        TMP_Text texto =
            CrearTexto(
                objeto.transform,
                "x" + cantidad,
                12.5f,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        texto.color =
            GameUITheme.TextoPrincipal;
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
            contenedor.GetComponent<
                RectTransform>();

        r.anchorMin =
            new Vector2(0.5f, 1f);

        r.anchorMax =
            new Vector2(0.5f, 1f);

        r.pivot =
            new Vector2(0.5f, 1f);

        r.anchoredPosition =
            new Vector2(0f, -48f);

        r.sizeDelta =
            new Vector2(190f, 112f);

        Image bg =
            contenedor.AddComponent<Image>();

        bg.sprite =
            UIRoundedSpriteFactory.Obtener(
                18f
            );

        bg.type =
            Image.Type.Sliced;

        bg.color =
            new Color32(
                17,
                20,
                25,
                255
            );

        GameObject brillo =
            CrearObjeto(
                "BrilloPreview",
                contenedor.transform
            );

        RectTransform rBrillo =
            brillo.GetComponent<RectTransform>();

        rBrillo.anchorMin =
            Vector2.zero;

        rBrillo.anchorMax =
            Vector2.one;

        rBrillo.offsetMin =
            new Vector2(7f, 7f);

        rBrillo.offsetMax =
            new Vector2(-7f, -7f);

        Image brilloImagen =
            brillo.AddComponent<Image>();

        brilloImagen.sprite =
            UIRoundedSpriteFactory.Obtener(
                16f
            );

        brilloImagen.type =
            Image.Type.Sliced;

        brilloImagen.color =
            new Color32(
                34,
                197,
                94,
                10
            );

        brilloImagen.raycastTarget =
            false;

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
            new Vector2(7f, 7f);

        rRender.offsetMax =
            new Vector2(-7f, -7f);

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
                fallback.GetComponent<
                    RectTransform>();

            rFallback.anchorMin =
                new Vector2(0.5f, 0.5f);

            rFallback.anchorMax =
                new Vector2(0.5f, 0.5f);

            rFallback.pivot =
                new Vector2(0.5f, 0.5f);

            rFallback.sizeDelta =
                new Vector2(45f, 45f);

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

    private void CrearSeparador()
    {
        GameObject objeto =
            CrearObjeto(
                "Separador",
                transform
            );

        RectTransform r =
            objeto.GetComponent<RectTransform>();

        r.anchorMin =
            new Vector2(0f, 1f);

        r.anchorMax =
            new Vector2(1f, 1f);

        r.pivot =
            new Vector2(0.5f, 1f);

        r.anchoredPosition =
            new Vector2(0f, -169f);

        r.sizeDelta =
            new Vector2(-26f, 1f);

        Image imagen =
            objeto.AddComponent<Image>();

        imagen.color =
            new Color32(
                255,
                255,
                255,
                18
            );

        imagen.raycastTarget =
            false;
    }

    private void CrearNombre(
        string nombre)
    {
        TMP_Text texto =
            CrearTexto(
                transform,
                nombre,
                17f,
                FontStyles.Bold,
                TextAlignmentOptions.Left
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
            new Vector2(0f, 76f);

        r.sizeDelta =
            new Vector2(-28f, 28f);

        texto.enableAutoSizing =
            true;

        texto.fontSizeMin =
            12f;

        texto.fontSizeMax =
            17f;

        texto.overflowMode =
            TextOverflowModes.Ellipsis;
    }

    private void CrearMeta(
        string tamano)
    {
        string contenido =
            string.IsNullOrWhiteSpace(
                tamano
            )
                ? "Mueble"
                : tamano +
                    " casillas";

        TMP_Text texto =
            CrearTexto(
                transform,
                contenido,
                12.5f,
                FontStyles.Normal,
                TextAlignmentOptions.Left
            );

        texto.color =
            GameUITheme.TextoSecundario;

        RectTransform r =
            texto.rectTransform;

        r.anchorMin =
            new Vector2(0f, 0f);

        r.anchorMax =
            new Vector2(1f, 0f);

        r.pivot =
            new Vector2(0.5f, 0f);

        r.anchoredPosition =
            new Vector2(0f, 53f);

        r.sizeDelta =
            new Vector2(-28f, 22f);
    }

    private void CrearBoton()
    {
        GameObject objeto =
            CrearObjeto(
                "BotonColocar",
                transform
            );

        RectTransform r =
            objeto.GetComponent<RectTransform>();

        r.anchorMin =
            new Vector2(0f, 0f);

        r.anchorMax =
            new Vector2(1f, 0f);

        r.pivot =
            new Vector2(0.5f, 0f);

        r.anchoredPosition =
            new Vector2(0f, 12f);

        r.sizeDelta =
            new Vector2(-24f, 36f);

        Image bg =
            objeto.AddComponent<Image>();

        bg.sprite =
            UIRoundedSpriteFactory.Obtener(
                GameUITheme.RadioBoton
            );

        bg.type =
            Image.Type.Sliced;

        bg.color =
            GameUITheme.Esmeralda;

        botonColocar =
            objeto.AddComponent<Button>();

        botonColocar.targetGraphic =
            bg;

        ColorBlock colores =
            botonColocar.colors;

        colores.normalColor =
            Color.white;

        colores.highlightedColor =
            new Color(
                0.90f,
                0.90f,
                0.90f,
                1f
            );

        colores.pressedColor =
            new Color(
                0.76f,
                0.76f,
                0.76f,
                1f
            );

        colores.selectedColor =
            Color.white;

        botonColocar.colors =
            colores;

        botonColocar.onClick.AddListener(
            () =>
            {
                accionColocar?.Invoke();
            }
        );

        GameObject iconoObjeto =
            CrearObjeto(
                "IconoColocar",
                objeto.transform
            );

        RectTransform rIcono =
            iconoObjeto.GetComponent<
                RectTransform>();

        rIcono.anchorMin =
            new Vector2(0f, 0.5f);

        rIcono.anchorMax =
            new Vector2(0f, 0.5f);

        rIcono.pivot =
            new Vector2(0f, 0.5f);

        rIcono.anchoredPosition =
            new Vector2(45f, 0f);

        rIcono.sizeDelta =
            new Vector2(17f, 17f);

        Image icono =
            iconoObjeto.AddComponent<Image>();

        icono.sprite =
            GameUIIconFactory.Obtener(
                GameUIIconFactory.Tipo.Colocar
            );

        icono.color =
            Color.white;

        icono.raycastTarget =
            false;

        TMP_Text texto =
            CrearTexto(
                objeto.transform,
                "COLOCAR",
                12.5f,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        texto.color =
            Color.white;

        texto.rectTransform.offsetMin =
            new Vector2(18f, 0f);
    }

    public void OnPointerEnter(
        PointerEventData eventData)
    {
        hover =
            true;

        if (fondo != null)
            fondo.color = colorHover;

        if (borde != null)
        {
            borde.effectColor =
                new Color32(
                    34,
                    197,
                    94,
                    110
                );

            borde.effectDistance =
                new Vector2(2f, -2f);
        }

        escalaObjetivo =
            new Vector3(
                1.022f,
                1.022f,
                1f
            );

        preview3D?.ActivarHover();
    }

    public void OnPointerExit(
        PointerEventData eventData)
    {
        hover =
            false;

        if (fondo != null)
            fondo.color = colorNormal;

        if (borde != null)
        {
            borde.effectColor =
                GameUITheme.BordeSuave;

            borde.effectDistance =
                new Vector2(1f, -1f);
        }

        escalaObjetivo =
            Vector3.one;

        preview3D?.DesactivarHover();
    }

    public void OnPointerDown(
        PointerEventData eventData)
    {
        escalaObjetivo =
            new Vector3(
                0.985f,
                0.985f,
                1f
            );
    }

    public void OnPointerUp(
        PointerEventData eventData)
    {
        escalaObjetivo =
            hover
                ? new Vector3(
                    1.022f,
                    1.022f,
                    1f
                )
                : Vector3.one;
    }

    private string NombreCategoria(
        string categoria)
    {
        if (
            string.IsNullOrWhiteSpace(
                categoria
            )
        )
        {
            return "MUEBLE";
        }

        return categoria
            .Trim()
            .ToUpperInvariant();
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
