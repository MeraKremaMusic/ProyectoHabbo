using System;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;

public class PlayerWalletService :
    MonoBehaviour
{
    public static PlayerWalletService Instance
    {
        get;
        private set;
    }


    public long SaldoActual
    {
        get;
        private set;
    }


    public bool SaldoCargado
    {
        get;
        private set;
    }


    public event Action<long> SaldoActualizado;


    private string usuarioCargado;

    private bool cargando;


    private void Awake()
    {
        if (
            Instance != null &&
            Instance != this
        )
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(
            gameObject
        );
    }


    private async void Update()
    {
        if (
            NakamaAuthService.Instance == null ||
            !NakamaAuthService.Instance
                .EstaAutenticado
        )
        {
            usuarioCargado = null;

            SaldoCargado = false;

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
            await CargarSaldo();


        if (correcto)
        {
            usuarioCargado =
                usuarioActual;
        }
    }


    public async Task<bool> CargarSaldo()
    {
        if (
            NakamaConnection.Instance == null ||
            NakamaConnection.Instance.Client == null ||
            NakamaAuthService.Instance == null ||
            !NakamaAuthService.Instance
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

                        "economy_get_wallet",

                        "{}"
                    );


            WalletData datos =
                JsonUtility.FromJson<
                    WalletData>(
                    respuesta.Payload
                );


            if (datos == null)
            {
                Debug.LogError(
                    "Respuesta de wallet invalida."
                );

                return false;
            }


            SaldoActual =
                datos.coins;

            SaldoCargado =
                true;


            SaldoActualizado?.Invoke(
                SaldoActual
            );


            Debug.Log(
                "MONEDAS CARGADAS -> " +
                SaldoActual
            );


            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Error cargando monedas: " +
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
}