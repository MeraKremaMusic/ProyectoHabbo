using UnityEngine;
using UnityEngine.InputSystem;

public class FurnitureRotation :
    MonoBehaviour
{
    [Header("Referencias")]
    public FurniturePlacement placement;

    public FurnitureSelection selection;


    private void Awake()
    {
        if (selection == null)
        {
            selection =
                GetComponent<
                    FurnitureSelection>();
        }
    }


    private void Update()
    {
        if (Keyboard.current == null)
            return;


        if (
            !Keyboard.current.rKey
                .wasPressedThisFrame
        )
        {
            return;
        }


        // Si estamos colocando un mueble,
        // R solamente rota el preview.
        // La sincronizacion con Nakama
        // ocurrira cuando confirmemos.
        if (
            placement != null &&
            placement.muebleActual != null
        )
        {
            RotarMuebleActual();

            return;
        }


        RotarSeleccionado();
    }


    // =====================================================
    // ROTAR MUEBLE QUE TODAVIA ESTA EN COLOCACION
    // =====================================================

    public void RotarMuebleActual()
    {
        if (
            placement == null ||
            placement.muebleActual == null
        )
        {
            return;
        }


        FurnitureData datos =
            placement.muebleActual
                .GetComponent<
                    FurnitureData>();


        if (datos == null)
            return;


        datos.Rotar();


        placement
            .RefrescarPosicionDesdeMouse();
    }


    // =====================================================
    // ROTAR MUEBLE YA COLOCADO
    // =====================================================

    public bool RotarSeleccionado()
    {
        if (
            selection == null ||
            selection.muebleSeleccionado == null
        )
        {
            return false;
        }


        GameObject mueble =
            selection.muebleSeleccionado;


        FurnitureData datos =
            mueble.GetComponent<
                FurnitureData>();


        GridObstacle obstaculo =
            mueble.GetComponent<
                GridObstacle>();


        if (
            datos == null ||
            obstaculo == null ||
            obstaculo.grid == null ||
            obstaculo.occupancy == null
        )
        {
            return false;
        }


        GridManager grid =
            obstaculo.grid;


        GridOccupancy occupancy =
            obstaculo.occupancy;


        // =================================================
        // GUARDAR ESTADO ORIGINAL
        // =================================================

        Vector3 posicionOriginal =
            mueble.transform.position;


        Quaternion rotacionOriginal =
            mueble.transform.rotation;


        bool rotadoOriginal =
            datos.rotado;


        // =================================================
        // OBTENER ANCLA ORIGINAL
        // =================================================

        Vector3 puntoAncla =
            posicionOriginal;


        puntoAncla.x -=
            (
                (datos.AnchoActual - 1)
                *
                grid.tamanoCasilla
            )
            / 2f;


        puntoAncla.z -=
            (
                (datos.LargoActual - 1)
                *
                grid.tamanoCasilla
            )
            / 2f;


        if (
            !grid.ObtenerCasilla(
                puntoAncla,
                out Vector2Int ancla
            )
        )
        {
            return false;
        }


        // =================================================
        // LIBERAR CASILLAS ACTUALES TEMPORALMENTE
        // =================================================

        obstaculo
            .LiberarCasillas();


        // =================================================
        // ROTAR LOCALMENTE
        // =================================================

        datos.Rotar();


        bool puedeRotar =
            PuedeOcupar(
                ancla,
                datos,
                grid,
                occupancy
            );


        // =================================================
        // SI NO CABE, VOLVER AL ESTADO ORIGINAL
        // =================================================

        if (!puedeRotar)
        {
            mueble.transform.position =
                posicionOriginal;


            mueble.transform.rotation =
                rotacionOriginal;


            datos.rotado =
                rotadoOriginal;


            obstaculo
                .RegistrarDesdeAncla(
                    ancla
                );


            Debug.Log(
                "No hay espacio para rotar el mueble."
            );


            return false;
        }


        // =================================================
        // RECALCULAR CENTRO SEGUN NUEVAS DIMENSIONES
        // =================================================

        Vector3 centroAncla =
            grid.ObtenerCentroCasilla(
                ancla,
                posicionOriginal.y
            );


        float desplazamientoX =
            (
                (datos.AnchoActual - 1)
                *
                grid.tamanoCasilla
            )
            / 2f;


        float desplazamientoZ =
            (
                (datos.LargoActual - 1)
                *
                grid.tamanoCasilla
            )
            / 2f;


        mueble.transform.position =
            centroAncla +
            new Vector3(
                desplazamientoX,
                0f,
                desplazamientoZ
            );


        // =================================================
        // VOLVER A REGISTRAR CASILLAS
        // =================================================

        if (
            !obstaculo
                .RegistrarDesdeAncla(
                    ancla
                )
        )
        {
            // Seguridad adicional:
            // si el registro falla,
            // restauramos todo.

            mueble.transform.position =
                posicionOriginal;


            mueble.transform.rotation =
                rotacionOriginal;


            datos.rotado =
                rotadoOriginal;


            obstaculo
                .RegistrarDesdeAncla(
                    ancla
                );


            return false;
        }


        // =================================================
        // SI HAY JUGADOR SENTADO, SE ACTUALIZA
        // =================================================

        FurnitureSeat asiento =
            mueble.GetComponent<
                FurnitureSeat>();


        if (
            asiento != null &&
            asiento.EstaOcupado
        )
        {
            asiento
                .SincronizarOcupante();
        }


        Debug.Log(
            "Mueble rotado correctamente."
        );


        // =================================================
        // SINCRONIZAR CON NAKAMA
        // =================================================

        SincronizarRotacion(
            mueble,
            ancla,
            posicionOriginal,
            rotacionOriginal,
            rotadoOriginal
        );


        return true;
    }


    // =====================================================
    // GUARDAR NUEVA ROTACION EN NAKAMA
    // =====================================================

    private async void SincronizarRotacion(
        GameObject mueble,
        Vector2Int ancla,
        Vector3 posicionOriginal,
        Quaternion rotacionOriginal,
        bool rotadoOriginal)
    {
        if (mueble == null)
            return;


        FurnitureInventoryInstance
            identidad =
                mueble.GetComponent<
                    FurnitureInventoryInstance>();


        // Muebles viejos o de prueba
        // pueden no tener identidad Nakama.
        if (
            identidad == null ||
            !identidad.TieneIdentidad
        )
        {
            return;
        }


        FurniturePlacementSyncService
            sync =
                FurniturePlacementSyncService
                    .Instance;


        if (sync == null)
        {
            Debug.LogWarning(
                "No se encontro FurniturePlacementSyncService."
            );

            return;
        }


        int rotacionY =
            Mathf.RoundToInt(
                mueble.transform
                    .eulerAngles.y
            );


        FurniturePlacementSyncResultData
            resultado =
                await sync
                    .GuardarColocacion(
                        identidad.ItemId,
                        ancla,
                        rotacionY
                    );


        if (
            resultado != null &&
            resultado.success
        )
        {
            Debug.Log(
                "ROTACION GUARDADA EN NAKAMA -> " +
                identidad.ItemId +
                " | rotacion " +
                resultado.rotation_y
            );

            return;
        }


        // =================================================
        // SI FALLA NAKAMA, VOLVER AL ESTADO ORIGINAL
        // =================================================

        Debug.LogWarning(
            "No se pudo guardar la rotacion. " +
            "Se restaurara la orientacion anterior."
        );


        GridObstacle obstaculo =
            mueble.GetComponent<
                GridObstacle>();


        FurnitureData datos =
            mueble.GetComponent<
                FurnitureData>();


        if (
            obstaculo == null ||
            datos == null
        )
        {
            return;
        }


        obstaculo
            .LiberarCasillas();


        mueble.transform.position =
            posicionOriginal;


        mueble.transform.rotation =
            rotacionOriginal;


        datos.rotado =
            rotadoOriginal;


        obstaculo
            .RegistrarDesdeAncla(
                ancla
            );


        FurnitureSeat asiento =
            mueble.GetComponent<
                FurnitureSeat>();


        if (
            asiento != null &&
            asiento.EstaOcupado
        )
        {
            asiento
                .SincronizarOcupante();
        }
    }


    // =====================================================
    // VALIDAR ESPACIO
    // =====================================================

    private bool PuedeOcupar(
        Vector2Int ancla,
        FurnitureData datos,
        GridManager grid,
        GridOccupancy occupancy)
    {
        for (
            int x = 0;
            x < datos.AnchoActual;
            x++
        )
        {
            for (
                int z = 0;
                z < datos.LargoActual;
                z++
            )
            {
                Vector2Int casilla =
                    ancla +
                    new Vector2Int(
                        x,
                        z
                    );


                if (
                    casilla.x < 0 ||
                    casilla.x >= grid.ancho ||
                    casilla.y < 0 ||
                    casilla.y >= grid.largo
                )
                {
                    return false;
                }


                if (
                    occupancy
                        .EstaOcupada(
                            casilla
                        )
                )
                {
                    return false;
                }
            }
        }


        return true;
    }
}