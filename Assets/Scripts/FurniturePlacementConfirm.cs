using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class FurniturePlacementConfirm :
    MonoBehaviour
{
    [Header("Referencias")]
    public FurniturePlacement placement;
    public FurniturePlacementValidator validator;
    public FurniturePreview preview;
    public GridManager grid;
    public GridOccupancy occupancy;
    public FurnitureMove furnitureMove;


    private GameObject muebleDetectado;

    private bool listoParaConfirmar;


    private void Update()
    {
        if (Mouse.current == null)
            return;


        if (
            placement == null ||
            !placement.EstaColocando
        )
        {
            muebleDetectado =
                null;

            listoParaConfirmar =
                false;

            return;
        }


        if (
            muebleDetectado !=
            placement.muebleActual
        )
        {
            muebleDetectado =
                placement.muebleActual;

            listoParaConfirmar =
                false;

            return;
        }


        if (!listoParaConfirmar)
        {
            if (
                !Mouse.current
                    .leftButton
                    .isPressed
            )
            {
                listoParaConfirmar =
                    true;
            }

            return;
        }


        if (
            EventSystem.current != null &&
            EventSystem.current
                .IsPointerOverGameObject()
        )
        {
            return;
        }


        if (
            Mouse.current
                .leftButton
                .wasPressedThisFrame
        )
        {
            IntentarColocar();
        }
    }


    private async void IntentarColocar()
    {
        if (
            placement == null ||
            validator == null
        )
        {
            return;
        }


        placement
            .RefrescarPosicionDesdeMouse();


        if (
            !validator
                .PuedeColocarActual()
        )
        {
            Debug.Log(
                "No se puede colocar el mueble aqui."
            );

            return;
        }


        if (
            !placement
                .ObtenerCasillaAncla(
                    out Vector2Int ancla
                )
        )
        {
            return;
        }


        GameObject mueble =
            placement.muebleActual;


        if (mueble == null)
            return;


        GridObstacle obstaculo =
            mueble.GetComponent<
                GridObstacle>();


        if (obstaculo == null)
        {
            obstaculo =
                mueble.AddComponent<
                    GridObstacle>();
        }


        obstaculo.grid =
            grid;


        obstaculo.occupancy =
            occupancy;


        obstaculo.datos =
            mueble.GetComponent<
                FurnitureData>();


        obstaculo.registrarAlIniciar =
            false;


        obstaculo.enabled =
            true;


        if (
            !obstaculo
                .RegistrarDesdeAncla(
                    ancla
                )
        )
        {
            return;
        }


        if (preview != null)
        {
            preview
                .LimpiarPreview();
        }


        if (furnitureMove != null)
        {
            furnitureMove
                .ConfirmarMovimiento(
                    mueble
                );
        }


        FurnitureInventoryInstance
            identidad =
                mueble.GetComponent<
                    FurnitureInventoryInstance>();


        int rotacionY =
            Mathf.RoundToInt(
                mueble.transform
                    .eulerAngles.y
            );


        placement
            .FinalizarColocacion();


        muebleDetectado =
            null;


        listoParaConfirmar =
            false;


        Debug.Log(
            "Mueble colocado correctamente."
        );


        // Los muebles antiguos o de prueba
        // pueden no tener identidad de Nakama.
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


        FurniturePlacementSyncResultData
            resultado =
                await sync
                    .GuardarColocacion(
                        identidad.ItemId,
                        ancla,
                        rotacionY
                    );


        if (
            resultado == null ||
            !resultado.success
        )
        {
            Debug.LogWarning(
                "El mueble se coloco localmente, " +
                "pero no pudo guardarse en Nakama."
            );
        }
    }
}