using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FurnitureInventoryAutoUI :
    MonoBehaviour
{
    private sealed class GrupoInventario
    {
        public string ProductId;
        public string Nombre;
        public string Categoria;
        public string Tamano;
        public GameObject Prefab;
        public List<FurnitureInventoryItemData>
            Items =
                new List<FurnitureInventoryItemData>();
    }

    private Canvas canvas;
    private GameObject panel;
    private CanvasGroup panelCanvasGroup;
    private RectTransform panelRect;

    private Transform contenido;
    private TMP_Text textoEstado;
    private TMP_Text textoContador;
    private TMP_InputField buscador;
    private Transform contenedorCategorias;

    private PlayerInventoryService
        inventoryService;

    private FurnitureInventorySpawner
        inventorySpawner;

    private bool conectado;

    private string categoriaActual =
        "Todos";

    private string busquedaActual =
        "";

    private readonly List<GrupoInventario>
        gruposActuales =
            new List<GrupoInventario>();

    private readonly Dictionary<
        string,
        Button>
        botonesCategorias =
            new Dictionary<string, Button>(
                StringComparer.OrdinalIgnoreCase
            );

    private Coroutine animacionPanel;

    private void Start()
    {
        inventorySpawner =
            UnityEngine.Object.FindAnyObjectByType<
                FurnitureInventorySpawner>();

        CrearEventSystemSiHaceFalta();
        CrearInterfaz();
        CerrarInventarioInmediato();
        IntentarConectarInventario();
    }

    private void Update()
    {
        if (!conectado)
            IntentarConectarInventario();

        if (inventorySpawner == null)
        {
            inventorySpawner =
                UnityEngine.Object.FindAnyObjectByType<
                    FurnitureInventorySpawner>();
        }
    }

    // =====================================================
    // CONEXION INVENTARIO
    // =====================================================

    private void IntentarConectarInventario()
    {
        if (conectado)
            return;

        inventoryService =
            PlayerInventoryService.Instance;

        if (inventoryService == null)
            return;

        inventoryService
            .InventarioActualizado +=
            ActualizarInventario;

        conectado =
            true;

        if (
            inventoryService
                .InventarioCargado
        )
        {
            ActualizarInventario();
        }
        else
        {
            MostrarEstado(
                "Cargando inventario..."
            );
        }
    }

    // =====================================================
    // INTERFAZ
    // =====================================================

    private void CrearInterfaz()
    {
        CrearCanvas();
        CrearBotonMuebles();
        CrearPanel();
        ActualizarInventario();
    }

    private void CrearCanvas()
    {
        GameObject objetoCanvas =
            new GameObject(
                "InventarioUI",
                typeof(RectTransform)
            );

        canvas =
            objetoCanvas.AddComponent<Canvas>();

        canvas.renderMode =
            RenderMode.ScreenSpaceOverlay;

        canvas.sortingOrder =
            155;

        CanvasScaler scaler =
            objetoCanvas.AddComponent<
                CanvasScaler>();

        scaler.uiScaleMode =
            CanvasScaler.ScaleMode
                .ScaleWithScreenSize;

        scaler.referenceResolution =
            new Vector2(
                1920f,
                1080f
            );

        scaler.matchWidthOrHeight =
            0.5f;

        objetoCanvas.AddComponent<
            GraphicRaycaster>();
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

        rect.anchorMin =
            new Vector2(0f, 0f);

        rect.anchorMax =
            new Vector2(0f, 0f);

        rect.pivot =
            new Vector2(0f, 0f);

        rect.anchoredPosition =
            new Vector2(
                35f,
                30f
            );

        rect.sizeDelta =
            new Vector2(
                170f,
                58f
            );

        Image imagen =
            objeto.AddComponent<Image>();

        imagen.sprite =
            UIRoundedSpriteFactory.Obtener(
                16f
            );

        imagen.type =
            Image.Type.Sliced;

        imagen.color =
            new Color32(
                10,
                10,
                10,
                245
            );

        Button boton =
            objeto.AddComponent<Button>();

        boton.targetGraphic =
            imagen;

        ConfigurarColoresBoton(
            boton
        );

        boton.onClick.AddListener(
            AlternarInventario
        );

        GameObject iconoMuebles =
            CrearObjetoUI(
                "IconoMuebles",
                objeto.transform
            );

        RectTransform rIconoMuebles =
            iconoMuebles.GetComponent<RectTransform>();

        rIconoMuebles.anchorMin =
            new Vector2(0f, 0.5f);

        rIconoMuebles.anchorMax =
            new Vector2(0f, 0.5f);

        rIconoMuebles.pivot =
            new Vector2(0f, 0.5f);

        rIconoMuebles.anchoredPosition =
            new Vector2(20f, 0f);

        rIconoMuebles.sizeDelta =
            new Vector2(22f, 22f);

        Image imagenMuebles =
            iconoMuebles.AddComponent<Image>();

        imagenMuebles.sprite =
            GameUIIconFactory.Obtener(
                GameUIIconFactory.Tipo.Mueble
            );

        imagenMuebles.color =
            Color.white;

        imagenMuebles.raycastTarget =
            false;

        TMP_Text texto =
            CrearTexto(
                objeto.transform,
                "MUEBLES",
                16f,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        texto.color =
            Color.white;

        texto.rectTransform.offsetMin =
            new Vector2(34f, 0f);
    }

    private void CrearPanel()
    {
        panel =
            CrearObjetoUI(
                "PanelInventarioModerno",
                canvas.transform
            );

        panelRect =
            panel.GetComponent<RectTransform>();

        panelRect.anchorMin =
            new Vector2(
                0.5f,
                0f
            );

        panelRect.anchorMax =
            new Vector2(
                0.5f,
                0f
            );

        panelRect.pivot =
            new Vector2(
                0.5f,
                0f
            );

        panelRect.anchoredPosition =
            new Vector2(
                0f,
                108f
            );

        panelRect.sizeDelta =
            new Vector2(
                1240f,
                525f
            );

        GameObject sombraPanel =
            CrearObjetoUI(
                "SombraPanel",
                panel.transform
            );

        sombraPanel.transform.SetAsFirstSibling();

        RectTransform rSombraPanel =
            sombraPanel.GetComponent<RectTransform>();

        rSombraPanel.anchorMin =
            Vector2.zero;

        rSombraPanel.anchorMax =
            Vector2.one;

        rSombraPanel.offsetMin =
            new Vector2(-8f, -14f);

        rSombraPanel.offsetMax =
            new Vector2(8f, 2f);

        Image imagenSombraPanel =
            sombraPanel.AddComponent<Image>();

        imagenSombraPanel.sprite =
            UIRoundedSpriteFactory.Obtener(
                GameUITheme.RadioPanel
            );

        imagenSombraPanel.type =
            Image.Type.Sliced;

        imagenSombraPanel.color =
            new Color32(0, 0, 0, 92);

        imagenSombraPanel.raycastTarget =
            false;

        Image fondo =
            panel.AddComponent<Image>();

        fondo.sprite =
            UIRoundedSpriteFactory.Obtener(
                GameUITheme.RadioPanel
            );

        fondo.type =
            Image.Type.Sliced;

        fondo.color =
            GameUITheme.FondoPrincipal;

        Outline borde =
            panel.AddComponent<Outline>();

        borde.effectColor =
            GameUITheme.BordeSuave;

        borde.effectDistance =
            new Vector2(1f, -1f);

        panelCanvasGroup =
            panel.AddComponent<CanvasGroup>();

        CrearCabecera();
        CrearBuscador();
        CrearCategorias();
        CrearSeparador();
        CrearScroll();
        CrearPie();
    }

    // =====================================================
    // CABECERA
    // =====================================================

    private void CrearCabecera()
    {
        TMP_Text titulo =
            CrearTexto(
                panel.transform,
                "INVENTARIO",
                30f,
                FontStyles.Bold,
                TextAlignmentOptions.Left
            );

        titulo.color =
            GameUITheme.TextoPrincipal;

        RectTransform rTitulo =
            titulo.rectTransform;

        rTitulo.anchorMin =
            new Vector2(0f, 1f);

        rTitulo.anchorMax =
            new Vector2(0f, 1f);

        rTitulo.pivot =
            new Vector2(0f, 1f);

        rTitulo.anchoredPosition =
            new Vector2(
                30f,
                -22f
            );

        rTitulo.sizeDelta =
            new Vector2(
                380f,
                40f
            );

        TMP_Text subtitulo =
            CrearTexto(
                panel.transform,
                "Elige un mueble y colocalo en tu habitacion",
                14f,
                FontStyles.Normal,
                TextAlignmentOptions.Left
            );

        subtitulo.color =
            GameUITheme.TextoSecundario;

        RectTransform rSub =
            subtitulo.rectTransform;

        rSub.anchorMin =
            new Vector2(0f, 1f);

        rSub.anchorMax =
            new Vector2(0f, 1f);

        rSub.pivot =
            new Vector2(0f, 1f);

        rSub.anchoredPosition =
            new Vector2(
                30f,
                -62f
            );

        rSub.sizeDelta =
            new Vector2(
                520f,
                26f
            );

        GameObject cerrar =
            CrearObjetoUI(
                "CerrarInventario",
                panel.transform
            );

        RectTransform rCerrar =
            cerrar.GetComponent<RectTransform>();

        rCerrar.anchorMin =
            new Vector2(1f, 1f);

        rCerrar.anchorMax =
            new Vector2(1f, 1f);

        rCerrar.pivot =
            new Vector2(1f, 1f);

        rCerrar.anchoredPosition =
            new Vector2(
                -24f,
                -22f
            );

        rCerrar.sizeDelta =
            new Vector2(
                44f,
                44f
            );

        Image bg =
            cerrar.AddComponent<Image>();

        bg.sprite =
            UIRoundedSpriteFactory.Obtener(
                14f
            );

        bg.type =
            Image.Type.Sliced;

        bg.color =
            new Color32(
                255,
                255,
                255,
                16
            );

        Button boton =
            cerrar.AddComponent<Button>();

        boton.targetGraphic =
            bg;

        ConfigurarColoresBoton(
            boton
        );

        boton.onClick.AddListener(
            CerrarInventario
        );

        GameObject iconoCerrar =
            CrearObjetoUI(
                "IconoCerrar",
                cerrar.transform
            );

        RectTransform rIconoCerrar =
            iconoCerrar.GetComponent<RectTransform>();

        rIconoCerrar.anchorMin =
            new Vector2(0.5f, 0.5f);

        rIconoCerrar.anchorMax =
            new Vector2(0.5f, 0.5f);

        rIconoCerrar.pivot =
            new Vector2(0.5f, 0.5f);

        rIconoCerrar.sizeDelta =
            new Vector2(18f, 18f);

        Image imagenCerrar =
            iconoCerrar.AddComponent<Image>();

        imagenCerrar.sprite =
            GameUIIconFactory.Obtener(
                GameUIIconFactory.Tipo.Cerrar
            );

        imagenCerrar.color =
            GameUITheme.TextoPrincipal;

        imagenCerrar.raycastTarget =
            false;
    }

    // =====================================================
    // BUSCADOR
    // =====================================================

    private void CrearBuscador()
    {
        GameObject caja =
            CrearObjetoUI(
                "Buscador",
                panel.transform
            );

        RectTransform r =
            caja.GetComponent<RectTransform>();

        r.anchorMin =
            new Vector2(1f, 1f);

        r.anchorMax =
            new Vector2(1f, 1f);

        r.pivot =
            new Vector2(1f, 1f);

        r.anchoredPosition =
            new Vector2(
                -86f,
                -24f
            );

        r.sizeDelta =
            new Vector2(
                330f,
                46f
            );

        Image bg =
            caja.AddComponent<Image>();

        bg.sprite =
            UIRoundedSpriteFactory.Obtener(
                15f
            );

        bg.type =
            Image.Type.Sliced;

        bg.color =
            GameUITheme.FondoElevado;

        Outline borde =
            caja.AddComponent<Outline>();

        borde.effectColor =
            GameUITheme.BordeSuave;

        borde.effectDistance =
            new Vector2(1f, -1f);

        buscador =
            caja.AddComponent<TMP_InputField>();

        buscador.targetGraphic =
            bg;

        GameObject iconoBuscar =
            CrearObjetoUI(
                "IconoBuscar",
                caja.transform
            );

        RectTransform rIconoBuscar =
            iconoBuscar.GetComponent<RectTransform>();

        rIconoBuscar.anchorMin =
            new Vector2(0f, 0.5f);

        rIconoBuscar.anchorMax =
            new Vector2(0f, 0.5f);

        rIconoBuscar.pivot =
            new Vector2(0f, 0.5f);

        rIconoBuscar.anchoredPosition =
            new Vector2(15f, 0f);

        rIconoBuscar.sizeDelta =
            new Vector2(18f, 18f);

        Image imagenBuscar =
            iconoBuscar.AddComponent<Image>();

        imagenBuscar.sprite =
            GameUIIconFactory.Obtener(
                GameUIIconFactory.Tipo.Buscar
            );

        imagenBuscar.color =
            GameUITheme.TextoSecundario;

        imagenBuscar.raycastTarget =
            false;

        GameObject placeholderObjeto =
            CrearObjetoUI(
                "Placeholder",
                caja.transform
            );

        TextMeshProUGUI placeholder =
            placeholderObjeto
                .AddComponent<TextMeshProUGUI>();

        placeholder.text =
            "Buscar mueble...";

        placeholder.fontSize =
            15f;

        placeholder.color =
            GameUITheme.TextoSecundario;

        placeholder.alignment =
            TextAlignmentOptions.Left;

        placeholder.raycastTarget =
            false;

        RectTransform rPlaceholder =
            placeholder.rectTransform;

        rPlaceholder.anchorMin =
            Vector2.zero;

        rPlaceholder.anchorMax =
            Vector2.one;

        rPlaceholder.offsetMin =
            new Vector2(44f, 4f);

        rPlaceholder.offsetMax =
            new Vector2(-18f, -4f);

        GameObject textoObjeto =
            CrearObjetoUI(
                "TextoEntrada",
                caja.transform
            );

        TextMeshProUGUI texto =
            textoObjeto
                .AddComponent<TextMeshProUGUI>();

        texto.fontSize =
            15f;

        texto.color =
            GameUITheme.TextoPrincipal;

        texto.alignment =
            TextAlignmentOptions.Left;

        texto.raycastTarget =
            false;

        RectTransform rTexto =
            texto.rectTransform;

        rTexto.anchorMin =
            Vector2.zero;

        rTexto.anchorMax =
            Vector2.one;

        rTexto.offsetMin =
            new Vector2(44f, 4f);

        rTexto.offsetMax =
            new Vector2(-18f, -4f);

        buscador.textViewport =
            rTexto;

        buscador.textComponent =
            texto;

        buscador.placeholder =
            placeholder;

        buscador.lineType =
            TMP_InputField.LineType.SingleLine;

        buscador.onValueChanged.AddListener(
            AlCambiarBusqueda
        );
    }

    // =====================================================
    // CATEGORIAS
    // =====================================================

    private void CrearCategorias()
    {
        GameObject objeto =
            CrearObjetoUI(
                "Categorias",
                panel.transform
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
            new Vector2(
                0f,
                -102f
            );

        r.sizeDelta =
            new Vector2(
                -60f,
                42f
            );

        contenedorCategorias =
            objeto.transform;

        HorizontalLayoutGroup layout =
            objeto.AddComponent<
                HorizontalLayoutGroup>();

        layout.spacing =
            10f;

        layout.childAlignment =
            TextAnchor.MiddleLeft;

        layout.childControlWidth =
            false;

        layout.childControlHeight =
            false;

        layout.childForceExpandWidth =
            false;

        layout.childForceExpandHeight =
            false;
    }

    private void ReconstruirCategorias()
    {
        if (contenedorCategorias == null)
            return;

        for (
            int i =
                contenedorCategorias.childCount - 1;
            i >= 0;
            i--
        )
        {
            Destroy(
                contenedorCategorias
                    .GetChild(i)
                    .gameObject
            );
        }

        botonesCategorias.Clear();

        HashSet<string> categorias =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase
            );

        categorias.Add("Todos");

        foreach (
            GrupoInventario grupo
            in gruposActuales
        )
        {
            if (
                !string.IsNullOrWhiteSpace(
                    grupo.Categoria
                )
            )
            {
                categorias.Add(
                    FormatearCategoria(
                        grupo.Categoria
                    )
                );
            }
        }

        foreach (string categoria in categorias)
        {
            CrearBotonCategoria(
                categoria
            );
        }

        ActualizarEstiloCategorias();
    }

    private void CrearBotonCategoria(
        string categoria)
    {
        GameObject objeto =
            CrearObjetoUI(
                "Categoria_" + categoria,
                contenedorCategorias
            );

        RectTransform r =
            objeto.GetComponent<RectTransform>();

        float ancho =
            Mathf.Clamp(
                72f +
                categoria.Length * 6f,
                92f,
                160f
            );

        r.sizeDelta =
            new Vector2(
                ancho,
                36f
            );

        Image bg =
            objeto.AddComponent<Image>();

        bg.sprite =
            UIRoundedSpriteFactory.Obtener(
                14f
            );

        bg.type =
            Image.Type.Sliced;

        Button boton =
            objeto.AddComponent<Button>();

        boton.targetGraphic =
            bg;

        LayoutElement layout =
            objeto.AddComponent<LayoutElement>();

        layout.preferredWidth =
            ancho;

        layout.preferredHeight =
            36f;

        string categoriaGuardada =
            categoria;

        boton.onClick.AddListener(
            () =>
            {
                categoriaActual =
                    categoriaGuardada;

                ActualizarEstiloCategorias();
                RenderizarTarjetas();
            }
        );

        TMP_Text texto =
            CrearTexto(
                objeto.transform,
                categoria,
                13f,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        botonesCategorias[
            categoria
        ] = boton;
    }

    private void ActualizarEstiloCategorias()
    {
        foreach (
            KeyValuePair<string, Button>
            par
            in botonesCategorias
        )
        {
            Image bg =
                par.Value.targetGraphic
                    as Image;

            if (bg == null)
                continue;

            bool activo =
                string.Equals(
                    par.Key,
                    categoriaActual,
                    StringComparison
                        .OrdinalIgnoreCase
                );

            bg.color =
                activo
                    ? GameUITheme.Esmeralda
                    : new Color32(
                        255,
                        255,
                        255,
                        14
                    );

            TMP_Text texto =
                par.Value
                    .GetComponentInChildren<
                        TMP_Text>();

            if (texto != null)
            {
                texto.color =
                    activo
                        ? Color.white
                        : GameUITheme
                            .TextoSecundario;
            }
        }
    }

    private void CrearSeparador()
    {
        GameObject objeto =
            CrearObjetoUI(
                "SeparadorCategorias",
                panel.transform
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
            new Vector2(0f, -149f);

        r.sizeDelta =
            new Vector2(-60f, 1f);

        Image linea =
            objeto.AddComponent<Image>();

        linea.color =
            new Color32(
                255,
                255,
                255,
                18
            );

        linea.raycastTarget =
            false;
    }

    // =====================================================
    // SCROLL
    // =====================================================

    private void CrearScroll()
    {
        GameObject scrollObjeto =
            CrearObjetoUI(
                "ScrollInventario",
                panel.transform
            );

        RectTransform rScroll =
            scrollObjeto
                .GetComponent<RectTransform>();

        rScroll.anchorMin =
            new Vector2(0f, 0f);

        rScroll.anchorMax =
            new Vector2(1f, 1f);

        rScroll.offsetMin =
            new Vector2(
                28f,
                50f
            );

        rScroll.offsetMax =
            new Vector2(
                -28f,
                -156f
            );

        ScrollRect scroll =
            scrollObjeto.AddComponent<
                ScrollRect>();

        GameObject viewport =
            CrearObjetoUI(
                "Viewport",
                scrollObjeto.transform
            );

        RectTransform rViewport =
            viewport
                .GetComponent<RectTransform>();

        rViewport.anchorMin =
            Vector2.zero;

        rViewport.anchorMax =
            Vector2.one;

        rViewport.offsetMin =
            Vector2.zero;

        rViewport.offsetMax =
            Vector2.zero;

        Image viewportImagen =
            viewport.AddComponent<Image>();

        viewportImagen.sprite =
            UIRoundedSpriteFactory.Obtener(
                18f
            );

        viewportImagen.type =
            Image.Type.Sliced;

        viewportImagen.color =
            new Color32(
                255,
                255,
                255,
                1
            );

        Mask mask =
            viewport.AddComponent<Mask>();

        mask.showMaskGraphic =
            false;

        GameObject contenidoObjeto =
            CrearObjetoUI(
                "Contenido",
                viewport.transform
            );

        contenido =
            contenidoObjeto.transform;

        RectTransform rContenido =
            contenidoObjeto
                .GetComponent<RectTransform>();

        rContenido.anchorMin =
            new Vector2(
                0f,
                1f
            );

        rContenido.anchorMax =
            new Vector2(
                1f,
                1f
            );

        rContenido.pivot =
            new Vector2(
                0.5f,
                1f
            );

        rContenido.anchoredPosition =
            Vector2.zero;

        rContenido.sizeDelta =
            Vector2.zero;

        GridLayoutGroup grid =
            contenidoObjeto.AddComponent<
                GridLayoutGroup>();

        grid.cellSize =
            new Vector2(
                220f,
                270f
            );

        grid.spacing =
            new Vector2(
                16f,
                16f
            );

        grid.padding =
            new RectOffset(
                4,
                4,
                4,
                4
            );

        grid.constraint =
            GridLayoutGroup.Constraint
                .FixedColumnCount;

        grid.constraintCount =
            5;

        ContentSizeFitter fitter =
            contenidoObjeto.AddComponent<
                ContentSizeFitter>();

        fitter.verticalFit =
            ContentSizeFitter.FitMode
                .PreferredSize;

        scroll.viewport =
            rViewport;

        scroll.content =
            rContenido;

        scroll.horizontal =
            false;

        scroll.vertical =
            true;

        scroll.movementType =
            ScrollRect.MovementType
                .Clamped;

        scroll.scrollSensitivity =
            28f;
    }

    // =====================================================
    // PIE
    // =====================================================

    private void CrearPie()
    {
        textoContador =
            CrearTexto(
                panel.transform,
                "0 muebles",
                13f,
                FontStyles.Bold,
                TextAlignmentOptions.Left
            );

        textoContador.color =
            GameUITheme.TextoSecundario;

        RectTransform rContador =
            textoContador.rectTransform;

        rContador.anchorMin =
            new Vector2(0f, 0f);

        rContador.anchorMax =
            new Vector2(0f, 0f);

        rContador.pivot =
            new Vector2(0f, 0f);

        rContador.anchoredPosition =
            new Vector2(
                30f,
                16f
            );

        rContador.sizeDelta =
            new Vector2(
                250f,
                24f
            );

        textoEstado =
            CrearTexto(
                panel.transform,
                "",
                13f,
                FontStyles.Normal,
                TextAlignmentOptions.Right
            );

        textoEstado.color =
            GameUITheme.TextoSecundario;

        RectTransform rEstado =
            textoEstado.rectTransform;

        rEstado.anchorMin =
            new Vector2(1f, 0f);

        rEstado.anchorMax =
            new Vector2(1f, 0f);

        rEstado.pivot =
            new Vector2(1f, 0f);

        rEstado.anchoredPosition =
            new Vector2(
                -30f,
                16f
            );

        rEstado.sizeDelta =
            new Vector2(
                420f,
                24f
            );
    }

    // =====================================================
    // INVENTARIO
    // =====================================================

    private void ActualizarInventario()
    {
        gruposActuales.Clear();

        if (inventoryService == null)
        {
            LimpiarTarjetas();
            MostrarEstado(
                "Cargando inventario..."
            );
            ActualizarContador(0);
            return;
        }

        if (
            !inventoryService
                .InventarioCargado
        )
        {
            LimpiarTarjetas();
            MostrarEstado(
                "Cargando inventario..."
            );
            ActualizarContador(0);
            return;
        }

        FurnitureInventoryItemData[]
            items =
                inventoryService.Items;

        if (
            items == null ||
            items.Length == 0
        )
        {
            LimpiarTarjetas();
            MostrarEstado(
                "Tu inventario esta vacio."
            );
            ActualizarContador(0);
            ReconstruirCategorias();
            return;
        }

        Dictionary<
            string,
            GrupoInventario>
            mapa =
                new Dictionary<
                    string,
                    GrupoInventario>();

        int totalDisponible =
            0;

        foreach (
            FurnitureInventoryItemData item
            in items
        )
        {
            if (item == null)
                continue;

            if (item.placed)
                continue;

            if (
                ExisteEnEscena(
                    item.item_id
                )
            )
            {
                continue;
            }

            if (
                string.IsNullOrWhiteSpace(
                    item.product_id
                )
            )
            {
                continue;
            }

            totalDisponible++;

            if (
                !mapa.TryGetValue(
                    item.product_id,
                    out GrupoInventario grupo
                )
            )
            {
                grupo =
                    CrearGrupo(item);

                mapa[
                    item.product_id
                ] = grupo;

                gruposActuales.Add(
                    grupo
                );
            }

            grupo.Items.Add(
                item
            );
        }

        ReconstruirCategorias();
        RenderizarTarjetas();
        ActualizarContador(
            totalDisponible
        );

        if (totalDisponible == 0)
        {
            MostrarEstado(
                "No tienes muebles disponibles."
            );
        }
        else
        {
            MostrarEstado(
                "Listo para decorar"
            );
        }
    }

    private GrupoInventario CrearGrupo(
        FurnitureInventoryItemData item)
    {
        GameObject prefab =
            FurniturePrefabResolver
                .ObtenerPrefab(
                    item.product_id
                );

        string nombre =
            item.name;

        if (
            string.IsNullOrWhiteSpace(
                nombre
            )
        )
        {
            nombre =
                prefab != null
                    ? prefab.name
                    : item.product_id;
        }

        string tamano =
            "";

        if (prefab != null)
        {
            FurnitureData datos =
                prefab.GetComponent<
                    FurnitureData>();

            if (datos != null)
            {
                tamano =
                    datos.ancho +
                    " × " +
                    datos.largo;
            }
        }

        return new GrupoInventario
        {
            ProductId =
                item.product_id,

            Nombre =
                nombre,

            Categoria =
                FormatearCategoria(
                    item.category
                ),

            Tamano =
                tamano,

            Prefab =
                prefab
        };
    }

    private void RenderizarTarjetas()
    {
        LimpiarTarjetas();

        if (contenido == null)
            return;

        int visibles =
            0;

        foreach (
            GrupoInventario grupo
            in gruposActuales
        )
        {
            if (!PasaFiltros(grupo))
                continue;

            visibles++;

            GameObject tarjeta =
                CrearObjetoUI(
                    "Mueble_" +
                    grupo.ProductId,
                    contenido
                );

            FurnitureInventoryCardUI
                tarjetaUI =
                    tarjeta.AddComponent<
                        FurnitureInventoryCardUI>();

            string productIdGuardado =
                grupo.ProductId;

            tarjetaUI.Construir(
                grupo.Prefab,
                grupo.Nombre,
                grupo.Categoria,
                grupo.Tamano,
                grupo.Items.Count,
                () =>
                {
                    SeleccionarProducto(
                        productIdGuardado
                    );
                }
            );
        }

        if (
            gruposActuales.Count > 0 &&
            visibles == 0
        )
        {
            MostrarEstado(
                "No encontramos muebles con ese filtro."
            );
        }
        else if (visibles > 0)
        {
            MostrarEstado(
                visibles +
                (
                    visibles == 1
                        ? " tipo visible"
                        : " tipos visibles"
                )
            );
        }
    }

    private bool PasaFiltros(
        GrupoInventario grupo)
    {
        if (grupo == null)
            return false;

        if (
            !string.Equals(
                categoriaActual,
                "Todos",
                StringComparison.OrdinalIgnoreCase
            )
            &&
            !string.Equals(
                grupo.Categoria,
                categoriaActual,
                StringComparison.OrdinalIgnoreCase
            )
        )
        {
            return false;
        }

        if (
            string.IsNullOrWhiteSpace(
                busquedaActual
            )
        )
        {
            return true;
        }

        string buscar =
            busquedaActual
                .Trim()
                .ToLowerInvariant();

        string nombre =
            (grupo.Nombre ?? "")
                .ToLowerInvariant();

        string categoria =
            (grupo.Categoria ?? "")
                .ToLowerInvariant();

        string producto =
            (grupo.ProductId ?? "")
                .ToLowerInvariant();

        return
            nombre.Contains(buscar) ||
            categoria.Contains(buscar) ||
            producto.Contains(buscar);
    }

    private void AlCambiarBusqueda(
        string valor)
    {
        busquedaActual =
            valor ?? "";

        RenderizarTarjetas();
    }

    // =====================================================
    // COLOCAR MUEBLE
    // =====================================================

    private void SeleccionarProducto(
        string productId)
    {
        if (inventorySpawner == null)
        {
            inventorySpawner =
                UnityEngine.Object.FindAnyObjectByType<
                    FurnitureInventorySpawner>();
        }

        if (inventorySpawner == null)
        {
            MostrarEstado(
                "No se encontro el sistema de colocacion."
            );

            return;
        }

        if (
            inventoryService == null ||
            inventoryService.Items == null
        )
        {
            return;
        }

        foreach (
            FurnitureInventoryItemData item
            in inventoryService.Items
        )
        {
            if (item == null)
                continue;

            if (
                item.product_id !=
                productId
            )
            {
                continue;
            }

            if (item.placed)
                continue;

            if (
                ExisteEnEscena(
                    item.item_id
                )
            )
            {
                continue;
            }

            bool creado =
                inventorySpawner
                    .CrearDesdeInventario(
                        item
                    );

            if (!creado)
                continue;

            ActualizarInventario();
            CerrarInventario();

            return;
        }

        MostrarEstado(
            "No quedan unidades disponibles."
        );

        ActualizarInventario();
    }

    // =====================================================
    // ABRIR / CERRAR
    // =====================================================

    public void AlternarInventario()
    {
        if (panel == null)
            return;

        if (panel.activeSelf)
        {
            CerrarInventario();
        }
        else
        {
            AbrirInventario();
        }
    }

    private void AbrirInventario()
    {
        if (panel == null)
            return;

        ActualizarInventario();

        panel.SetActive(true);

        if (animacionPanel != null)
            StopCoroutine(animacionPanel);

        animacionPanel =
            StartCoroutine(
                AnimarPanel(
                    true
                )
            );
    }

    public void CerrarInventario()
    {
        if (
            panel == null ||
            !panel.activeSelf
        )
        {
            return;
        }

        if (animacionPanel != null)
            StopCoroutine(animacionPanel);

        animacionPanel =
            StartCoroutine(
                AnimarPanel(
                    false
                )
            );
    }

    private void CerrarInventarioInmediato()
    {
        if (panel == null)
            return;

        panel.SetActive(false);
    }

    private IEnumerator AnimarPanel(
        bool abrir)
    {
        float duracion =
            0.18f;

        float tiempo =
            0f;

        Vector2 posicionAbierta =
            new Vector2(
                0f,
                108f
            );

        Vector2 posicionCerrada =
            new Vector2(
                0f,
                -470f
            );

        Vector2 desde =
            abrir
                ? posicionCerrada
                : panelRect
                    .anchoredPosition;

        Vector2 hasta =
            abrir
                ? posicionAbierta
                : posicionCerrada;

        float alphaDesde =
            abrir
                ? 0f
                : panelCanvasGroup.alpha;

        float alphaHasta =
            abrir
                ? 1f
                : 0f;

        panelRect.anchoredPosition =
            desde;

        panelCanvasGroup.alpha =
            alphaDesde;

        while (tiempo < duracion)
        {
            tiempo +=
                Time.unscaledDeltaTime;

            float t =
                Mathf.Clamp01(
                    tiempo /
                    duracion
                );

            float suave =
                1f -
                Mathf.Pow(
                    1f - t,
                    3f
                );

            panelRect.anchoredPosition =
                Vector2.Lerp(
                    desde,
                    hasta,
                    suave
                );

            panelCanvasGroup.alpha =
                Mathf.Lerp(
                    alphaDesde,
                    alphaHasta,
                    suave
                );

            yield return null;
        }

        panelRect.anchoredPosition =
            hasta;

        panelCanvasGroup.alpha =
            alphaHasta;

        if (!abrir)
            panel.SetActive(false);

        animacionPanel =
            null;
    }

    // =====================================================
    // UTILIDADES
    // =====================================================

    private bool ExisteEnEscena(
        string itemId)
    {
        if (
            string.IsNullOrWhiteSpace(
                itemId
            )
        )
        {
            return false;
        }

        FurnitureInventoryInstance[]
            instancias =
                UnityEngine.Object.FindObjectsByType<
                    FurnitureInventoryInstance>(
                    FindObjectsSortMode.None
                );

        foreach (
            FurnitureInventoryInstance
                instancia
            in instancias
        )
        {
            if (
                instancia != null &&
                instancia.ItemId ==
                itemId
            )
            {
                return true;
            }
        }

        return false;
    }

    private void LimpiarTarjetas()
    {
        if (contenido == null)
            return;

        for (
            int i =
                contenido.childCount - 1;
            i >= 0;
            i--
        )
        {
            Destroy(
                contenido
                    .GetChild(i)
                    .gameObject
            );
        }
    }

    private void ActualizarContador(
        int total)
    {
        if (textoContador == null)
            return;

        textoContador.text =
            total +
            (
                total == 1
                    ? " mueble disponible"
                    : " muebles disponibles"
            );
    }

    private void MostrarEstado(
        string mensaje)
    {
        if (textoEstado == null)
            return;

        textoEstado.text =
            mensaje;
    }

    private string FormatearCategoria(
        string categoria)
    {
        if (string.IsNullOrWhiteSpace(categoria))
            return "Otros";

        string limpia =
            categoria.Trim();

        if (limpia.Length == 1)
            return limpia.ToUpperInvariant();

        return
            char.ToUpperInvariant(
                limpia[0]
            )
            +
            limpia.Substring(1)
                .ToLowerInvariant();
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
            objetoTexto
                .AddComponent<
                    TextMeshProUGUI>();

        texto.text =
            contenidoTexto;

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

        RectTransform rect =
            texto.rectTransform;

        rect.anchorMin =
            Vector2.zero;

        rect.anchorMax =
            Vector2.one;

        rect.offsetMin =
            new Vector2(
                6f,
                3f
            );

        rect.offsetMax =
            new Vector2(
                -6f,
                -3f
            );

        return texto;
    }

    private void ConfigurarColoresBoton(
        Button boton)
    {
        ColorBlock colores =
            boton.colors;

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
                0.78f,
                0.78f,
                0.78f,
                1f
            );

        colores.selectedColor =
            Color.white;

        colores.disabledColor =
            new Color(
                0.5f,
                0.5f,
                0.5f,
                0.5f
            );

        boton.colors =
            colores;
    }

    private void CrearEventSystemSiHaceFalta()
    {
        EventSystem actual =
            UnityEngine.Object.FindFirstObjectByType<
                EventSystem>();

        if (actual != null)
            return;

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

    private void OnDestroy()
    {
        if (
            inventoryService != null &&
            conectado
        )
        {
            inventoryService
                .InventarioActualizado -=
                ActualizarInventario;
        }
    }
}
