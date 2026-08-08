using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureInventoryAutoUI : MonoBehaviour
{
    [Header("Referencias")]
    public FurnitureCatalog catalogo;
    public FurnitureSpawner spawner;

    private Canvas canvas;
    private GameObject panel;
    private Transform contenido;

    private readonly Color colorPanel =
        new Color32(24, 27, 34, 250);

    private readonly Color colorTarjeta =
        new Color32(47, 52, 63, 255);

    private readonly Color colorTarjetaHover =
        new Color32(65, 72, 87, 255);

    private readonly Color colorBoton =
        new Color32(35, 39, 48, 255);

    private void Start()
    {
        CrearInterfaz();
        CerrarInventario();
    }

    private void CrearInterfaz()
    {
        CrearCanvas();
        CrearBotonMuebles();
        CrearPanel();
        CrearTarjetas();
    }

    private void CrearCanvas()
    {
        GameObject objetoCanvas =
            new GameObject("InventarioUI");

        canvas =
            objetoCanvas.AddComponent<Canvas>();

        canvas.renderMode =
            RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler =
            objetoCanvas.AddComponent<CanvasScaler>();

        scaler.uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;

        scaler.referenceResolution =
            new Vector2(1920f, 1080f);

        scaler.matchWidthOrHeight = 0.5f;

        objetoCanvas.AddComponent<GraphicRaycaster>();
    }

    private void CrearBotonMuebles()
    {
        GameObject objeto =
            CrearObjetoUI(
                "BotonMuebles",
                canvas.transform
            );

        RectTransform rect =
            objeto.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(0f, 0f);
        rect.pivot = new Vector2(0f, 0f);

        rect.anchoredPosition =
            new Vector2(35f, 35f);

        rect.sizeDelta =
            new Vector2(170f, 58f);

        Image imagen =
            objeto.AddComponent<Image>();

        imagen.color = colorBoton;

        Button boton =
            objeto.AddComponent<Button>();

        ConfigurarColoresBoton(
            boton,
            colorBoton
        );

        boton.onClick.AddListener(
            AlternarInventario
        );

        CrearTexto(
            objeto.transform,
            "Muebles",
            21,
            FontStyles.Bold,
            TextAlignmentOptions.Center
        );
    }

    private void CrearPanel()
    {
        panel =
            CrearObjetoUI(
                "PanelInventario",
                canvas.transform
            );

        RectTransform rect =
            panel.GetComponent<RectTransform>();

        rect.anchorMin =
            new Vector2(0.5f, 0f);

        rect.anchorMax =
            new Vector2(0.5f, 0f);

        rect.pivot =
            new Vector2(0.5f, 0f);

        rect.anchoredPosition =
            new Vector2(0f, 120f);

        rect.sizeDelta =
            new Vector2(820f, 300f);

        Image fondo =
            panel.AddComponent<Image>();

        fondo.color = colorPanel;

        CrearCabecera();
        CrearScroll();
    }

    private void CrearCabecera()
    {
        TMP_Text titulo =
            CrearTexto(
                panel.transform,
                "INVENTARIO",
                26,
                FontStyles.Bold,
                TextAlignmentOptions.Left
            );

        RectTransform tituloRect =
            titulo.rectTransform;

        tituloRect.anchorMin =
            new Vector2(0f, 1f);

        tituloRect.anchorMax =
            new Vector2(1f, 1f);

        tituloRect.pivot =
            new Vector2(0f, 1f);

        tituloRect.anchoredPosition =
            new Vector2(25f, -18f);

        tituloRect.sizeDelta =
            new Vector2(-100f, 40f);


        TMP_Text subtitulo =
            CrearTexto(
                panel.transform,
                "Elige un mueble",
                15,
                FontStyles.Normal,
                TextAlignmentOptions.Left
            );

        subtitulo.color =
            new Color32(180, 185, 195, 255);

        RectTransform subRect =
            subtitulo.rectTransform;

        subRect.anchorMin =
            new Vector2(0f, 1f);

        subRect.anchorMax =
            new Vector2(1f, 1f);

        subRect.pivot =
            new Vector2(0f, 1f);

        subRect.anchoredPosition =
            new Vector2(25f, -55f);

        subRect.sizeDelta =
            new Vector2(-100f, 30f);


        GameObject cerrar =
            CrearObjetoUI(
                "Cerrar",
                panel.transform
            );

        RectTransform cerrarRect =
            cerrar.GetComponent<RectTransform>();

        cerrarRect.anchorMin =
            new Vector2(1f, 1f);

        cerrarRect.anchorMax =
            new Vector2(1f, 1f);

        cerrarRect.pivot =
            new Vector2(1f, 1f);

        cerrarRect.anchoredPosition =
            new Vector2(-18f, -18f);

        cerrarRect.sizeDelta =
            new Vector2(45f, 45f);

        Image cerrarImagen =
            cerrar.AddComponent<Image>();

        cerrarImagen.color =
            new Color32(55, 60, 72, 255);

        Button cerrarBoton =
            cerrar.AddComponent<Button>();

        cerrarBoton.onClick.AddListener(
            CerrarInventario
        );

        CrearTexto(
            cerrar.transform,
            "×",
            27,
            FontStyles.Normal,
            TextAlignmentOptions.Center
        );
    }

    private void CrearScroll()
{
    GameObject scroll =
        CrearObjetoUI(
            "Scroll",
            panel.transform
        );

    RectTransform scrollRectTransform =
        scroll.GetComponent<RectTransform>();

    scrollRectTransform.anchorMin =
        new Vector2(0f, 0f);

    scrollRectTransform.anchorMax =
        new Vector2(1f, 1f);

    scrollRectTransform.offsetMin =
        new Vector2(20f, 20f);

    scrollRectTransform.offsetMax =
        new Vector2(-20f, -95f);


    // VIEWPORT
    GameObject viewport =
        CrearObjetoUI(
            "Viewport",
            scroll.transform
        );

    RectTransform viewportRect =
        viewport.GetComponent<RectTransform>();

    viewportRect.anchorMin = Vector2.zero;
    viewportRect.anchorMax = Vector2.one;
    viewportRect.offsetMin = Vector2.zero;
    viewportRect.offsetMax = Vector2.zero;

    // Usamos RectMask2D.
    // No necesita una imagen transparente.
    viewport.AddComponent<RectMask2D>();


    // CONTENIDO
    GameObject contenidoObjeto =
        CrearObjetoUI(
            "Contenido",
            viewport.transform
        );

    contenido =
        contenidoObjeto.transform;

    RectTransform contenidoRect =
        contenidoObjeto.GetComponent<RectTransform>();

    contenidoRect.anchorMin =
        new Vector2(0f, 1f);

    contenidoRect.anchorMax =
        new Vector2(1f, 1f);

    contenidoRect.pivot =
        new Vector2(0.5f, 1f);

    contenidoRect.anchoredPosition =
        Vector2.zero;

    contenidoRect.sizeDelta =
        new Vector2(0f, 120f);


    // CUADRICULA
    GridLayoutGroup grid =
        contenidoObjeto.AddComponent<GridLayoutGroup>();

    grid.cellSize =
        new Vector2(175f, 115f);

    grid.spacing =
        new Vector2(12f, 12f);

    grid.padding =
        new RectOffset(5, 5, 5, 5);

    grid.constraint =
        GridLayoutGroup.Constraint.FixedColumnCount;

    grid.constraintCount = 4;


    // ALTURA AUTOMATICA
    ContentSizeFitter fitter =
        contenidoObjeto.AddComponent<ContentSizeFitter>();

    fitter.verticalFit =
        ContentSizeFitter.FitMode.PreferredSize;


    // SCROLL
    ScrollRect scrollRect =
        scroll.AddComponent<ScrollRect>();

    scrollRect.viewport = viewportRect;
    scrollRect.content = contenidoRect;

    scrollRect.horizontal = false;
    scrollRect.vertical = true;

    scrollRect.movementType =
        ScrollRect.MovementType.Clamped;

    scrollRect.scrollSensitivity = 25f;
}

    private void CrearTarjetas()
    {
        if (
            catalogo == null ||
            catalogo.muebles == null
        )
        {
            return;
        }

        for (
            int i = 0;
            i < catalogo.muebles.Length;
            i++
        )
        {
            GameObject prefab =
                catalogo.muebles[i];

            if (prefab == null)
                continue;

            CrearTarjeta(
                prefab,
                i
            );
        }
    }

    private void CrearTarjeta(
        GameObject prefab,
        int indice)
    {
        GameObject tarjeta =
            CrearObjetoUI(
                "Mueble_" + prefab.name,
                contenido
            );

        Image fondo =
            tarjeta.AddComponent<Image>();

        fondo.color = colorTarjeta;

        Button boton =
            tarjeta.AddComponent<Button>();

        ConfigurarColoresBoton(
            boton,
            colorTarjeta
        );

        int indiceGuardado = indice;

        boton.onClick.AddListener(
            () => SeleccionarMueble(
                indiceGuardado
            )
        );


        FurnitureData datos =
            prefab.GetComponent<FurnitureData>();

        string tamano = "";

        if (datos != null)
        {
            tamano =
                "\n<size=14><color=#AEB5C2>" +
                datos.ancho +
                " × " +
                datos.largo +
                "</color></size>";
        }

        TMP_Text texto =
            CrearTexto(
                tarjeta.transform,
                prefab.name + tamano,
                18,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        texto.richText = true;
    }

    private void SeleccionarMueble(
        int indice)
    {
        if (spawner == null)
            return;

        spawner.CrearMueble(indice);

        CerrarInventario();
    }

    public void AlternarInventario()
    {
        if (panel == null)
            return;

        panel.SetActive(
            !panel.activeSelf
        );
    }

    public void CerrarInventario()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
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

    private TMP_Text CrearTexto(
        Transform padre,
        string contenidoTexto,
        float tamano,
        FontStyles estilo,
        TextAlignmentOptions alineacion)
    {
        GameObject objetoTexto =
            CrearObjetoUI(
                "Texto",
                padre
            );

        TextMeshProUGUI texto =
            objetoTexto.AddComponent<TextMeshProUGUI>();

        texto.text = contenidoTexto;
        texto.fontSize = tamano;
        texto.fontStyle = estilo;
        texto.alignment = alineacion;
        texto.color = Color.white;

        RectTransform rect =
            texto.rectTransform;

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;

        rect.offsetMin =
            new Vector2(8f, 5f);

        rect.offsetMax =
            new Vector2(-8f, -5f);

        return texto;
    }

    private void ConfigurarColoresBoton(
        Button boton,
        Color normal)
    {
        ColorBlock colores =
            boton.colors;

        colores.normalColor = normal;

        colores.highlightedColor =
            colorTarjetaHover;

        colores.pressedColor =
            new Color32(30, 33, 40, 255);

        colores.selectedColor =
            colorTarjetaHover;

        boton.colors = colores;
    }
}