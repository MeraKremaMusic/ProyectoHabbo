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

    private Button botonColocarSeleccionado;
    private Image fondoBotonColocar;
    private TMP_Text textoSeleccionado;

    private string productIdSeleccionado =
        null;

    private readonly Dictionary<
        string,
        FurnitureInventoryCardUI>
        tarjetasPorProducto =
            new Dictionary<
                string,
                FurnitureInventoryCardUI>(
                    StringComparer.OrdinalIgnoreCase
                );

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
                "PanelInventarioReferenciaA",
                canvas.transform
            );

        panelRect =
            panel.GetComponent<RectTransform>();

        panelRect.anchorMin =
            new Vector2(0.5f, 0.5f);

        panelRect.anchorMax =
            new Vector2(0.5f, 0.5f);

        panelRect.pivot =
            new Vector2(0.5f, 0.5f);

        panelRect.anchoredPosition =
            Vector2.zero;

        panelRect.sizeDelta =
            new Vector2(
                850f,
                900f
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
            new Vector2(-9f, -13f);

        rSombraPanel.offsetMax =
            new Vector2(9f, 3f);

        Image imagenSombraPanel =
            sombraPanel.AddComponent<Image>();

        imagenSombraPanel.sprite =
            UIRoundedSpriteFactory.Obtener(24f);

        imagenSombraPanel.type =
            Image.Type.Sliced;

        imagenSombraPanel.color =
            new Color32(0, 0, 0, 120);

        imagenSombraPanel.raycastTarget =
            false;

        Image fondo =
            panel.AddComponent<Image>();

        fondo.sprite =
            UIRoundedSpriteFactory.Obtener(24f);

        fondo.type =
            Image.Type.Sliced;

        fondo.color =
            new Color32(
                18,
                21,
                26,
                250
            );

        Outline borde =
            panel.AddComponent<Outline>();

        borde.effectColor =
            new Color32(
                255,
                255,
                255,
                24
            );

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
        GameObject zonaArrastre =
            CrearObjetoUI(
                "ZonaArrastre",
                panel.transform
            );

        zonaArrastre.transform.SetAsFirstSibling();

        RectTransform rArrastre =
            zonaArrastre.GetComponent<RectTransform>();

        rArrastre.anchorMin =
            new Vector2(0f, 1f);

        rArrastre.anchorMax =
            new Vector2(1f, 1f);

        rArrastre.pivot =
            new Vector2(0.5f, 1f);

        rArrastre.anchoredPosition =
            Vector2.zero;

        rArrastre.sizeDelta =
            new Vector2(0f, 82f);

        Image imagenArrastre =
            zonaArrastre.AddComponent<Image>();

        imagenArrastre.color =
            new Color32(
                255,
                255,
                255,
                1
            );

        UIDraggableWindow arrastre =
            zonaArrastre.AddComponent<
                UIDraggableWindow>();

        arrastre.Configurar(
            panelRect,
            canvas.transform as RectTransform
        );

        GameObject iconoTitulo =
            CrearObjetoUI(
                "IconoInventario",
                panel.transform
            );

        RectTransform rIconoTitulo =
            iconoTitulo.GetComponent<RectTransform>();

        rIconoTitulo.anchorMin =
            new Vector2(0f, 1f);

        rIconoTitulo.anchorMax =
            new Vector2(0f, 1f);

        rIconoTitulo.pivot =
            new Vector2(0f, 1f);

        rIconoTitulo.anchoredPosition =
            new Vector2(
                30f,
                -24f
            );

        rIconoTitulo.sizeDelta =
            new Vector2(
                38f,
                38f
            );

        Image imagenTitulo =
            iconoTitulo.AddComponent<Image>();

        imagenTitulo.sprite =
            InventoryUIIconFactory.Obtener(
                InventoryUIIconFactory.Tipo.Inventario
            );

        imagenTitulo.color =
            GameUITheme.TextoPrincipal;

        imagenTitulo.raycastTarget =
            false;

        TMP_Text titulo =
            CrearTexto(
                panel.transform,
                "Inventario",
                29f,
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
                82f,
                -22f
            );

        rTitulo.sizeDelta =
            new Vector2(
                360f,
                44f
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
                -20f
            );

        rCerrar.sizeDelta =
            new Vector2(
                48f,
                48f
            );

        Image bg =
            cerrar.AddComponent<Image>();

        bg.sprite =
            UIRoundedSpriteFactory.Obtener(14f);

        bg.type =
            Image.Type.Sliced;

        bg.color =
            new Color32(
                255,
                255,
                255,
                14
            );

        Outline bordeCerrar =
            cerrar.AddComponent<Outline>();

        bordeCerrar.effectColor =
            new Color32(
                255,
                255,
                255,
                20
            );

        bordeCerrar.effectDistance =
            new Vector2(1f, -1f);

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
            new Vector2(21f, 21f);

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
            new Vector2(0f, 1f);

        r.anchorMax =
            new Vector2(1f, 1f);

        r.pivot =
            new Vector2(0.5f, 1f);

        r.anchoredPosition =
            new Vector2(
                0f,
                -92f
            );

        r.sizeDelta =
            new Vector2(
                -58f,
                54f
            );

        Image bg =
            caja.AddComponent<Image>();

        bg.sprite =
            UIRoundedSpriteFactory.Obtener(15f);

        bg.type =
            Image.Type.Sliced;

        bg.color =
            new Color32(
                12,
                15,
                19,
                245
            );

        Outline borde =
            caja.AddComponent<Outline>();

        borde.effectColor =
            new Color32(
                255,
                255,
                255,
                22
            );

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
            new Vector2(18f, 0f);

        rIconoBuscar.sizeDelta =
            new Vector2(22f, 22f);

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
            "Buscar muebles...";

        placeholder.fontSize =
            16f;

        placeholder.color =
            new Color32(
                125,
                132,
                143,
                255
            );

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
            new Vector2(52f, 5f);

        rPlaceholder.offsetMax =
            new Vector2(-18f, -5f);

        GameObject textoObjeto =
            CrearObjetoUI(
                "TextoEntrada",
                caja.transform
            );

        TextMeshProUGUI texto =
            textoObjeto
                .AddComponent<TextMeshProUGUI>();

        texto.fontSize =
            16f;

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
            new Vector2(52f, 5f);

        rTexto.offsetMax =
            new Vector2(-18f, -5f);

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
                -164f
            );

        r.sizeDelta =
            new Vector2(
                -58f,
                78f
            );

        Image fondoCategorias =
            objeto.AddComponent<Image>();

        fondoCategorias.sprite =
            UIRoundedSpriteFactory.Obtener(16f);

        fondoCategorias.type =
            Image.Type.Sliced;

        fondoCategorias.color =
            new Color32(
                12,
                15,
                19,
                238
            );

        Outline bordeCategorias =
            objeto.AddComponent<Outline>();

        bordeCategorias.effectColor =
            new Color32(
                255,
                255,
                255,
                18
            );

        bordeCategorias.effectDistance =
            new Vector2(1f, -1f);

        contenedorCategorias =
            objeto.transform;

        HorizontalLayoutGroup layout =
            objeto.AddComponent<
                HorizontalLayoutGroup>();

        layout.spacing =
            0f;

        layout.padding =
            new RectOffset(
                0,
                0,
                0,
                0
            );

        layout.childAlignment =
            TextAnchor.MiddleCenter;

        layout.childControlWidth =
            true;

        layout.childControlHeight =
            true;

        layout.childForceExpandWidth =
            true;

        layout.childForceExpandHeight =
            true;
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

        string[] categorias =
        {
            "Todos",
            "Sillas",
            "Mesas",
            "Luces",
            "Decoración"
        };

        foreach (
            string categoria
            in categorias
        )
        {
            CrearBotonCategoria(
                categoria
            );
        }

        if (
            !botonesCategorias.ContainsKey(
                categoriaActual
            )
        )
        {
            categoriaActual =
                "Todos";
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

        Image bg =
            objeto.AddComponent<Image>();

        bg.sprite =
            UIRoundedSpriteFactory.Obtener(13f);

        bg.type =
            Image.Type.Sliced;

        bg.color =
            new Color32(
                255,
                255,
                255,
                0
            );

        Button boton =
            objeto.AddComponent<Button>();

        boton.targetGraphic =
            bg;

        LayoutElement layout =
            objeto.AddComponent<LayoutElement>();

        layout.flexibleWidth =
            1f;

        layout.preferredHeight =
            78f;

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

        GameObject iconoObjeto =
            CrearObjetoUI(
                "Icono",
                objeto.transform
            );

        RectTransform rIcono =
            iconoObjeto.GetComponent<RectTransform>();

        rIcono.anchorMin =
            new Vector2(0.5f, 1f);

        rIcono.anchorMax =
            new Vector2(0.5f, 1f);

        rIcono.pivot =
            new Vector2(0.5f, 1f);

        rIcono.anchoredPosition =
            new Vector2(0f, -13f);

        rIcono.sizeDelta =
            new Vector2(29f, 29f);

        Image imagenIcono =
            iconoObjeto.AddComponent<Image>();

        imagenIcono.sprite =
            InventoryUIIconFactory.Obtener(
                ObtenerIconoCategoria(
                    categoria
                )
            );

        imagenIcono.color =
            GameUITheme.TextoSecundario;

        imagenIcono.raycastTarget =
            false;

        TMP_Text texto =
            CrearTexto(
                objeto.transform,
                categoria,
                13f,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        RectTransform rTexto =
            texto.rectTransform;

        rTexto.anchorMin =
            new Vector2(0f, 0f);

        rTexto.anchorMax =
            new Vector2(1f, 0f);

        rTexto.pivot =
            new Vector2(0.5f, 0f);

        rTexto.anchoredPosition =
            new Vector2(0f, 9f);

        rTexto.sizeDelta =
            new Vector2(-8f, 25f);

        texto.color =
            GameUITheme.TextoSecundario;

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

            bool activo =
                string.Equals(
                    par.Key,
                    categoriaActual,
                    StringComparison
                        .OrdinalIgnoreCase
                );

            if (bg != null)
            {
                bg.color =
                    activo
                        ? new Color32(
                            8,
                            87,
                            60,
                            255
                        )
                        : new Color32(
                            255,
                            255,
                            255,
                            0
                        );
            }

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

            Image[] imagenes =
                par.Value
                    .GetComponentsInChildren<
                        Image>();

            foreach (
                Image imagen
                in imagenes
            )
            {
                if (
                    imagen == null ||
                    imagen == bg
                )
                {
                    continue;
                }

                imagen.color =
                    activo
                        ? new Color32(
                            73,
                            235,
                            171,
                            255
                        )
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
            new Vector2(
                0f,
                -258f
            );

        r.sizeDelta =
            new Vector2(
                -58f,
                1f
            );

        Image linea =
            objeto.AddComponent<Image>();

        linea.color =
            new Color32(
                255,
                255,
                255,
                14
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
                29f,
                92f
            );

        rScroll.offsetMax =
            new Vector2(
                -29f,
                -274f
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
            new Vector2(0f, 1f);

        rContenido.anchorMax =
            new Vector2(1f, 1f);

        rContenido.pivot =
            new Vector2(0.5f, 1f);

        rContenido.anchoredPosition =
            Vector2.zero;

        rContenido.sizeDelta =
            Vector2.zero;

        GridLayoutGroup grid =
            contenidoObjeto.AddComponent<
                GridLayoutGroup>();

        grid.cellSize =
            new Vector2(
                245f,
                260f
            );

        grid.spacing =
            new Vector2(
                18f,
                18f
            );

        grid.padding =
            new RectOffset(
                4,
                4,
                5,
                5
            );

        grid.constraint =
            GridLayoutGroup.Constraint
                .FixedColumnCount;

        grid.constraintCount =
            3;

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
            30f;
    }

    // =====================================================
    // PIE
    // =====================================================


    private void CrearPie()
    {
        GameObject lineaObjeto =
            CrearObjetoUI(
                "SeparadorPie",
                panel.transform
            );

        RectTransform rLinea =
            lineaObjeto.GetComponent<RectTransform>();

        rLinea.anchorMin =
            new Vector2(0f, 0f);

        rLinea.anchorMax =
            new Vector2(1f, 0f);

        rLinea.pivot =
            new Vector2(0.5f, 0f);

        rLinea.anchoredPosition =
            new Vector2(0f, 78f);

        rLinea.sizeDelta =
            new Vector2(-58f, 1f);

        Image linea =
            lineaObjeto.AddComponent<Image>();

        linea.color =
            new Color32(
                255,
                255,
                255,
                17
            );

        linea.raycastTarget =
            false;

        GameObject iconoInfoObjeto =
            CrearObjetoUI(
                "IconoInfo",
                panel.transform
            );

        RectTransform rInfo =
            iconoInfoObjeto.GetComponent<RectTransform>();

        rInfo.anchorMin =
            new Vector2(0f, 0f);

        rInfo.anchorMax =
            new Vector2(0f, 0f);

        rInfo.pivot =
            new Vector2(0f, 0f);

        rInfo.anchoredPosition =
            new Vector2(
                32f,
                26f
            );

        rInfo.sizeDelta =
            new Vector2(
                24f,
                24f
            );

        Image iconoInfo =
            iconoInfoObjeto.AddComponent<Image>();

        iconoInfo.sprite =
            InventoryUIIconFactory.Obtener(
                InventoryUIIconFactory.Tipo.Info
            );

        iconoInfo.color =
            GameUITheme.TextoSecundario;

        iconoInfo.raycastTarget =
            false;

        textoEstado =
            CrearTexto(
                panel.transform,
                "Selecciona un mueble para colocarlo en la sala.",
                13f,
                FontStyles.Normal,
                TextAlignmentOptions.Left
            );

        textoEstado.color =
            GameUITheme.TextoSecundario;

        RectTransform rEstado =
            textoEstado.rectTransform;

        rEstado.anchorMin =
            new Vector2(0f, 0f);

        rEstado.anchorMax =
            new Vector2(0f, 0f);

        rEstado.pivot =
            new Vector2(0f, 0f);

        rEstado.anchoredPosition =
            new Vector2(
                66f,
                24f
            );

        rEstado.sizeDelta =
            new Vector2(
                490f,
                28f
            );

        GameObject botonObjeto =
            CrearObjetoUI(
                "BotonColocarSeleccionado",
                panel.transform
            );

        RectTransform rBoton =
            botonObjeto.GetComponent<RectTransform>();

        rBoton.anchorMin =
            new Vector2(1f, 0f);

        rBoton.anchorMax =
            new Vector2(1f, 0f);

        rBoton.pivot =
            new Vector2(1f, 0f);

        rBoton.anchoredPosition =
            new Vector2(
                -30f,
                18f
            );

        rBoton.sizeDelta =
            new Vector2(
                200f,
                48f
            );

        fondoBotonColocar =
            botonObjeto.AddComponent<Image>();

        fondoBotonColocar.sprite =
            UIRoundedSpriteFactory.Obtener(13f);

        fondoBotonColocar.type =
            Image.Type.Sliced;

        botonColocarSeleccionado =
            botonObjeto.AddComponent<Button>();

        botonColocarSeleccionado.targetGraphic =
            fondoBotonColocar;

        ConfigurarColoresBoton(
            botonColocarSeleccionado
        );

        botonColocarSeleccionado.onClick.AddListener(
            ColocarSeleccionado
        );

        GameObject iconoObjeto =
            CrearObjetoUI(
                "IconoColocar",
                botonObjeto.transform
            );

        RectTransform rIcono =
            iconoObjeto.GetComponent<RectTransform>();

        rIcono.anchorMin =
            new Vector2(0f, 0.5f);

        rIcono.anchorMax =
            new Vector2(0f, 0.5f);

        rIcono.pivot =
            new Vector2(0f, 0.5f);

        rIcono.anchoredPosition =
            new Vector2(37f, 0f);

        rIcono.sizeDelta =
            new Vector2(19f, 19f);

        Image icono =
            iconoObjeto.AddComponent<Image>();

        icono.sprite =
            GameUIIconFactory.Obtener(
                GameUIIconFactory.Tipo.Colocar
            );

        icono.raycastTarget =
            false;

        TMP_Text textoBoton =
            CrearTexto(
                botonObjeto.transform,
                "Colocar",
                15f,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );

        textoBoton.rectTransform.offsetMin =
            new Vector2(24f, 0f);

        ActualizarEstadoSeleccion();
    }

    // =====================================================
    // INVENTARIO
    // =====================================================

    private void ActualizarInventario()
    {
        gruposActuales.Clear();

        if (
            productIdSeleccionado != null &&
            !ProductoSigueDisponible(
                productIdSeleccionado
            )
        )
        {
            LimpiarSeleccion();
        }

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
                "Selecciona un mueble para colocarlo en la sala."
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
                ResolverCategoriaVisual(
                    item,
                    nombre
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

        tarjetasPorProducto.Clear();

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
                    SeleccionarTarjeta(
                        productIdGuardado
                    );
                }
            );

            tarjetasPorProducto[
                grupo.ProductId
            ] = tarjetaUI;

            tarjetaUI.EstablecerSeleccionada(
                string.Equals(
                    productIdSeleccionado,
                    grupo.ProductId,
                    StringComparison
                        .OrdinalIgnoreCase
                )
            );
        }

        ActualizarEstadoSeleccion();

        if (
            gruposActuales.Count > 0 &&
            visibles == 0
        )
        {
            MostrarEstado(
                "No encontramos muebles con ese filtro."
            );
        }
        else if (
            visibles > 0 &&
            string.IsNullOrWhiteSpace(
                productIdSeleccionado
            )
        )
        {
            MostrarEstado(
                "Selecciona un mueble para colocarlo en la sala."
            );
        }
    }

    private void SeleccionarTarjeta(
        string productId)
    {
        if (string.IsNullOrWhiteSpace(productId))
            return;

        productIdSeleccionado =
            productId;

        foreach (
            KeyValuePair<
                string,
                FurnitureInventoryCardUI>
            par
            in tarjetasPorProducto
        )
        {
            if (par.Value == null)
                continue;

            par.Value.EstablecerSeleccionada(
                string.Equals(
                    par.Key,
                    productIdSeleccionado,
                    StringComparison
                        .OrdinalIgnoreCase
                )
            );
        }

        ActualizarEstadoSeleccion();
    }

    private void LimpiarSeleccion()
    {
        productIdSeleccionado =
            null;

        foreach (
            FurnitureInventoryCardUI tarjeta
            in tarjetasPorProducto.Values
        )
        {
            if (tarjeta != null)
            {
                tarjeta.EstablecerSeleccionada(
                    false
                );
            }
        }

        ActualizarEstadoSeleccion();
    }


    private void ActualizarEstadoSeleccion()
    {
        GrupoInventario seleccionado =
            ObtenerGrupo(
                productIdSeleccionado
            );

        bool tieneSeleccion =
            seleccionado != null &&
            seleccionado.Items != null &&
            seleccionado.Items.Count > 0;

        if (botonColocarSeleccionado != null)
        {
            botonColocarSeleccionado.interactable =
                tieneSeleccion;
        }

        if (fondoBotonColocar != null)
        {
            fondoBotonColocar.color =
                tieneSeleccion
                    ? new Color32(
                        8,
                        113,
                        75,
                        255
                    )
                    : new Color32(
                        42,
                        48,
                        54,
                        255
                    );
        }

        if (textoSeleccionado != null)
        {
            textoSeleccionado.text =
                tieneSeleccion
                    ? seleccionado.Nombre
                    : "";

            textoSeleccionado.color =
                tieneSeleccion
                    ? GameUITheme.Esmeralda
                    : GameUITheme.TextoPrincipal;
        }

        if (textoEstado != null)
        {
            textoEstado.text =
                tieneSeleccion
                    ? seleccionado.Nombre +
                        " seleccionado. Pulsa Colocar."
                    : "Selecciona un mueble para colocarlo en la sala.";
        }
    }

    private GrupoInventario ObtenerGrupo(
        string productId)
    {
        if (string.IsNullOrWhiteSpace(productId))
            return null;

        foreach (
            GrupoInventario grupo
            in gruposActuales
        )
        {
            if (
                grupo != null &&
                string.Equals(
                    grupo.ProductId,
                    productId,
                    StringComparison
                        .OrdinalIgnoreCase
                )
            )
            {
                return grupo;
            }
        }

        return null;
    }

    private bool ProductoSigueDisponible(
        string productId)
    {
        if (
            string.IsNullOrWhiteSpace(productId) ||
            inventoryService == null ||
            inventoryService.Items == null
        )
        {
            return false;
        }

        foreach (
            FurnitureInventoryItemData item
            in inventoryService.Items
        )
        {
            if (item == null)
                continue;

            if (
                !string.Equals(
                    item.product_id,
                    productId,
                    StringComparison
                        .OrdinalIgnoreCase
                )
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

            return true;
        }

        return false;
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

    private void ColocarSeleccionado()
    {
        if (
            string.IsNullOrWhiteSpace(
                productIdSeleccionado
            )
        )
        {
            MostrarEstado(
                "Primero selecciona un mueble."
            );

            return;
        }

        ColocarProducto(
            productIdSeleccionado
        );
    }

    private void ColocarProducto(
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

            LimpiarSeleccion();
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

        LimpiarSeleccion();
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
            0.16f;

        float tiempo =
            0f;

        Vector3 escalaDesde =
            abrir
                ? new Vector3(
                    0.965f,
                    0.965f,
                    1f
                )
                : panelRect.localScale;

        Vector3 escalaHasta =
            abrir
                ? Vector3.one
                : new Vector3(
                    0.975f,
                    0.975f,
                    1f
                );

        float alphaDesde =
            abrir
                ? 0f
                : panelCanvasGroup.alpha;

        float alphaHasta =
            abrir
                ? 1f
                : 0f;

        panelRect.localScale =
            escalaDesde;

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

            panelRect.localScale =
                Vector3.Lerp(
                    escalaDesde,
                    escalaHasta,
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

        panelRect.localScale =
            escalaHasta;

        panelCanvasGroup.alpha =
            alphaHasta;

        if (!abrir)
        {
            panel.SetActive(false);
            panelRect.localScale =
                Vector3.one;
        }

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


    private InventoryUIIconFactory.Tipo
        ObtenerIconoCategoria(
            string categoria)
    {
        switch (categoria)
        {
            case "Sillas":
                return InventoryUIIconFactory
                    .Tipo.Sillas;

            case "Mesas":
                return InventoryUIIconFactory
                    .Tipo.Mesas;

            case "Luces":
                return InventoryUIIconFactory
                    .Tipo.Luces;

            case "Decoración":
                return InventoryUIIconFactory
                    .Tipo.Decoracion;

            default:
                return InventoryUIIconFactory
                    .Tipo.Todos;
        }
    }

    private string ResolverCategoriaVisual(
        FurnitureInventoryItemData item,
        string nombre)
    {
        string texto =
            (
                (item != null
                    ? item.product_id
                    : "")
                +
                " "
                +
                (item != null
                    ? item.category
                    : "")
                +
                " "
                +
                (nombre ?? "")
            )
            .ToLowerInvariant();

        if (
            texto.Contains("silla") ||
            texto.Contains("chair") ||
            texto.Contains("taburete") ||
            texto.Contains("stool") ||
            texto.Contains("sofa") ||
            texto.Contains("sofá") ||
            texto.Contains("banco")
        )
        {
            return "Sillas";
        }

        if (
            texto.Contains("mesa") ||
            texto.Contains("table") ||
            texto.Contains("escritorio") ||
            texto.Contains("desk")
        )
        {
            return "Mesas";
        }

        if (
            texto.Contains("lampara") ||
            texto.Contains("lámpara") ||
            texto.Contains("lamp") ||
            texto.Contains("luz") ||
            texto.Contains("light")
        )
        {
            return "Luces";
        }

        return "Decoración";
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
