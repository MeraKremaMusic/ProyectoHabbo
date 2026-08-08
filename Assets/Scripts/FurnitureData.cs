using UnityEngine;

public class FurnitureData : MonoBehaviour
{
    [Header("Tamaño en casillas")]
    [Min(1)]
    public int ancho = 1;

    [Min(1)]
    public int largo = 1;

    [Header("Estado")]
    public bool rotado = false;

    public int AnchoActual
    {
        get
        {
            return rotado ? largo : ancho;
        }
    }

    public int LargoActual
    {
        get
        {
            return rotado ? ancho : largo;
        }
    }

    public void Rotar()
    {
        rotado = !rotado;

        transform.Rotate(
            0f,
            90f,
            0f
        );
    }
}