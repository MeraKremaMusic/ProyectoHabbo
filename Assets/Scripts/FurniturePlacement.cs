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
    [Tooltip(
        "Pequena separacion opcional entre " +
        "el mueble y el piso."
    )]
    public float separacionPiso = 0f;

    private Vector2Int casillaAnclaActual;
    private bool tieneAncla;

    private int frameUltimaColocacion = -1;

    public bool EstaColocando
    {
        get
        {
            return muebleActual != null;
        }
    }

    public bool BloquearSeleccionJugador
    {
        get
        {
            return
                EstaColocando ||
                Time.frameCount ==
                frameUltimaColocacion;
        }
    }

    private void Update()
    {
        if (!EstaColocando)
            return;

        RefrescarPosicionDesdeMouse();
    }

    public void RefrescarPosicionDesdeMouse()
    {
        if (Mouse.current == null)
            return;

        Camera camara =
            Camera.main;

        if (
            camara == null ||
            grid == null ||
            piso == null
        )
        {
            return;
        }

        Vector2 posicionMouse =
            Mouse.current.position.ReadValue();

        Ray ray =
            camara.ScreenPointToRay(
                posicionMouse
            );

        float alturaPiso =
            ObtenerAlturaPiso();

        Plane planoPiso =
            new Plane(
                Vector3.up,
                new Vector3(
                    0f,
                    alturaPiso,
                    0f
                )
            );

        if (
            !planoPiso.Raycast(
                ray,
                out float distancia
            )
        )
        {
            return;
        }

        Vector3 puntoPiso =
            ray.GetPoint(distancia);

        if (
            !grid.ObtenerCasilla(
                puntoPiso,
                out Vector2Int casillaMouse
            )
        )
        {
            tieneAncla = false;
            return;
        }

        casillaAnclaActual =
            casillaMouse;

        tieneAncla =
            true;

        ActualizarPosicionMueble(
            alturaPiso
        );
    }

    public bool ObtenerCasillaAncla(
        out Vector2Int casilla)
    {
        casilla =
            casillaAnclaActual;

        return tieneAncla;
    }

    public void FinalizarColocacion()
    {
        frameUltimaColocacion =
            Time.frameCount;

        muebleActual =
            null;

        tieneAncla =
            false;
    }

    private void ActualizarPosicionMueble(
        float alturaPiso)
    {
        if (
            !tieneAncla ||
            muebleActual == null
        )
        {
            return;
        }

        FurnitureData datos =
            muebleActual
                .GetComponent<FurnitureData>();

        int ancho = 1;
        int largo = 1;

        if (datos != null)
        {
            ancho =
                datos.AnchoActual;

            largo =
                datos.LargoActual;
        }

        // Primero posicionamos correctamente
        // el mueble en X y Z.
        Vector3 centroCasillaAncla =
            grid.ObtenerCentroCasilla(
                casillaAnclaActual,
                muebleActual
                    .transform.position.y
            );

        float desplazamientoX =
            ((ancho - 1) *
            grid.tamanoCasilla) / 2f;

        float desplazamientoZ =
            ((largo - 1) *
            grid.tamanoCasilla) / 2f;

        Vector3 posicion =
            muebleActual.transform.position;

        posicion.x =
            centroCasillaAncla.x +
            desplazamientoX;

        posicion.z =
            centroCasillaAncla.z +
            desplazamientoZ;

        muebleActual.transform.position =
            posicion;

        // Después calculamos automáticamente
        // dónde está la parte más baja
        // del modelo 3D y la apoyamos
        // exactamente sobre el piso.
        AjustarAlturaAlPiso(
            alturaPiso
        );
    }

    private void AjustarAlturaAlPiso(
        float alturaPiso)
    {
        if (muebleActual == null)
            return;

        Renderer[] renderers =
            muebleActual
                .GetComponentsInChildren<Renderer>();

        if (
            renderers == null ||
            renderers.Length == 0
        )
        {
            // Si por alguna razón el objeto
            // no tiene Renderer, simplemente
            // usamos la altura del piso.
            Vector3 posicion =
                muebleActual.transform.position;

            posicion.y =
                alturaPiso +
                separacionPiso;

            muebleActual.transform.position =
                posicion;

            return;
        }

        bool encontroRenderer =
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

            if (!encontroRenderer)
            {
                bounds =
                    renderer.bounds;

                encontroRenderer =
                    true;
            }
            else
            {
                bounds.Encapsulate(
                    renderer.bounds
                );
            }
        }

        if (!encontroRenderer)
            return;

        float parteMasBaja =
            bounds.min.y;

        float alturaObjetivo =
            alturaPiso +
            separacionPiso;

        float diferencia =
            alturaObjetivo -
            parteMasBaja;

        Vector3 posicionActual =
            muebleActual.transform.position;

        posicionActual.y +=
            diferencia;

        muebleActual.transform.position =
            posicionActual;
    }

    private float ObtenerAlturaPiso()
    {
        float alturaPiso =
            piso.transform.position.y;

        Collider colliderPiso =
            piso.GetComponent<Collider>();

        if (colliderPiso != null)
        {
            alturaPiso =
                colliderPiso.bounds.max.y;
        }

        return alturaPiso;
    }
}