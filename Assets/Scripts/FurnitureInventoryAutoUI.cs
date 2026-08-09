using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureInventoryAutoUI :
    MonoBehaviour
{
    private Canvas canvas;

    private GameObject panel;

    private Transform contenido;

    private TMP_Text textoEstado;


    private PlayerInventoryService
        inventoryService;

    private FurnitureInventorySpawner
        inventorySpawner;


    private bool conectado;


    private readonly Color colorPanel =
        new Color32(
            24,
            27,
            34,
            250
        );


    private readonly Color colorTarjeta =
        new Color32(
            47,
            52,
            63,
            255
        );


    private readonly Color colorTarjetaHover =
        new Color32(
            65,
            72,
            87,
            255
        );


    private readonly Color colorBoton =
        new Color32(
            35,
            39,
            48,
            255
        );


    private readonly Color verde =
        new Color32(
            39,
            174,
            112,
            255
        );


    private readonly Color gris =
        new Color32(
            180,
            185,
            195,
            255
        );


    private void Start()
    {
        inventorySpawner =
            Object.FindAnyObjectByType<
                FurnitureInventorySpawner>();


        CrearInterfaz();

        CerrarInventario();

        IntentarConectarInventario();
    }


    private void Update()
    {
        if (!conectado)
        {
            IntentarConectarInventario();
        }


        if (inventorySpawner == null)
        {
            inventorySpawner =
                Object.FindAnyObjectByType<
                    FurnitureInventorySpawner>();
        }
    }


    // =====================================================
    // INVENTARIO DE NAKAMA
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
    // CREAR UI
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
                "InventarioUI"
            );


        canvas =
            objetoCanvas
                .AddComponent<Canvas>();


        canvas.renderMode =
            RenderMode.ScreenSpaceOverlay;


        canvas.sortingOrder =
            155;


        CanvasScaler scaler =
            objetoCanvas
                .AddComponent<
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


        objetoCanvas
            .AddComponent<
                GraphicRaycaster>();
    }


    // =====================================================
    // BOTON MUEBLES
    // =====================================================

    private void CrearBotonMuebles()
    {
        GameObject objeto =
            CrearObjetoUI(
                "BotonMuebles",
                canvas.transform
            );


        RectTransform rect =
            objeto.GetComponent<
                RectTransform>();


        rect.anchorMin =
            new Vector2(
                0f,
                0f
            );


        rect.anchorMax =
            new Vector2(
                0f,
                0f
            );


        rect.pivot =
            new Vector2(
                0f,
                0f
            );


        rect.anchoredPosition =
            new Vector2(
                35f,
                35f
            );


        rect.sizeDelta =
            new Vector2(
                170f,
                58f
            );


        Image imagen =
            objeto.AddComponent<Image>();


        imagen.color =
            colorBoton;


        Button boton =
            objeto.AddComponent<Button>();


        ConfigurarColoresBoton(
            boton,
            colorBoton
        );


        boton.onClick.AddListener(
            AlternarInventario
        );


        TMP_Text texto =
            CrearTexto(
                objeto.transform,
                "Muebles",
                21f,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );


        texto.raycastTarget =
            false;
    }


    // =====================================================
    // PANEL
    // =====================================================

    private void CrearPanel()
    {
        panel =
            CrearObjetoUI(
                "PanelInventario",
                canvas.transform
            );


        RectTransform rect =
            panel.GetComponent<
                RectTransform>();


        rect.anchorMin =
            new Vector2(
                0.5f,
                0f
            );


        rect.anchorMax =
            new Vector2(
                0.5f,
                0f
            );


        rect.pivot =
            new Vector2(
                0.5f,
                0f
            );


        rect.anchoredPosition =
            new Vector2(
                0f,
                120f
            );


        rect.sizeDelta =
            new Vector2(
                820f,
                330f
            );


        Image fondo =
            panel.AddComponent<Image>();


        fondo.color =
            colorPanel;


        CrearCabecera();

        CrearScroll();

        CrearEstado();
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
                26f,
                FontStyles.Bold,
                TextAlignmentOptions.Left
            );


        RectTransform tituloRect =
            titulo.rectTransform;


        tituloRect.anchorMin =
            new Vector2(
                0f,
                1f
            );


        tituloRect.anchorMax =
            new Vector2(
                1f,
                1f
            );


        tituloRect.pivot =
            new Vector2(
                0f,
                1f
            );


        tituloRect.anchoredPosition =
            new Vector2(
                25f,
                -18f
            );


        tituloRect.sizeDelta =
            new Vector2(
                -100f,
                40f
            );


        TMP_Text subtitulo =
            CrearTexto(
                panel.transform,
                "Tus muebles disponibles",
                15f,
                FontStyles.Normal,
                TextAlignmentOptions.Left
            );


        subtitulo.color =
            gris;


        RectTransform subRect =
            subtitulo.rectTransform;


        subRect.anchorMin =
            new Vector2(
                0f,
                1f
            );


        subRect.anchorMax =
            new Vector2(
                1f,
                1f
            );


        subRect.pivot =
            new Vector2(
                0f,
                1f
            );


        subRect.anchoredPosition =
            new Vector2(
                25f,
                -55f
            );


        subRect.sizeDelta =
            new Vector2(
                -100f,
                30f
            );


        GameObject cerrar =
            CrearObjetoUI(
                "Cerrar",
                panel.transform
            );


        RectTransform cerrarRect =
            cerrar.GetComponent<
                RectTransform>();


        cerrarRect.anchorMin =
            new Vector2(
                1f,
                1f
            );


        cerrarRect.anchorMax =
            new Vector2(
                1f,
                1f
            );


        cerrarRect.pivot =
            new Vector2(
                1f,
                1f
            );


        cerrarRect.anchoredPosition =
            new Vector2(
                -18f,
                -18f
            );


        cerrarRect.sizeDelta =
            new Vector2(
                45f,
                45f
            );


        Image cerrarImagen =
            cerrar.AddComponent<Image>();


        cerrarImagen.color =
            new Color32(
                55,
                60,
                72,
                255
            );


        Button cerrarBoton =
            cerrar.AddComponent<Button>();


        cerrarBoton.onClick.AddListener(
            CerrarInventario
        );


        TMP_Text textoCerrar =
            CrearTexto(
                cerrar.transform,
                "X",
                20f,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );


        textoCerrar.raycastTarget =
            false;
    }


    // =====================================================
    // SCROLL
    // =====================================================

    private void CrearScroll()
    {
        GameObject scroll =
            CrearObjetoUI(
                "Scroll",
                panel.transform
            );


        RectTransform scrollRectTransform =
            scroll.GetComponent<
                RectTransform>();


        scrollRectTransform.anchorMin =
            new Vector2(
                0f,
                0f
            );


        scrollRectTransform.anchorMax =
            new Vector2(
                1f,
                1f
            );


        scrollRectTransform.offsetMin =
            new Vector2(
                20f,
                45f
            );


        scrollRectTransform.offsetMax =
            new Vector2(
                -20f,
                -95f
            );


        GameObject viewport =
            CrearObjetoUI(
                "Viewport",
                scroll.transform
            );


        RectTransform viewportRect =
            viewport.GetComponent<
                RectTransform>();


        viewportRect.anchorMin =
            Vector2.zero;


        viewportRect.anchorMax =
            Vector2.one;


        viewportRect.offsetMin =
            Vector2.zero;


        viewportRect.offsetMax =
            Vector2.zero;


        viewport.AddComponent<
            RectMask2D>();


        GameObject contenidoObjeto =
            CrearObjetoUI(
                "Contenido",
                viewport.transform
            );


        contenido =
            contenidoObjeto.transform;


        RectTransform contenidoRect =
            contenidoObjeto
                .GetComponent<
                    RectTransform>();


        contenidoRect.anchorMin =
            new Vector2(
                0f,
                1f
            );


        contenidoRect.anchorMax =
            new Vector2(
                1f,
                1f
            );


        contenidoRect.pivot =
            new Vector2(
                0.5f,
                1f
            );


        contenidoRect.anchoredPosition =
            Vector2.zero;


        contenidoRect.sizeDelta =
            new Vector2(
                0f,
                120f
            );


        GridLayoutGroup grid =
            contenidoObjeto
                .AddComponent<
                    GridLayoutGroup>();


        grid.cellSize =
            new Vector2(
                180f,
                150f
            );


        grid.spacing =
            new Vector2(
                12f,
                12f
            );


        grid.padding =
            new RectOffset(
                5,
                5,
                5,
                5
            );


        grid.constraint =
            GridLayoutGroup
                .Constraint
                .FixedColumnCount;


        grid.constraintCount =
            4;


        ContentSizeFitter fitter =
            contenidoObjeto
                .AddComponent<
                    ContentSizeFitter>();


        fitter.verticalFit =
            ContentSizeFitter
                .FitMode
                .PreferredSize;


        ScrollRect scrollRect =
            scroll.AddComponent<
                ScrollRect>();


        scrollRect.viewport =
            viewportRect;


        scrollRect.content =
            contenidoRect;


        scrollRect.horizontal =
            false;


        scrollRect.vertical =
            true;


        scrollRect.movementType =
            ScrollRect
                .MovementType
                .Clamped;


        scrollRect.scrollSensitivity =
            25f;
    }


    // =====================================================
    // ESTADO
    // =====================================================

    private void CrearEstado()
    {
        textoEstado =
            CrearTexto(
                panel.transform,
                "",
                14f,
                FontStyles.Normal,
                TextAlignmentOptions.Center
            );


        textoEstado.color =
            gris;


        RectTransform rect =
            textoEstado.rectTransform;


        rect.anchorMin =
            new Vector2(
                0f,
                0f
            );


        rect.anchorMax =
            new Vector2(
                1f,
                0f
            );


        rect.pivot =
            new Vector2(
                0.5f,
                0f
            );


        rect.anchoredPosition =
            new Vector2(
                0f,
                12f
            );


        rect.sizeDelta =
            new Vector2(
                -40f,
                30f
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


    // =====================================================
    // ACTUALIZAR INVENTARIO
    // =====================================================

    private void ActualizarInventario()
    {
        LimpiarTarjetas();


        if (
            inventoryService == null
        )
        {
            MostrarEstado(
                "Cargando inventario..."
            );

            return;
        }


        if (
            !inventoryService
                .InventarioCargado
        )
        {
            MostrarEstado(
                "Cargando inventario..."
            );

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
            MostrarEstado(
                "Tu inventario esta vacio."
            );

            return;
        }


        Dictionary<
            string,
            List<FurnitureInventoryItemData>>
            grupos =
                new Dictionary<
                    string,
                    List<FurnitureInventoryItemData>>();


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


            if (
                !grupos.ContainsKey(
                    item.product_id
                )
            )
            {
                grupos[
                    item.product_id
                ] =
                    new List<
                        FurnitureInventoryItemData>();
            }


            grupos[
                item.product_id
            ].Add(
                item
            );
        }


        if (grupos.Count == 0)
        {
            MostrarEstado(
                "No tienes muebles disponibles."
            );

            return;
        }


        int totalDisponible =
            0;


        foreach (
            KeyValuePair<
                string,
                List<
                    FurnitureInventoryItemData>>
                grupo
            in grupos
        )
        {
            if (
                grupo.Value == null ||
                grupo.Value.Count == 0
            )
            {
                continue;
            }


            totalDisponible +=
                grupo.Value.Count;


            CrearTarjeta(
                grupo.Key,
                grupo.Value
            );
        }


        MostrarEstado(
            totalDisponible +
            " mueble(s) disponible(s)"
        );
    }


    // =====================================================
    // TARJETA
    // =====================================================

    private void CrearTarjeta(
        string productId,
        List<FurnitureInventoryItemData>
            items)
    {
        if (
            items == null ||
            items.Count == 0
        )
        {
            return;
        }


        GameObject prefab =
            FurniturePrefabResolver
                .ObtenerPrefab(
                    productId
                );


        FurnitureInventoryItemData
            ejemplo =
                items[0];


        string nombre =
            ejemplo.name;


        if (
            string.IsNullOrWhiteSpace(
                nombre
            )
        )
        {
            if (prefab != null)
            {
                nombre =
                    prefab.name;
            }
            else
            {
                nombre =
                    productId;
            }
        }


        GameObject tarjeta =
            CrearObjetoUI(
                "Mueble_" +
                productId,
                contenido
            );


        Image fondo =
            tarjeta.AddComponent<Image>();


        fondo.color =
            colorTarjeta;


        FurnitureData datos =
            null;


        if (prefab != null)
        {
            datos =
                prefab.GetComponent<
                    FurnitureData>();
        }


        string tamano =
            "";


        if (datos != null)
        {
            tamano =
                datos.ancho +
                " × " +
                datos.largo;
        }


        TMP_Text nombreTexto =
            CrearTexto(
                tarjeta.transform,
                nombre,
                18f,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );


        RectTransform nombreRect =
            nombreTexto.rectTransform;


        nombreRect.anchorMin =
            new Vector2(
                0f,
                1f
            );


        nombreRect.anchorMax =
            new Vector2(
                1f,
                1f
            );


        nombreRect.pivot =
            new Vector2(
                0.5f,
                1f
            );


        nombreRect.anchoredPosition =
            new Vector2(
                0f,
                -12f
            );


        nombreRect.sizeDelta =
            new Vector2(
                -10f,
                32f
            );


        TMP_Text infoTexto =
            CrearTexto(
                tarjeta.transform,
                (
                    string.IsNullOrWhiteSpace(
                        tamano
                    )
                        ?
                        ""
                        :
                        tamano + "\n"
                )
                +
                "Disponibles: x" +
                items.Count,
                14f,
                FontStyles.Normal,
                TextAlignmentOptions.Center
            );


        infoTexto.color =
            gris;


        RectTransform infoRect =
            infoTexto.rectTransform;


        infoRect.anchorMin =
            new Vector2(
                0f,
                0.5f
            );


        infoRect.anchorMax =
            new Vector2(
                1f,
                0.5f
            );


        infoRect.pivot =
            new Vector2(
                0.5f,
                0.5f
            );


        infoRect.anchoredPosition =
            new Vector2(
                0f,
                5f
            );


        infoRect.sizeDelta =
            new Vector2(
                -10f,
                50f
            );


        GameObject botonObjeto =
            CrearObjetoUI(
                "BotonColocar",
                tarjeta.transform
            );


        RectTransform botonRect =
            botonObjeto.GetComponent<
                RectTransform>();


        botonRect.anchorMin =
            new Vector2(
                0f,
                0f
            );


        botonRect.anchorMax =
            new Vector2(
                1f,
                0f
            );


        botonRect.pivot =
            new Vector2(
                0.5f,
                0f
            );


        botonRect.anchoredPosition =
            new Vector2(
                0f,
                10f
            );


        botonRect.sizeDelta =
            new Vector2(
                -20f,
                38f
            );


        Image botonImagen =
            botonObjeto
                .AddComponent<Image>();


        botonImagen.color =
            verde;


        Button boton =
            botonObjeto
                .AddComponent<Button>();


        boton.targetGraphic =
            botonImagen;


        string idGuardado =
            productId;


        boton.onClick.AddListener(
            () =>
            {
                SeleccionarProducto(
                    idGuardado
                );
            }
        );


        TMP_Text textoBoton =
            CrearTexto(
                botonObjeto.transform,
                "COLOCAR",
                14f,
                FontStyles.Bold,
                TextAlignmentOptions.Center
            );


        textoBoton.raycastTarget =
            false;
    }


    // =====================================================
    // SACAR MUEBLE
    // =====================================================

    private void SeleccionarProducto(
        string productId)
    {
        if (
            inventorySpawner == null
        )
        {
            inventorySpawner =
                Object.FindAnyObjectByType<
                    FurnitureInventorySpawner>();
        }


        if (inventorySpawner == null)
        {
            MostrarEstado(
                "No se encontro el sistema de colocacion."
            );

            Debug.LogError(
                "FurnitureInventorySpawner no esta en la escena."
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


            Debug.Log(
                "INVENTARIO UI -> preparando item " +
                item.item_id
            );


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
    // COMPROBAR ITEMS YA EN LA HABITACION
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
                Object.FindObjectsByType<
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


    // =====================================================
    // ABRIR / CERRAR
    // =====================================================

    public void AlternarInventario()
    {
        if (panel == null)
            return;


        bool abrir =
            !panel.activeSelf;


        panel.SetActive(
            abrir
        );


        if (abrir)
        {
            ActualizarInventario();
        }
    }


    public void CerrarInventario()
    {
        if (panel != null)
        {
            panel.SetActive(
                false
            );
        }
    }


    // =====================================================
    // LIMPIEZA TARJETAS
    // =====================================================

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


    // =====================================================
    // UTILIDADES UI
    // =====================================================

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
            Color.white;


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
                8f,
                5f
            );


        rect.offsetMax =
            new Vector2(
                -8f,
                -5f
            );


        return texto;
    }


    private void ConfigurarColoresBoton(
        Button boton,
        Color normal)
    {
        ColorBlock colores =
            boton.colors;


        colores.normalColor =
            normal;


        colores.highlightedColor =
            colorTarjetaHover;


        colores.pressedColor =
            new Color32(
                30,
                33,
                40,
                255
            );


        colores.selectedColor =
            colorTarjetaHover;


        boton.colors =
            colores;
    }


    // =====================================================
    // LIMPIEZA EVENTOS
    // =====================================================

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