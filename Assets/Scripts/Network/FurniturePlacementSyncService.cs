using System;
using System.Threading.Tasks;
using Nakama;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FurniturePlacementSyncService :
    MonoBehaviour
{
    public static FurniturePlacementSyncService
        Instance
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
        FurniturePlacementSyncResultData>
        GuardarColocacion(
            string itemId,
            Vector2Int ancla,
            int rotacionY)
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


        if (
            NakamaConnection.Instance == null ||
            NakamaConnection.Instance.Client == null ||
            NakamaAuthService.Instance == null ||
            !NakamaAuthService.Instance
                .EstaAutenticado
        )
        {
            return CrearError(
                "not_authenticated",
                "No hay una sesion activa."
            );
        }


        rotacionY =
            NormalizarRotacion(
                rotacionY
            );


        FurniturePlacementSyncRequestData
            solicitud =
                new
                FurniturePlacementSyncRequestData
                {
                    item_id =
                        itemId,

                    room_id =
                        SceneManager
                            .GetActiveScene()
                            .name,

                    grid_x =
                        ancla.x,

                    grid_z =
                        ancla.y,

                    rotation_y =
                        rotacionY
                };


        try
        {
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

                        "inventory_place",

                        payload
                    );


            FurniturePlacementSyncResultData
                resultado =
                    JsonUtility.FromJson<
                        FurniturePlacementSyncResultData>(
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
                    "COLOCACION GUARDADA -> " +
                    resultado.item_id +
                    " | (" +
                    resultado.grid_x +
                    ", " +
                    resultado.grid_z +
                    ")" +
                    " | rotacion " +
                    resultado.rotation_y
                );


                if (
                    PlayerInventoryService
                        .Instance != null
                )
                {
                    await PlayerInventoryService
                        .Instance
                        .CargarInventario();
                }
            }
            else
            {
                Debug.LogWarning(
                    "COLOCACION NO GUARDADA -> " +
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
                "Error sincronizando colocacion: " +
                e.Message
            );


            return CrearError(
                "network_error",
                "No se pudo guardar la colocacion."
            );
        }
    }


    private int NormalizarRotacion(
        int rotacion)
    {
        rotacion %= 360;


        if (rotacion < 0)
        {
            rotacion += 360;
        }


        return
            Mathf.RoundToInt(
                rotacion / 90f
            )
            * 90
            % 360;
    }


    private FurniturePlacementSyncResultData
        CrearError(
            string codigo,
            string mensaje)
    {
        return new
            FurniturePlacementSyncResultData
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