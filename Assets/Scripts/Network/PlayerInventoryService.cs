using System;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;

public class PlayerInventoryService :
    MonoBehaviour
{
    public static PlayerInventoryService Instance
    {
        get;
        private set;
    }


    public FurnitureInventoryItemData[] Items
    {
        get;
        private set;
    } =
        Array.Empty<
            FurnitureInventoryItemData>();


    public bool InventarioCargado
    {
        get;
        private set;
    }


    public event Action
        InventarioActualizado;


    private string usuarioCargado;

    private bool cargando;

    private bool compraConectada;

    private FurniturePurchaseService
        purchaseService;


    private void Awake()
    {
        if (
            Instance != null &&
            Instance != this
        )
        {
            Destroy(
                gameObject
            );

            return;
        }


        Instance =
            this;


        DontDestroyOnLoad(
            gameObject
        );
    }


    private async void Update()
    {
        IntentarConectarCompras();


        if (
            NakamaAuthService.Instance == null ||
            !NakamaAuthService
                .Instance
                .EstaAutenticado
        )
        {
            usuarioCargado =
                null;

            InventarioCargado =
                false;

            Items =
                Array.Empty<
                    FurnitureInventoryItemData>();

            return;
        }


        string usuarioActual =
            NakamaAuthService
                .Instance
                .Session
                .UserId;


        if (
            usuarioCargado ==
            usuarioActual
        )
        {
            return;
        }


        if (cargando)
            return;


        bool correcto =
            await CargarInventario();


        if (correcto)
        {
            usuarioCargado =
                usuarioActual;
        }
    }


    private void IntentarConectarCompras()
    {
        if (compraConectada)
            return;


        purchaseService =
            FurniturePurchaseService
                .Instance;


        if (purchaseService == null)
            return;


        purchaseService
            .CompraCompletada +=
            AlCompletarCompra;


        compraConectada =
            true;
    }


    private async void AlCompletarCompra(
        FurniturePurchaseResultData resultado)
    {
        if (
            resultado == null ||
            !resultado.success
        )
        {
            return;
        }


        await CargarInventario();
    }


    public async Task<bool>
        CargarInventario()
    {
        if (
            NakamaConnection.Instance == null ||
            NakamaConnection
                .Instance
                .Client == null ||
            NakamaAuthService.Instance == null ||
            !NakamaAuthService
                .Instance
                .EstaAutenticado
        )
        {
            return false;
        }


        if (cargando)
            return false;


        cargando =
            true;


        try
        {
            IApiRpc respuesta =
                await NakamaConnection
                    .Instance
                    .Client
                    .RpcAsync(
                        NakamaAuthService
                            .Instance
                            .Session,

                        "inventory_get",

                        "{}"
                    );


            FurnitureInventoryData datos =
                JsonUtility.FromJson<
                    FurnitureInventoryData>(
                    respuesta.Payload
                );


            if (datos == null)
            {
                Debug.LogError(
                    "Respuesta de inventario invalida."
                );

                return false;
            }


            Items =
                datos.items ??
                Array.Empty<
                    FurnitureInventoryItemData>();


            InventarioCargado =
                true;


            Debug.Log(
                "INVENTARIO CARGADO -> " +
                Items.Length +
                " item(s)"
            );


            foreach (
                FurnitureInventoryItemData item
                in Items
            )
            {
                if (item == null)
                    continue;


                Debug.Log(
                    "INVENTARIO -> " +
                    item.item_id +
                    " | " +
                    item.product_id +
                    " | colocado: " +
                    item.placed
                );
            }


            InventarioActualizado?.Invoke();


            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Error cargando inventario: " +
                e.Message
            );

            return false;
        }
        finally
        {
            cargando =
                false;
        }
    }


    public FurnitureInventoryItemData
        ObtenerItem(
            string itemId)
    {
        if (
            Items == null ||
            string.IsNullOrWhiteSpace(
                itemId
            )
        )
        {
            return null;
        }


        foreach (
            FurnitureInventoryItemData item
            in Items
        )
        {
            if (
                item != null &&
                item.item_id ==
                itemId
            )
            {
                return item;
            }
        }


        return null;
    }


    private void OnDestroy()
    {
        if (
            purchaseService != null &&
            compraConectada
        )
        {
            purchaseService
                .CompraCompletada -=
                AlCompletarCompra;
        }
    }
}