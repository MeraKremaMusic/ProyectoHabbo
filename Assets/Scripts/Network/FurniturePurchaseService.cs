using System;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;

public class FurniturePurchaseService :
    MonoBehaviour
{
    public static FurniturePurchaseService
        Instance
    {
        get;
        private set;
    }


    public bool CompraEnCurso
    {
        get;
        private set;
    }


    public event Action<
        FurniturePurchaseResultData>
        CompraCompletada;


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


    public async Task<
        FurniturePurchaseResultData>
        Comprar(
            string productId
        )
    {
        if (CompraEnCurso)
        {
            return new
                FurniturePurchaseResultData
            {
                success =
                    false,

                code =
                    "purchase_in_progress",

                message =
                    "Ya hay una compra en proceso."
            };
        }


        if (
            string.IsNullOrWhiteSpace(
                productId
            )
        )
        {
            return new
                FurniturePurchaseResultData
            {
                success =
                    false,

                code =
                    "invalid_product",

                message =
                    "Producto invalido."
            };
        }


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
            return new
                FurniturePurchaseResultData
            {
                success =
                    false,

                code =
                    "not_authenticated",

                message =
                    "No hay una sesion activa."
            };
        }


        CompraEnCurso =
            true;


        try
        {
            FurniturePurchaseRequestData
                solicitud =
                    new
                    FurniturePurchaseRequestData
                    {
                        product_id =
                            productId
                    };


            string payload =
                JsonUtility.ToJson(
                    solicitud
                );


            IApiRpc respuesta =
                await NakamaConnection
                    .Instance
                    .Client
                    .RpcAsync(
                        NakamaAuthService
                            .Instance
                            .Session,

                        "shop_buy",

                        payload
                    );


            FurniturePurchaseResultData
                resultado =
                    JsonUtility.FromJson<
                        FurniturePurchaseResultData>(
                        respuesta.Payload
                    );


            if (resultado == null)
            {
                resultado =
                    new
                    FurniturePurchaseResultData
                    {
                        success =
                            false,

                        code =
                            "invalid_response",

                        message =
                            "Respuesta de compra invalida."
                    };
            }


            if (resultado.success)
            {
                Debug.Log(
                    "COMPRA COMPLETADA -> " +
                    resultado.product_id +
                    " | ITEM -> " +
                    resultado.item_id +
                    " | SALDO -> " +
                    resultado.coins
                );


                // El WalletService sigue siendo
                // el dueño del saldo local.
                // Solo le pedimos que se refresque.
                if (
                    PlayerWalletService
                        .Instance != null
                )
                {
                    await PlayerWalletService
                        .Instance
                        .CargarSaldo();
                }
            }
            else
            {
                Debug.LogWarning(
                    "COMPRA RECHAZADA -> " +
                    resultado.code +
                    " | " +
                    resultado.message
                );
            }


            CompraCompletada?.Invoke(
                resultado
            );


            return resultado;
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Error realizando compra: " +
                e.Message
            );


            FurniturePurchaseResultData
                error =
                    new
                    FurniturePurchaseResultData
                    {
                        success =
                            false,

                        code =
                            "network_error",

                        message =
                            "No se pudo conectar con la tienda."
                    };


            CompraCompletada?.Invoke(
                error
            );


            return error;
        }
        finally
        {
            CompraEnCurso =
                false;
        }
    }
}