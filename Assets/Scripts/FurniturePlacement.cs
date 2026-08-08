using UnityEngine;
using UnityEngine.InputSystem;

public class FurniturePlacement : MonoBehaviour
{
    [Header("Referencias")]
    public GridManager grid;
    public GameObject piso;

    [Header("Mueble en colocacion")]
    public GameObject muebleActual;

    [Header("Configuracion")]
    public float alturaSobrePiso = 0.5f;

    private Vector2Int casillaAnclaActual;
    private bool tieneAncla;

    private void Update()
    {
        if (muebleActual == null)
            return;

        SeguirMouse();
    }

    private void SeguirMouse()
    {
        if (Mouse.current == null)
            return;

        Camera camara = Camera.main;

        if (camara == null || grid == null || piso == null)
            return;

        Vector2 posicionMouse =
            Mouse.current.position.ReadValue();

        Ray ray =
            camara.ScreenPointToRay(posicionMouse);

        float alturaPiso =
            piso.transform.position.y;

        Collider colliderPiso =
            piso.GetComponent<Collider>();

        if (colliderPiso != null)
        {
            alturaPiso =
                colliderPiso.bounds.max.y;
        }

        Plane planoPiso =
            new Plane(
                Vector3.up,
                new Vector3(
                    0f,
                    alturaPiso,
                    0f
                )
            );

        if (!planoPiso.Raycast(ray, out float distancia))
            return;

        Vector3 puntoPiso =
            ray.GetPoint(distancia);

        if (
            !grid.ObtenerCasilla(
                puntoPiso,
                out Vector2Int casillaMouse
            )
        )
        {
            return;
        }

        casillaAnclaActual = casillaMouse;
        tieneAncla = true;

        ActualizarPosicionMueble(
            alturaPiso
        );
    }

    private void ActualizarPosicionMueble(
        float alturaPiso)
    {
        if (!tieneAncla || muebleActual == null)
            return;

        FurnitureData datos =
            muebleActual.GetComponent<FurnitureData>();

        int ancho = 1;
        int largo = 1;

        if (datos != null)
        {
            ancho = datos.AnchoActual;
            largo = datos.LargoActual;
        }

        // Esta es SIEMPRE la casilla bajo el mouse.
        Vector3 centroCasillaAncla =
            grid.ObtenerCentroCasilla(
                casillaAnclaActual,
                alturaPiso + alturaSobrePiso
            );

        // El mueble se extiende desde la casilla ancla.
        float desplazamientoX =
            ((ancho - 1) *
            grid.tamanoCasilla) / 2f;

        float desplazamientoZ =
            ((largo - 1) *
            grid.tamanoCasilla) / 2f;

        muebleActual.transform.position =
            centroCasillaAncla +
            new Vector3(
                desplazamientoX,
                0f,
                desplazamientoZ
            );
    }
}