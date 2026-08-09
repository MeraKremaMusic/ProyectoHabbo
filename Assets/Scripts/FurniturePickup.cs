using UnityEngine;

public class FurniturePickup :
    MonoBehaviour
{
    public FurnitureSelection selection;

    public FurniturePlacement placement;


    public bool Recogiendo
    {
        get;
        private set;
    }


    private void Awake()
    {
        if (selection == null)
        {
            selection =
                GetComponent<
                    FurnitureSelection>();
        }


        if (placement == null)
        {
            placement =
                GetComponent<
                    FurniturePlacement>();
        }
    }


    public async void RecogerSeleccionado()
    {
        if (Recogiendo)
            return;


        if (
            placement != null &&
            placement.EstaColocando
        )
        {
            return;
        }


        if (selection == null)
            return;


        GameObject mueble =
            selection.muebleSeleccionado;


        if (mueble == null)
            return;


        FurnitureInventoryInstance identidad =
            mueble.GetComponent<
                FurnitureInventoryInstance>();


        if (
            identidad == null ||
            !identidad.TieneIdentidad
        )
        {
            Debug.LogWarning(
                "Este mueble no pertenece al inventario de Nakama."
            );

            return;
        }


        FurniturePickupService servicio =
            FurniturePickupService.Instance;


        if (servicio == null)
        {
            Debug.LogError(
                "FurniturePickupService no esta disponible."
            );

            return;
        }


        Recogiendo =
            true;


        try
        {
            string itemId =
                identidad.ItemId;


            FurniturePickupResultData resultado =
                await servicio
                    .Recoger(
                        itemId
                    );


            // IMPORTANTE:
            // No eliminamos nada localmente
            // hasta recibir OK de Nakama.
            if (
                resultado == null ||
                !resultado.success
            )
            {
                return;
            }


            if (mueble != null)
            {
                FurnitureSeat asiento =
                    mueble.GetComponent<
                        FurnitureSeat>();


                if (
                    asiento != null &&
                    asiento.EstaOcupado
                )
                {
                    asiento
                        .LevantarOcupanteEnPosicionDelMueble();
                }


                GridObstacle obstaculo =
                    mueble.GetComponent<
                        GridObstacle>();


                if (obstaculo != null)
                {
                    obstaculo
                        .LiberarCasillas();
                }


                if (
                    selection
                        .muebleSeleccionado ==
                    mueble
                )
                {
                    selection
                        .muebleSeleccionado =
                        null;
                }


                // Lo ocultamos inmediatamente.
                // Esto también evita que la UI
                // lo detecte como existente
                // mientras se refresca el inventario.
                mueble.SetActive(
                    false
                );


                Destroy(
                    mueble
                );
            }


            if (
                PlayerInventoryService
                    .Instance != null
            )
            {
                await PlayerInventoryService
                    .Instance
                    .CargarInventario();
            }


            Debug.Log(
                "MUEBLE DEVUELTO AL INVENTARIO -> " +
                itemId
            );
        }
        finally
        {
            Recogiendo =
                false;
        }
    }
}