using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class FurnitureInventoryCardUI :
    MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    private Image fondo;
    private Button botonColocar;
    private RectTransform rect;
    private Action accionColocar;

    private Color colorNormal;
    private Color colorHover;

    private Vector3 escalaNormal =
        Vector3.one;

    private Vector3 escalaObjetivo =
        Vector3.one;

    public void Construir(
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

        Outline borde =
            gameObject.AddComponent<Outline>();

        borde.effectColor =
            GameUITheme.BordeSuave;

        borde.effectDistance =
            new Vector2(1f, -1f);

        CrearEtiquetaCategoria(
            categoria
        );

        CrearIconoVisual(
            nombre,
            categoria
        );

        CrearNombre(
            nombre
        );

        CrearMeta(
            tamano
        );

        CrearCantidad(
            cantidad
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
                14f
            );
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
            new Vector2(14f, -14f);

        r.sizeDelta =
            new Vector2(110f, 28f);

        Image bg =
            objeto.AddComponent<Image>();

        bg.sprite =
            UIRoundedSpriteFactory.Obtener(12f);

        bg.type =
            Image.Type.Sliced;

        bg.color =
            GameUITheme.EsmeraldaSuave;

        TMP_Text texto =
            CrearTexto(
                objeto.transform,
                NombreCategoria(categoria),
                12f,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        texto.color =
            GameUITheme.Esmeralda;
    }

    private void CrearIconoVisual(
        string nombre,
        string categoria)
    {
        GameObject objeto =
            CrearObjeto(
                "Preview",
                transform
            );

        RectTransform r =
            objeto.GetComponent<RectTransform>();

        r.anchorMin =
            new Vector2(0.5f, 1f);

        r.anchorMax =
            new Vector2(0.5f, 1f);

        r.pivot =
            new Vector2(0.5f, 1f);

        r.anchoredPosition =
            new Vector2(0f, -52f);

        r.sizeDelta =
            new Vector2(112f, 78f);

        Image bg =
            objeto.AddComponent<Image>();

        bg.sprite =
            UIRoundedSpriteFactory.Obtener(18f);

        bg.type =
            Image.Type.Sliced;

        bg.color =
            new Color32(
                18,
                21,
                26,
                255
            );

        TMP_Text icono =
            CrearTexto(
                objeto.transform,
                ObtenerSiglas(nombre),
                30f,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        icono.color =
            new Color32(
                210,
                215,
                224,
                255
            );
    }

    private void CrearNombre(
        string nombre)
    {
        TMP_Text texto =
            CrearTexto(
                transform,
                nombre,
                18f,
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
            new Vector2(0f, 74f);

        r.sizeDelta =
            new Vector2(-28f, 30f);

        texto.enableAutoSizing =
            true;

        texto.fontSizeMin =
            13f;

        texto.fontSizeMax =
            18f;

        texto.overflowMode =
            TextOverflowModes.Ellipsis;
    }

    private void CrearMeta(
        string tamano)
    {
        string contenido =
            string.IsNullOrWhiteSpace(tamano)
                ? "Mueble"
                : tamano + " casillas";

        TMP_Text texto =
            CrearTexto(
                transform,
                contenido,
                13f,
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
            new Vector2(0f, 51f);

        r.sizeDelta =
            new Vector2(-28f, 24f);
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
            new Vector2(48f, 30f);

        Image bg =
            objeto.AddComponent<Image>();

        bg.sprite =
            UIRoundedSpriteFactory.Obtener(12f);

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
                13f,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        texto.color =
            GameUITheme.TextoPrincipal;
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
            new Vector2(-24f, 38f);

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
                0.92f,
                0.92f,
                0.92f,
                1f
            );

        colores.pressedColor =
            new Color(
                0.80f,
                0.80f,
                0.80f,
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

        TMP_Text texto =
            CrearTexto(
                objeto.transform,
                "COLOCAR",
                13f,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        texto.color =
            Color.white;
    }

    public void OnPointerEnter(
        PointerEventData eventData)
    {
        if (fondo != null)
            fondo.color = colorHover;

        escalaObjetivo =
            new Vector3(
                1.025f,
                1.025f,
                1f
            );
    }

    public void OnPointerExit(
        PointerEventData eventData)
    {
        if (fondo != null)
            fondo.color = colorNormal;

        escalaObjetivo =
            escalaNormal;
    }

    private string NombreCategoria(
        string categoria)
    {
        if (string.IsNullOrWhiteSpace(categoria))
            return "MUEBLE";

        return categoria
            .Trim()
            .ToUpperInvariant();
    }

    private string ObtenerSiglas(
        string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return "MB";

        string[] partes =
            nombre.Trim().Split(' ');

        if (partes.Length >= 2)
        {
            return (
                partes[0][0].ToString() +
                partes[1][0].ToString()
            ).ToUpperInvariant();
        }

        string limpio =
            partes[0];

        if (limpio.Length >= 2)
        {
            return limpio
                .Substring(0, 2)
                .ToUpperInvariant();
        }

        return limpio.ToUpperInvariant();
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
            objeto.AddComponent<TextMeshProUGUI>();

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
