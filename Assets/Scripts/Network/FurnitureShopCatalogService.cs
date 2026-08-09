using System;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;

public class FurnitureShopCatalogService :
    MonoBehaviour
{
    public static FurnitureShopCatalogService
        Instance
    {
        get;
        private set;
    }


    public FurnitureShopProductData[]
        Productos
    {
        get;
        private set;
    }


    public bool CatalogoCargado
    {
        get;
        private set;
    }


    public event Action
        CatalogoActualizado;


    private string usuarioCargado;

    private bool cargando;


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
        if (
            NakamaAuthService.Instance == null ||
            !NakamaAuthService
                .Instance
                .EstaAutenticado
        )
        {
            usuarioCargado =
                null;

            CatalogoCargado =
                false;

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
            await CargarCatalogo();


        if (correcto)
        {
            usuarioCargado =
                usuarioActual;
        }
    }


    public async Task<bool>
        CargarCatalogo()
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

                        "shop_get_catalog",

                        "{}"
                    );


            FurnitureShopCatalogData datos =
                JsonUtility.FromJson<
                    FurnitureShopCatalogData>(
                    respuesta.Payload
                );


            if (
                datos == null ||
                datos.products == null
            )
            {
                Debug.LogError(
                    "Catalogo de tienda invalido."
                );

                return false;
            }


            Productos =
                datos.products;


            CatalogoCargado =
                true;


            CatalogoActualizado?.Invoke();


            Debug.Log(
                "CATALOGO TIENDA CARGADO -> " +
                Productos.Length +
                " producto(s)"
            );


            foreach (
                FurnitureShopProductData producto
                in Productos
            )
            {
                Debug.Log(
                    "TIENDA -> " +
                    producto.id +
                    " | " +
                    producto.name +
                    " | " +
                    producto.price +
                    " monedas"
                );
            }


            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Error cargando catalogo de tienda: " +
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


    public FurnitureShopProductData
        ObtenerProducto(
            string productId
        )
    {
        if (
            Productos == null ||
            string.IsNullOrWhiteSpace(
                productId
            )
        )
        {
            return null;
        }


        foreach (
            FurnitureShopProductData producto
            in Productos
        )
        {
            if (
                producto != null &&
                producto.id ==
                productId
            )
            {
                return producto;
            }
        }


        return null;
    }
}