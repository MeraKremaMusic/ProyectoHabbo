using UnityEngine;
using UnityEngine.SceneManagement;

public class FurnitureRoomLoader :
    MonoBehaviour
{
    private PlayerInventoryService
        inventoryService;

    private FurnitureSpawner
        furnitureSpawner;

    private FurniturePlacement
        placement;

    private GridManager
        grid;

    private GridOccupancy
        occupancy;

    private Transform
        contenedorMuebles;


    private bool conectado;

    private bool cargando;


    private string RoomId
    {
        get
        {
            return SceneManager
                .GetActiveScene()
                .name;
        }
    }


    private void Start()
    {
        BuscarReferencias();

        IntentarConectarInventario();
    }


    private void Update()
    {
        if (!conectado)
        {
            IntentarConectarInventario();
        }
    }


    // =====================================================
    // REFERENCIAS
    // =====================================================

    private void BuscarReferencias()
    {
        furnitureSpawner =
            Object.FindAnyObjectByType<
                FurnitureSpawner>();


        if (furnitureSpawner != null)
        {
            placement =
                furnitureSpawner
                    .placement;

            contenedorMuebles =
                furnitureSpawner
                    .contenedorMuebles;
        }


        if (placement != null)
        {
            grid =
                placement.grid;
        }


        if (grid == null)
        {
            grid =
                Object.FindAnyObjectByType<
                    GridManager>();
        }


        occupancy =
            Object.FindAnyObjectByType<
                GridOccupancy>();


        if (contenedorMuebles == null)
        {
            GameObject objetoMuebles =
                GameObject.Find(
                    "Muebles"
                );


            if (objetoMuebles != null)
            {
                contenedorMuebles =
                    objetoMuebles.transform;
            }
        }
    }


    // =====================================================
    // INVENTARIO
    // =====================================================

    private void IntentarConectarInventario()
    {
        if (conectado)
            return;


        inventoryService =
            PlayerInventoryService.Instance;


        if (inventoryService == null)
            return;


        inventoryService
            .InventarioActualizado +=
            AlActualizarInventario;


        conectado =
            true;


        if (
            inventoryService
                .InventarioCargado
        )
        {
            CargarMueblesGuardados();
        }
    }


    private void AlActualizarInventario()
    {
        CargarMueblesGuardados();
    }


    // =====================================================
    // CARGAR HABITACION
    // =====================================================

    public void CargarMueblesGuardados()
    {
        if (cargando)
            return;


        if (
            inventoryService == null ||
            !inventoryService
                .InventarioCargado
        )
        {
            return;
        }


        if (
            grid == null ||
            occupancy == null
        )
        {
            BuscarReferencias();
        }


        if (
            grid == null ||
            occupancy == null
        )
        {
            Debug.LogError(
                "FurnitureRoomLoader: faltan GridManager o GridOccupancy."
            );

            return;
        }


        cargando =
            true;


        try
        {
            FurnitureInventoryItemData[]
                items =
                    inventoryService.Items;


            if (items == null)
                return;


            int cargados =
                0;


            foreach (
                FurnitureInventoryItemData item
                in items
            )
            {
                if (item == null)
                    continue;


                if (!item.placed)
                    continue;


                if (
                    item.room_id !=
                    RoomId
                )
                {
                    continue;
                }


                if (
                    ExisteEnHabitacion(
                        item.item_id
                    )
                )
                {
                    continue;
                }


                if (
                    CrearMueblePersistido(
                        item
                    )
                )
                {
                    cargados++;
                }
            }


            Debug.Log(
                "HABITACION CARGADA -> " +
                cargados +
                " mueble(s) restaurado(s)"
            );
        }
        finally
        {
            cargando =
                false;
        }
    }


    // =====================================================
    // CREAR MUEBLE
    // =====================================================

    private bool CrearMueblePersistido(
        FurnitureInventoryItemData item)
    {
        if (
            string.IsNullOrWhiteSpace(
                item.item_id
            )
            ||
            string.IsNullOrWhiteSpace(
                item.product_id
            )
        )
        {
            return false;
        }


        GameObject prefab =
            FurniturePrefabResolver
                .ObtenerPrefab(
                    item.product_id
                );


        if (prefab == null)
        {
            Debug.LogWarning(
                "No se pudo restaurar " +
                item.item_id +
                ". No existe prefab para " +
                item.product_id
            );

            return false;
        }


        GameObject mueble;


        if (contenedorMuebles != null)
        {
            mueble =
                Instantiate(
                    prefab,
                    contenedorMuebles
                );
        }
        else
        {
            mueble =
                Instantiate(
                    prefab
                );
        }


        // =================================================
        // IDENTIDAD
        // =================================================

        FurnitureInventoryInstance
            identidad =
                mueble.GetComponent<
                    FurnitureInventoryInstance>();


        if (identidad == null)
        {
            identidad =
                mueble.AddComponent<
                    FurnitureInventoryInstance>();
        }


        identidad.Configurar(
            item.item_id,
            item.product_id
        );


        // =================================================
        // ROTACION
        // =================================================

        int rotacion =
            NormalizarRotacion(
                item.rotation_y
            );


        mueble.transform.rotation =
            Quaternion.Euler(
                0f,
                rotacion,
                0f
            );


        FurnitureData datos =
            mueble.GetComponent<
                FurnitureData>();


        if (datos == null)
        {
            Debug.LogError(
                "El prefab " +
                prefab.name +
                " no tiene FurnitureData."
            );

            Destroy(
                mueble
            );

            return false;
        }


        datos.rotado =
            rotacion == 90 ||
            rotacion == 270;


        // =================================================
        // CASILLA
        // =================================================

        Vector2Int ancla =
            new Vector2Int(
                item.grid_x,
                item.grid_z
            );


        if (
            !PuedeRegistrar(
                ancla,
                datos
            )
        )
        {
            Debug.LogWarning(
                "No se pudo restaurar " +
                item.item_id +
                ". La posicion esta ocupada o fuera de la cuadricula."
            );


            Destroy(
                mueble
            );

            return false;
        }


        PosicionarEnCasilla(
            mueble,
            ancla,
            datos
        );


        // =================================================
        // OCUPACION DE GRID
        // =================================================

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
            datos;


        obstaculo.registrarAlIniciar =
            false;


        if (
            !obstaculo
                .RegistrarDesdeAncla(
                    ancla
                )
        )
        {
            Destroy(
                mueble
            );

            return false;
        }


        Debug.Log(
            "MUEBLE RESTAURADO -> " +
            item.item_id +
            " | " +
            item.product_id +
            " | (" +
            item.grid_x +
            ", " +
            item.grid_z +
            ")" +
            " | rotacion " +
            rotacion
        );


        return true;
    }


    // =====================================================
    // POSICION
    // =====================================================

    private void PosicionarEnCasilla(
        GameObject mueble,
        Vector2Int ancla,
        FurnitureData datos)
    {
        Vector3 centro =
            grid.ObtenerCentroCasilla(
                ancla,
                mueble
                    .transform
                    .position
                    .y
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


        Vector3 posicion =
            mueble.transform.position;


        posicion.x =
            centro.x +
            desplazamientoX;


        posicion.z =
            centro.z +
            desplazamientoZ;


        mueble.transform.position =
            posicion;


        AjustarAlturaAlPiso(
            mueble
        );
    }


    // =====================================================
    // ALTURA
    // =====================================================

    private void AjustarAlturaAlPiso(
        GameObject mueble)
    {
        if (
            mueble == null ||
            placement == null ||
            placement.piso == null
        )
        {
            return;
        }


        float alturaPiso =
            placement
                .piso
                .transform
                .position
                .y;


        Collider colliderPiso =
            placement
                .piso
                .GetComponent<
                    Collider>();


        if (colliderPiso != null)
        {
            alturaPiso =
                colliderPiso
                    .bounds
                    .max
                    .y;
        }


        Renderer[] renderers =
            mueble
                .GetComponentsInChildren<
                    Renderer>();


        if (
            renderers == null ||
            renderers.Length == 0
        )
        {
            Vector3 posicion =
                mueble.transform.position;


            posicion.y =
                alturaPiso +
                placement
                    .separacionPiso;


            mueble.transform.position =
                posicion;


            return;
        }


        bool encontro =
            false;


        Bounds bounds =
            new Bounds();


        foreach (
            Renderer renderer
            in renderers
        )
        {
            if (
                renderer == null ||
                !renderer.enabled
            )
            {
                continue;
            }


            if (!encontro)
            {
                bounds =
                    renderer.bounds;

                encontro =
                    true;
            }
            else
            {
                bounds.Encapsulate(
                    renderer.bounds
                );
            }
        }


        if (!encontro)
            return;


        float alturaObjetivo =
            alturaPiso +
            placement
                .separacionPiso;


        float diferencia =
            alturaObjetivo -
            bounds.min.y;


        Vector3 posicionActual =
            mueble.transform.position;


        posicionActual.y +=
            diferencia;


        mueble.transform.position =
            posicionActual;
    }


    // =====================================================
    // VALIDACION GRID
    // =====================================================

    private bool PuedeRegistrar(
        Vector2Int ancla,
        FurnitureData datos)
    {
        if (
            datos == null ||
            grid == null ||
            occupancy == null
        )
        {
            return false;
        }


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
                    casilla.x >=
                    grid.ancho ||
                    casilla.y < 0 ||
                    casilla.y >=
                    grid.largo
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


    // =====================================================
    // EVITAR DUPLICADOS
    // =====================================================

    private bool ExisteEnHabitacion(
        string itemId)
    {
        if (
            string.IsNullOrWhiteSpace(
                itemId
            )
        )
        {
            return false;
        }


        if (contenedorMuebles == null)
            return false;


        FurnitureInventoryInstance[]
            muebles =
                contenedorMuebles
                    .GetComponentsInChildren<
                        FurnitureInventoryInstance>(
                        true
                    );


        foreach (
            FurnitureInventoryInstance mueble
            in muebles
        )
        {
            if (
                mueble != null &&
                mueble.ItemId ==
                itemId
            )
            {
                return true;
            }
        }


        return false;
    }


    // =====================================================
    // ROTACION
    // =====================================================

    private int NormalizarRotacion(
        int rotacion)
    {
        rotacion %=
            360;


        if (rotacion < 0)
        {
            rotacion +=
                360;
        }


        return
            Mathf.RoundToInt(
                rotacion / 90f
            )
            *
            90
            %
            360;
    }


    // =====================================================
    // LIMPIEZA
    // =====================================================

    private void OnDestroy()
    {
        if (
            inventoryService != null &&
            conectado
        )
        {
            inventoryService
                .InventarioActualizado -=
                AlActualizarInventario;
        }
    }
}