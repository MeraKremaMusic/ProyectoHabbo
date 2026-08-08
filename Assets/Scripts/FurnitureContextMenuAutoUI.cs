using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureContextMenuAutoUI : MonoBehaviour
{
    private FurnitureSelection selection;
    private FurnitureMove furnitureMove;
    private FurnitureRotation furnitureRotation;
    private FurnitureDelete furnitureDelete;
    private FurniturePlacement placement;

    private Canvas canvas;
    private GameObject panel;

    private TMP_Text nombreTexto;
    private TMP_Text tamanoTexto;

    private GameObject ultimoSeleccionado;

    private readonly Color colorPanel =
        new Color32(24, 27, 34, 250);

    private readonly Color colorBoton =
        new Color32(47, 52, 63, 255);

    private readonly Color colorHover =
        new Color32(66, 74, 89, 255);

    private readonly Color colorEliminar =
        new Color32(145, 52, 59, 255);

    private void Awake()
    {
        BuscarReferencias();
    }

    private void Start()
    {
        CrearInterfaz();
        Ocultar();
    }

    private void Update()
    {
        if (
            selection == null ||
            panel == null
        )
        {
            return;
        }

        if (
            placement != null &&
            placement.EstaColocando
        )
        {
            Ocultar();
            return;
        }

        GameObject seleccionado =
            selection.muebleSeleccionado;

        if (seleccionado == null)
        {
            ultimoSeleccionado = null;
            Ocultar();
            return;
        }

        if (
            seleccionado !=
            ultimoSeleccionado
        )
        {
            ultimoSeleccionado =
                seleccionado;

            ActualizarInformacion(
                seleccionado
            );

            Mostrar();
        }
    }

    private void BuscarReferencias()
    {
        selection =
            GetComponent<FurnitureSelection>();

        furnitureMove =
            GetComponent<FurnitureMove>();

        furnitureRotation =
            GetComponent<FurnitureRotation>();

        furnitureDelete =
            GetComponent<FurnitureDelete>();

        placement =
            GetComponent<FurniturePlacement>();
    }

    private void CrearInterfaz()
    {
        GameObject objetoCanvas =
            new GameObject(
                "FurnitureContextUI"
            );

        canvas =
            objetoCanvas.AddComponent<Canvas>();

        canvas.renderMode =
            RenderMode.ScreenSpaceOverlay;

        canvas.sortingOrder = 50;

        CanvasScaler scaler =
            objetoCanvas.AddComponent<CanvasScaler>();

        scaler.uiScaleMode =
            CanvasScaler.ScaleMode.ScaleWithScreenSize;

        scaler.referenceResolution =
            new Vector2(1920f, 1080f);

        scaler.matchWidthOrHeight =
            0.5f;

        objetoCanvas.AddComponent<GraphicRaycaster>();

        CrearPanel();
    }

    private void CrearPanel()
    {
        panel =
            CrearObjetoUI(
                "MenuMueble",
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
            new Vector2(0f, 35f);

        rect.sizeDelta =
            new Vector2(620f, 118f);

        Image fondo =
            panel.AddComponent<Image>();

        fondo.color =
            colorPanel;

        CrearInformacion();
        CrearBotones();
    }

    private void CrearInformacion()
    {
        GameObject zona =
            CrearObjetoUI(
                "Informacion",
                panel.transform
            );

        RectTransform rect =
            zona.GetComponent<RectTransform>();

        rect.anchorMin =
            new Vector2(0f, 0f);

        rect.anchorMax =
            new Vector2(0f, 1f);

        rect.pivot =
            new Vector2(0f, 0.5f);

        rect.anchoredPosition =
            new Vector2(20f, 0f);

        rect.sizeDelta =
            new Vector2(200f, 0f);

        nombreTexto =
            CrearTexto(
                zona.transform,
                "Mueble",
                22,
                FontStyles.Bold,
                TextAlignmentOptions.MidlineLeft
            );

        RectTransform nombreRect =
            nombreTexto.rectTransform;

        nombreRect.anchorMin =
            new Vector2(0f, 0.45f);

        nombreRect.anchorMax =
            new Vector2(1f, 1f);

        nombreRect.offsetMin =
            Vector2.zero;

        nombreRect.offsetMax =
            Vector2.zero;


        tamanoTexto =
            CrearTexto(
                zona.transform,
                "",
                15,
                FontStyles.Normal,
                TextAlignmentOptions.MidlineLeft
            );

        tamanoTexto.color =
            new Color32(
                180,
                185,
                195,
                255
            );

        RectTransform tamanoRect =
            tamanoTexto.rectTransform;

        tamanoRect.anchorMin =
            new Vector2(0f, 0f);

        tamanoRect.anchorMax =
            new Vector2(1f, 0.5f);

        tamanoRect.offsetMin =
            Vector2.zero;

        tamanoRect.offsetMax =
            Vector2.zero;
    }

    private void CrearBotones()
    {
        CrearBoton(
            "Mover",
            new Vector2(235f, 28f),
            new Vector2(105f, 62f),
            colorBoton,
            AccionMover
        );

        CrearBoton(
            "Rotar",
            new Vector2(352f, 28f),
            new Vector2(105f, 62f),
            colorBoton,
            AccionRotar
        );

        CrearBoton(
            "Eliminar",
            new Vector2(469f, 28f),
            new Vector2(125f, 62f),
            colorEliminar,
            AccionEliminar
        );
    }

    private void CrearBoton(
        string texto,
        Vector2 posicion,
        Vector2 tamano,
        Color color,
        UnityEngine.Events.UnityAction accion)
    {
        GameObject objeto =
            CrearObjetoUI(
                "Boton" + texto,
                panel.transform
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
            posicion;

        rect.sizeDelta =
            tamano;

        Image imagen =
            objeto.AddComponent<Image>();

        imagen.color =
            color;

        Button boton =
            objeto.AddComponent<Button>();

        ColorBlock colores =
            boton.colors;

        colores.normalColor =
            color;

        colores.highlightedColor =
            colorHover;

        colores.pressedColor =
            new Color32(
                30,
                33,
                40,
                255
            );

        boton.colors =
            colores;

        boton.onClick.AddListener(
            accion
        );

        CrearTexto(
            objeto.transform,
            texto,
            17,
            FontStyles.Bold,
            TextAlignmentOptions.Center
        );
    }

    private void AccionMover()
    {
        if (furnitureMove == null)
            return;

        furnitureMove.MoverSeleccionado();

        ultimoSeleccionado = null;

        Ocultar();
    }

    private void AccionRotar()
    {
        if (furnitureRotation == null)
            return;

        furnitureRotation.RotarSeleccionado();

        if (
            selection != null &&
            selection.muebleSeleccionado != null
        )
        {
            ActualizarInformacion(
                selection.muebleSeleccionado
            );
        }
    }

    private void AccionEliminar()
    {
        if (furnitureDelete == null)
            return;

        furnitureDelete.EliminarSeleccionado();

        ultimoSeleccionado = null;

        Ocultar();
    }

    private void ActualizarInformacion(
        GameObject mueble)
    {
        if (mueble == null)
            return;

        string nombre =
            mueble.name.Replace(
                "(Clone)",
                ""
            );

        nombreTexto.text =
            nombre;

        FurnitureData datos =
            mueble.GetComponent<FurnitureData>();

        if (datos != null)
        {
            tamanoTexto.text =
                datos.AnchoActual +
                " × " +
                datos.LargoActual +
                " casillas";
        }
        else
        {
            tamanoTexto.text = "";
        }
    }

    private void Mostrar()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }
    }

    private void Ocultar()
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
        string contenido,
        float tamano,
        FontStyles estilo,
        TextAlignmentOptions alineacion)
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
            new Vector2(8f, 5f);

        rect.offsetMax =
            new Vector2(-8f, -5f);

        return texto;
    }
}