using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WalletAutoUI : MonoBehaviour
{
    private TMP_Text textoSaldo;

    private PlayerWalletService walletService;

    private bool conectado;

    private static readonly CultureInfo Cultura =
        CultureInfo.GetCultureInfo("es-CO");


    private void Start()
    {
        CrearInterfaz();

        IntentarConectar();
    }


    private void Update()
    {
        if (!conectado)
        {
            IntentarConectar();
        }
    }


    private void IntentarConectar()
    {
        if (conectado)
            return;

        walletService =
            PlayerWalletService.Instance;

        if (walletService == null)
            return;

        walletService.SaldoActualizado +=
            ActualizarSaldo;

        conectado = true;

        if (walletService.SaldoCargado)
        {
            ActualizarSaldo(
                walletService.SaldoActual
            );
        }
        else
        {
            MostrarCargando();
        }
    }


    private void CrearInterfaz()
    {
        GameObject canvasObjeto =
            new GameObject(
                "WalletUI",
                typeof(RectTransform)
            );

        canvasObjeto.transform.SetParent(
            transform,
            false
        );

        Canvas canvas =
            canvasObjeto.AddComponent<Canvas>();

        canvas.renderMode =
            RenderMode.ScreenSpaceOverlay;

        canvas.sortingOrder =
            150;


        CanvasScaler scaler =
            canvasObjeto.AddComponent<
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


        canvasObjeto.AddComponent<
            GraphicRaycaster>();


        GameObject panel =
            new GameObject(
                "PanelMonedas",
                typeof(RectTransform)
            );

        panel.transform.SetParent(
            canvasObjeto.transform,
            false
        );


        RectTransform panelRect =
            panel.GetComponent<
                RectTransform>();

        panelRect.anchorMin =
            new Vector2(
                1f,
                1f
            );

        panelRect.anchorMax =
            new Vector2(
                1f,
                1f
            );

        panelRect.pivot =
            new Vector2(
                1f,
                1f
            );

        panelRect.anchoredPosition =
            new Vector2(
                -35f,
                -35f
            );

        panelRect.sizeDelta =
            new Vector2(
                270f,
                72f
            );


        Image fondo =
            panel.AddComponent<Image>();

        fondo.color =
            new Color32(
                20,
                22,
                27,
                235
            );


        GameObject tituloObjeto =
            new GameObject(
                "Titulo",
                typeof(RectTransform)
            );

        tituloObjeto.transform.SetParent(
            panel.transform,
            false
        );


        TMP_Text titulo =
            tituloObjeto.AddComponent<
                TextMeshProUGUI>();

        titulo.text =
            "MONEDAS";

        titulo.fontSize =
            15f;

        titulo.fontStyle =
            FontStyles.Bold;

        titulo.color =
            new Color32(
                41,
                190,
                120,
                255
            );

        titulo.alignment =
            TextAlignmentOptions.Left;

        titulo.raycastTarget =
            false;


        RectTransform tituloRect =
            titulo.rectTransform;

        tituloRect.anchorMin =
            Vector2.zero;

        tituloRect.anchorMax =
            Vector2.one;

        tituloRect.offsetMin =
            new Vector2(
                18f,
                37f
            );

        tituloRect.offsetMax =
            new Vector2(
                -18f,
                -7f
            );


        GameObject saldoObjeto =
            new GameObject(
                "Saldo",
                typeof(RectTransform)
            );

        saldoObjeto.transform.SetParent(
            panel.transform,
            false
        );


        textoSaldo =
            saldoObjeto.AddComponent<
                TextMeshProUGUI>();

        textoSaldo.text =
            "---";

        textoSaldo.fontSize =
            25f;

        textoSaldo.fontStyle =
            FontStyles.Bold;

        textoSaldo.color =
            Color.white;

        textoSaldo.alignment =
            TextAlignmentOptions.Left;

        textoSaldo.raycastTarget =
            false;


        RectTransform saldoRect =
            textoSaldo.rectTransform;

        saldoRect.anchorMin =
            Vector2.zero;

        saldoRect.anchorMax =
            Vector2.one;

        saldoRect.offsetMin =
            new Vector2(
                18f,
                5f
            );

        saldoRect.offsetMax =
            new Vector2(
                -18f,
                -31f
            );
    }


    private void MostrarCargando()
    {
        if (textoSaldo == null)
            return;

        textoSaldo.text =
            "...";
    }


    private void ActualizarSaldo(
        long nuevoSaldo)
    {
        if (textoSaldo == null)
            return;

        textoSaldo.text =
            nuevoSaldo.ToString(
                "N0",
                Cultura
            );

        Debug.Log(
            "UI MONEDAS ACTUALIZADA -> " +
            nuevoSaldo
        );
    }


    private void OnDestroy()
    {
        if (
            walletService != null &&
            conectado
        )
        {
            walletService.SaldoActualizado -=
                ActualizarSaldo;
        }
    }
}