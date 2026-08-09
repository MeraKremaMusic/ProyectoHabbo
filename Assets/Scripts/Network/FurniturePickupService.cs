using System;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FurniturePickupService :
    MonoBehaviour
{
    public static FurniturePickupService Instance
    {
        get;
        private set;
    }


    public bool SolicitudEnCurso
    {
        get;
        private set;
    }


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
        FurniturePickupResultData>
        Recoger(
            string itemId)
    {
        if (
            string.IsNullOrWhiteSpace(
                itemId
            )
        )
        {
            return CrearError(
                "invalid_item",
                "El mueble no tiene Item ID."
            );
        }


        if (SolicitudEnCurso)
        {
            return CrearError(
                "request_in_progress",
                "Ya se esta recogiendo un mueble."
            );
        }


        if (
            NakamaConnection.Instance == null ||
            NakamaConnection.Instance.Client == null ||
            NakamaAuthService.Instance == null ||
            !NakamaAuthService
                .Instance
                .EstaAutenticado
        )
        {
            return CrearError(
                "not_authenticated",
                "No hay una sesion activa."
            );
        }


        SolicitudEnCurso =
            true;


        try
        {
            FurniturePickupRequestData solicitud =
                new FurniturePickupRequestData
                {
                    item_id =
                        itemId,

                    room_id =
                        SceneManager
                            .GetActiveScene()
                            .name
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

                        "inventory_pickup",

                        payload
                    );


            FurniturePickupResultData resultado =
                JsonUtility.FromJson<
                    FurniturePickupResultData>(
                    respuesta.Payload
                );


            if (resultado == null)
            {
                return CrearError(
                    "invalid_response",
                    "Respuesta invalida del servidor."
                );
            }


            if (resultado.success)
            {
                Debug.Log(
                    "MUEBLE RECOGIDO EN NAKAMA -> " +
                    resultado.item_id
                );
            }
            else
            {
                Debug.LogWarning(
                    "NO SE PUDO RECOGER -> " +
                    resultado.code +
                    " | " +
                    resultado.message
                );
            }


            return resultado;
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Error recogiendo mueble: " +
                e.Message
            );


            return CrearError(
                "network_error",
                "No se pudo conectar con el servidor."
            );
        }
        finally
        {
            SolicitudEnCurso =
                false;
        }
    }


    private FurniturePickupResultData
        CrearError(
            string codigo,
            string mensaje)
    {
        return new FurniturePickupResultData
        {
            success =
                false,

            code =
                codigo,

            message =
                mensaje
        };
    }
}