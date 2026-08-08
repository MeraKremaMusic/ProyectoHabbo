using UnityEngine;
using UnityEngine.InputSystem;

public class FurnitureRotation : MonoBehaviour
{
    [Header("Referencias")]
    public FurniturePlacement placement;
    public FurnitureSelection selection;

    private void Awake()
    {
        if (selection == null)
        {
            selection =
                GetComponent<FurnitureSelection>();
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (!Keyboard.current.rKey.wasPressedThisFrame)
            return;

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
                .GetComponent<FurnitureData>();

        if (datos == null)
            return;

        datos.Rotar();

        placement.RefrescarPosicionDesdeMouse();
    }

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
            mueble.GetComponent<FurnitureData>();

        GridObstacle obstaculo =
            mueble.GetComponent<GridObstacle>();

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

        Vector3 posicionOriginal =
            mueble.transform.position;

        Quaternion rotacionOriginal =
            mueble.transform.rotation;

        bool rotadoOriginal =
            datos.rotado;

        // Calculamos la casilla ancla ANTES de rotar.
        Vector3 puntoAncla =
            posicionOriginal;

        puntoAncla.x -=
            ((datos.AnchoActual - 1) *
            grid.tamanoCasilla) / 2f;

        puntoAncla.z -=
            ((datos.LargoActual - 1) *
            grid.tamanoCasilla) / 2f;

        if (
            !grid.ObtenerCasilla(
                puntoAncla,
                out Vector2Int ancla
            )
        )
        {
            return false;
        }

        // Liberamos temporalmente su espacio.
        obstaculo.LiberarCasillas();

        datos.Rotar();

        bool puedeRotar =
            PuedeOcupar(
                ancla,
                datos,
                grid,
                occupancy
            );

        if (!puedeRotar)
        {
            // No cabe: restauramos todo.
            mueble.transform.position =
                posicionOriginal;

            mueble.transform.rotation =
                rotacionOriginal;

            datos.rotado =
                rotadoOriginal;

            obstaculo.RegistrarDesdeAncla(
                ancla
            );

            Debug.Log(
                "No hay espacio para rotar el mueble."
            );

            return false;
        }

        Vector3 centroAncla =
            grid.ObtenerCentroCasilla(
                ancla,
                posicionOriginal.y
            );

        float desplazamientoX =
            ((datos.AnchoActual - 1) *
            grid.tamanoCasilla) / 2f;

        float desplazamientoZ =
            ((datos.LargoActual - 1) *
            grid.tamanoCasilla) / 2f;

        mueble.transform.position =
            centroAncla +
            new Vector3(
                desplazamientoX,
                0f,
                desplazamientoZ
            );

        obstaculo.RegistrarDesdeAncla(
            ancla
        );

        Debug.Log(
            "Mueble rotado correctamente."
        );

        return true;
    }

    private bool PuedeOcupar(
        Vector2Int ancla,
        FurnitureData datos,
        GridManager grid,
        GridOccupancy occupancy)
    {
        for (int x = 0; x < datos.AnchoActual; x++)
        {
            for (int z = 0; z < datos.LargoActual; z++)
            {
                Vector2Int casilla =
                    ancla +
                    new Vector2Int(x, z);

                if (
                    casilla.x < 0 ||
                    casilla.x >= grid.ancho ||
                    casilla.y < 0 ||
                    casilla.y >= grid.largo
                )
                {
                    return false;
                }

                if (occupancy.EstaOcupada(casilla))
                {
                    return false;
                }
            }
        }

        return true;
    }
}