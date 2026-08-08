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

    public bool EstaMoviendoExistente
    {
        get
        {
            return muebleEnMovimiento != null;
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

        if (Keyboard.current.mKey.wasPressedThisFrame)
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

        if (obstaculo != null)
        {
            obstaculo.LiberarCasillas();
        }

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
            obstaculo.RegistrarDesdePosicionActual();
        }

        LimpiarEstado();

        Debug.Log(
            "Movimiento cancelado. Mueble restaurado."
        );
    }

    public void ConfirmarMovimiento(
        GameObject mueble)
    {
        if (!EsMuebleEnMovimiento(mueble))
            return;

        LimpiarEstado();
    }

    private void LimpiarEstado()
    {
        muebleEnMovimiento = null;
        posicionOriginal = Vector3.zero;
        rotacionOriginal = Quaternion.identity;
        rotadoOriginal = false;
    }
}