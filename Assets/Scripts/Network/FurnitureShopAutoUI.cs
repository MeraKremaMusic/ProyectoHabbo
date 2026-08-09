using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureShopAutoUI : MonoBehaviour
{
    private GameObject panelTienda;

    private Transform contenidoProductos;

    private TMP_Text textoSaldo;
    private TMP_Text textoEstado;

    private FurnitureShopCatalogService catalogoService;
    private PlayerWalletService walletService;

    private bool catalogoConectado;
    private bool walletConectado;

    private bool interfazCreada;


    private readonly Color fondoPanel =
        new Color32(
            22,
            24,
            29,
            250
        );

    private readonly Color fondoTarjeta =
        new Color32(
            39,
            43,
            52,
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
            170,
            176,
            188,
            255
        );


    private void Start()
    {
        CrearInterfaz();

        IntentarConectarServicios();
    }


    private void Update()
    {
        if (
            !catalogoConectado ||
            !walletConectado
        )
        {
            IntentarConectarServicios();
        }
    }


    // =====================================================
    // CONEXION CON SERVICIOS
    // =====================================================

    private void IntentarConectarServicios()
    {
        if (!catalogoConectado)
        {
            catalogoService =
                FurnitureShopCatalogService
                    .Instance;

            if (catalogoService != null)
            {
                catalogoService
                    .CatalogoActualizado +=
                    ActualizarProductos;

                catalogoConectado = true;

                if (
                    catalogoService
                        .CatalogoCargado
                )
                {
                    ActualizarProductos();
                }
            }
        }


        if (!walletConectado)
        {
            walletService =
                PlayerWalletService.Instance;

            if (walletService != null)
            {
                walletService
                    .SaldoActualizado +=
                    ActualizarSaldo;

                walletConectado = true;

                if (
                    walletService
                        .SaldoCargado
                )
                {
                    ActualizarSaldo(
                        walletService
                            .SaldoActual
                    );
                }
            }
        }
    }


    // =====================================================
    // CREAR INTERFAZ
    // =====================================================

    private void CrearInterfaz()
    {
        if (interfazCreada)
            return;

        interfazCreada = true;


        GameObject canvasObjeto =
            new GameObject(
                "FurnitureShopUI",
                typeof(RectTransform)
            );


        Canvas canvas =
            canvasObjeto.AddComponent<
                Canvas>();

        canvas.renderMode =
            RenderMode.ScreenSpaceOverlay;

        canvas.sortingOrder =
            160;


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


        CrearBotonTienda(
            canvasObjeto.transform
        );


        CrearPanelTienda(
            canvasObjeto.transform
        );


        panelTienda.SetActive(
            false
        );
    }


    // =====================================================
    // BOTON TIENDA
    // =====================================================

    private void CrearBotonTienda(
        Transform padre)
    {
        GameObject objeto =
            CrearObjetoUI(
                "BotonTienda",
                padre
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
                220f,
                30f
            );

        rect.sizeDelta =
            new Vector2(
                180f,
                58f
            );


        Image imagen =
            objeto.AddComponent<Image>();

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

        boton.onClick.AddListener(
            AbrirTienda
        );


        TMP_Text texto =
            CrearTexto(
                objeto.transform,
                "TIENDA",
                18f,
                FontStyles.Bold
            );

        Estirar(
            texto.rectTransform
        );
    }


    // =====================================================
    // PANEL PRINCIPAL
    // =====================================================

    private void CrearPanelTienda(
        Transform padre)
    {
        panelTienda =
            CrearObjetoUI(
                "PanelTienda",
                padre
            );


        RectTransform rect =
            panelTienda.GetComponent<
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
                960f,
                680f
            );


        Image imagen =
            panelTienda.AddComponent<
                Image>();

        imagen.color =
            fondoPanel;


        CrearCabecera();

        CrearZonaProductos();

        CrearEstado();
    }


    // =====================================================
    // CABECERA
    // =====================================================

    private void CrearCabecera()
    {
        TMP_Text titulo =
            CrearTexto(
                panelTienda.transform,
                "TIENDA",
                32f,
                FontStyles.Bold
            );


        ConfigurarRect(
            titulo.rectTransform,
            35f,
            -25f,
            -300f,
            -80f
        );


        titulo.alignment =
            TextAlignmentOptions.Left;


        textoSaldo =
            CrearTexto(
                panelTienda.transform,
                "MONEDAS: ...",
                19f,
                FontStyles.Bold
            );


        textoSaldo.color =
            verde;

        textoSaldo.alignment =
            TextAlignmentOptions.Right;


        ConfigurarRect(
            textoSaldo.rectTransform,
            600f,
            -30f,
            -90f,
            -75f
        );


        GameObject cerrarObjeto =
            CrearObjetoUI(
                "BotonCerrar",
                panelTienda.transform
            );


        RectTransform cerrarRect =
            cerrarObjeto.GetComponent<
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
                -20f,
                -20f
            );

        cerrarRect.sizeDelta =
            new Vector2(
                50f,
                50f
            );


        Image imagen =
            cerrarObjeto.AddComponent<
                Image>();

        imagen.color =
            new Color32(
                45,
                49,
                59,
                255
            );


        Button boton =
            cerrarObjeto.AddComponent<
                Button>();

        boton.targetGraphic =
            imagen;

        boton.onClick.AddListener(
            CerrarTienda
        );


        TMP_Text textoCerrar =
            CrearTexto(
                cerrarObjeto.transform,
                "X",
                20f,
                FontStyles.Bold
            );

        Estirar(
            textoCerrar.rectTransform
        );
    }


    // =====================================================
    // ZONA DE PRODUCTOS
    // =====================================================

    private void CrearZonaProductos()
    {
        GameObject scrollObjeto =
            CrearObjetoUI(
                "ScrollProductos",
                panelTienda.transform
            );


        ConfigurarRect(
            scrollObjeto.GetComponent<
                RectTransform>(),
            35f,
            -105f,
            -35f,
            -600f
        );


        ScrollRect scroll =
            scrollObjeto.AddComponent<
                ScrollRect>();


        Image fondoScroll =
            scrollObjeto.AddComponent<
                Image>();

        fondoScroll.color =
            new Color32(
                17,
                19,
                23,
                255
            );


        GameObject viewport =
            CrearObjetoUI(
                "Viewport",
                scrollObjeto.transform
            );


        RectTransform viewportRect =
            viewport.GetComponent<
                RectTransform>();

        viewportRect.anchorMin =
            Vector2.zero;

        viewportRect.anchorMax =
            Vector2.one;

        viewportRect.offsetMin =
            new Vector2(
                15f,
                15f
            );

        viewportRect.offsetMax =
            new Vector2(
                -15f,
                -15f
            );


        Image viewportImage =
            viewport.AddComponent<
                Image>();

        viewportImage.color =
            new Color(
                1f,
                1f,
                1f,
                0.01f
            );


        viewport.AddComponent<
            Mask>()
            .showMaskGraphic =
            false;


        GameObject contenido =
            CrearObjetoUI(
                "Content",
                viewport.transform
            );


        contenidoProductos =
            contenido.transform;


        RectTransform contenidoRect =
            contenido.GetComponent<
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
                0f
            );


        GridLayoutGroup grid =
            contenido.AddComponent<
                GridLayoutGroup>();

        grid.cellSize =
            new Vector2(
                270f,
                210f
            );

        grid.spacing =
            new Vector2(
                20f,
                20f
            );

        grid.padding =
            new RectOffset(
                10,
                10,
                10,
                10
            );

        grid.constraint =
            GridLayoutGroup
                .Constraint
                .FixedColumnCount;

        grid.constraintCount =
            3;


        ContentSizeFitter fitter =
            contenido.AddComponent<
                ContentSizeFitter>();

        fitter.verticalFit =
            ContentSizeFitter
                .FitMode
                .PreferredSize;


        scroll.viewport =
            viewportRect;

        scroll.content =
            contenidoRect;

        scroll.horizontal =
            false;

        scroll.vertical =
            true;
    }


    // =====================================================
    // CREAR TARJETAS
    // =====================================================

    private void ActualizarProductos()
    {
        if (
            contenidoProductos == null ||
            catalogoService == null ||
            !catalogoService
                .CatalogoCargado
        )
        {
            return;
        }


        foreach (
            Transform hijo
            in contenidoProductos
        )
        {
            Destroy(
                hijo.gameObject
            );
        }


        FurnitureShopProductData[]
            productos =
                catalogoService
                    .Productos;


        if (
            productos == null ||
            productos.Length == 0
        )
        {
            MostrarEstado(
                "No hay productos disponibles."
            );

            return;
        }


        foreach (
            FurnitureShopProductData producto
            in productos
        )
        {
            if (producto == null)
                continue;

            CrearTarjetaProducto(
                producto
            );
        }


        MostrarEstado(
            productos.Length +
            " producto(s) disponible(s)"
        );
    }


    private void CrearTarjetaProducto(
        FurnitureShopProductData producto)
    {
        GameObject tarjeta =
            CrearObjetoUI(
                "Producto_" +
                producto.id,
                contenidoProductos
            );


        Image fondo =
            tarjeta.AddComponent<Image>();

        fondo.color =
            fondoTarjeta;


        TMP_Text nombre =
            CrearTexto(
                tarjeta.transform,
                producto.name,
                21f,
                FontStyles.Bold
            );


        ConfigurarRect(
            nombre.rectTransform,
            15f,
            -18f,
            -15f,
            -60f
        );


        TMP_Text categoria =
            CrearTexto(
                tarjeta.transform,
                producto.category,
                14f,
                FontStyles.Normal
            );

        categoria.color =
            gris;


        ConfigurarRect(
            categoria.rectTransform,
            15f,
            -65f,
            -15f,
            -100f
        );


        TMP_Text precio =
            CrearTexto(
                tarjeta.transform,
                producto.price +
                " MONEDAS",
                18f,
                FontStyles.Bold
            );

        precio.color =
            verde;


        ConfigurarRect(
            precio.rectTransform,
            15f,
            -110f,
            -15f,
            -150f
        );


        CrearBotonComprar(
            tarjeta.transform,
            producto
        );
    }


    // =====================================================
    // BOTON COMPRAR
    // =====================================================

    private void CrearBotonComprar(
        Transform padre,
        FurnitureShopProductData producto)
    {
        GameObject objeto =
            CrearObjetoUI(
                "BotonComprar",
                padre
            );


        ConfigurarRect(
            objeto.GetComponent<
                RectTransform>(),
            20f,
            -155f,
            -20f,
            -200f
        );


        Image imagen =
            objeto.AddComponent<Image>();

        imagen.color =
            verde;


        Button boton =
            objeto.AddComponent<Button>();

        boton.targetGraphic =
            imagen;


        boton.onClick.AddListener(
            () =>
            {
                CompraPendiente(
                    producto
                );
            }
        );


        TMP_Text texto =
            CrearTexto(
                objeto.transform,
                "COMPRAR",
                16f,
                FontStyles.Bold
            );

        Estirar(
            texto.rectTransform
        );
    }


    private void CompraPendiente(
        FurnitureShopProductData producto)
    {
        Debug.Log(
            "COMPRA SOLICITADA -> " +
            producto.id
        );


        MostrarEstado(
            "Sistema de compras en el siguiente paso."
        );
    }


    // =====================================================
    // SALDO
    // =====================================================

    private void ActualizarSaldo(
        long saldo)
    {
        if (textoSaldo == null)
            return;


        textoSaldo.text =
            "MONEDAS: " +
            saldo.ToString("N0");
    }


    // =====================================================
    // ABRIR / CERRAR
    // =====================================================

    private void AbrirTienda()
    {
        panelTienda.SetActive(
            true
        );


        if (
            catalogoService != null &&
            catalogoService
                .CatalogoCargado
        )
        {
            ActualizarProductos();
        }


        if (
            walletService != null &&
            walletService
                .SaldoCargado
        )
        {
            ActualizarSaldo(
                walletService
                    .SaldoActual
            );
        }
    }


    private void CerrarTienda()
    {
        panelTienda.SetActive(
            false
        );
    }


    // =====================================================
    // ESTADO
    // =====================================================

    private void CrearEstado()
    {
        textoEstado =
            CrearTexto(
                panelTienda.transform,
                "Cargando tienda...",
                14f,
                FontStyles.Normal
            );

        textoEstado.color =
            gris;


        ConfigurarRect(
            textoEstado.rectTransform,
            35f,
            -615f,
            -35f,
            -660f
        );
    }


    private void MostrarEstado(
        string contenido)
    {
        if (textoEstado == null)
            return;


        textoEstado.text =
            contenido;
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


    // =====================================================
    // LIMPIEZA
    // =====================================================

    private void OnDestroy()
    {
        if (
            catalogoService != null &&
            catalogoConectado
        )
        {
            catalogoService
                .CatalogoActualizado -=
                ActualizarProductos;
        }


        if (
            walletService != null &&
            walletConectado
        )
        {
            walletService
                .SaldoActualizado -=
                ActualizarSaldo;
        }
    }
}