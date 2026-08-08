using UnityEngine;
using UnityEngine.InputSystem;

public class FurnitureMove : MonoBehaviour
{
    [Header("Referencias")]
    public FurnitureSelection selection;
    public FurniturePlacement placement;

    private GameObject muebleEnMovimiento;

    private Vector3 posicionOriginal;
    private Quaternion rotacionOriginal;
    private bool rotadoOriginal;

    // Si alguien estaba sentado cuando
    // empezamos a mover la silla, lo guardamos.
    // Esto nos permite devolverlo al asiento
    // si el usuario cancela con ESC.
    private PlayerSitting ocupanteLevantadoAlMover;

    public bool EstaMoviendoExistente
    {
        get
        {
            return
                muebleEnMovimiento != null;
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (
            selection == null ||
            placement == null
        )
        {
            return;
        }

        if (placement.EstaColocando)
            return;

        if (
            Keyboard.current.mKey
                .wasPressedThisFrame
        )
        {
            MoverSeleccionado();
        }
    }

    public void MoverSeleccionado()
    {
        GameObject mueble =
            selection.muebleSeleccionado;

        if (mueble == null)
            return;

        FurnitureData datos =
            mueble.GetComponent<FurnitureData>();

        GridObstacle obstaculo =
            mueble.GetComponent<GridObstacle>();

        FurnitureSeat asiento =
            mueble.GetComponent<FurnitureSeat>();

        // Guardamos el estado ORIGINAL
        // antes de tocar el mueble.
        muebleEnMovimiento =
            mueble;

        posicionOriginal =
            mueble.transform.position;

        rotacionOriginal =
            mueble.transform.rotation;

        if (datos != null)
        {
            rotadoOriginal =
                datos.rotado;
        }

        ocupanteLevantadoAlMover =
            null;

        // Si la silla estaba ocupada,
        // primero ponemos al personaje
        // de pie donde estaba la silla.
        if (
            asiento != null &&
            asiento.EstaOcupado
        )
        {
            ocupanteLevantadoAlMover =
                asiento
                    .LevantarOcupanteEnPosicionDelMueble();
        }

        // Ahora liberamos las casillas.
        if (obstaculo != null)
        {
            obstaculo.LiberarCasillas();
        }

        // El mueble pasa al cursor.
        placement.muebleActual =
            mueble;

        selection.muebleSeleccionado =
            null;

        placement.RefrescarPosicionDesdeMouse();

        Debug.Log(
            "Moviendo mueble: " +
            mueble.name
        );
    }

    public bool EsMuebleEnMovimiento(
        GameObject mueble)
    {
        return
            muebleEnMovimiento != null &&
            muebleEnMovimiento == mueble;
    }

    public void CancelarMovimiento()
    {
        if (muebleEnMovimiento == null)
            return;

        GameObject mueble =
            muebleEnMovimiento;

        // Devolvemos el mueble
        // exactamente a su estado original.
        mueble.transform.position =
            posicionOriginal;

        mueble.transform.rotation =
            rotacionOriginal;

        FurnitureData datos =
            mueble.GetComponent<FurnitureData>();

        if (datos != null)
        {
            datos.rotado =
                rotadoOriginal;
        }

        GridObstacle obstaculo =
            mueble.GetComponent<GridObstacle>();

        if (obstaculo != null)
        {
            obstaculo
                .RegistrarDesdePosicionActual();
        }

        // Si alguien estaba sentado antes
        // de comenzar a moverlo, y cancelamos,
        // vuelve automáticamente a sentarse.
        if (
            ocupanteLevantadoAlMover != null
        )
        {
            FurnitureSeat asiento =
                mueble.GetComponent<FurnitureSeat>();

            if (asiento != null)
            {
                ocupanteLevantadoAlMover
                    .SentarseDirectamente(
                        asiento
                    );
            }
        }

        LimpiarEstado();

        Debug.Log(
            "Movimiento cancelado. " +
            "Mueble restaurado."
        );
    }

    public void ConfirmarMovimiento(
        GameObject mueble)
    {
        if (!EsMuebleEnMovimiento(mueble))
            return;

        // Al confirmar, el personaje
        // permanece de pie donde estaba
        // originalmente la silla.
        LimpiarEstado();

        Debug.Log(
            "Nueva posicion del mueble confirmada."
        );
    }

    private void LimpiarEstado()
    {
        muebleEnMovimiento =
            null;

        posicionOriginal =
            Vector3.zero;

        rotacionOriginal =
            Quaternion.identity;

        rotadoOriginal =
            false;

        ocupanteLevantadoAlMover =
            null;
    }
}