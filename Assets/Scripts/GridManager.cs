using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Configuracion de la cuadricula")]
    public int ancho = 10;
    public int largo = 10;
    public float tamanoCasilla = 1f;

    public float InicioX => -(ancho * tamanoCasilla) / 2f;
    public float InicioZ => -(largo * tamanoCasilla) / 2f;

    public bool ObtenerCasilla(Vector3 posicionMundo, out Vector2Int casilla)
    {
        float xLocal = posicionMundo.x - InicioX;
        float zLocal = posicionMundo.z - InicioZ;

        int x = Mathf.FloorToInt(xLocal / tamanoCasilla);
        int z = Mathf.FloorToInt(zLocal / tamanoCasilla);

        casilla = new Vector2Int(x, z);

        return x >= 0 && x < ancho &&
               z >= 0 && z < largo;
    }

    public Vector3 ObtenerCentroCasilla(Vector2Int casilla, float altura)
    {
        float x =
            InicioX +
            (casilla.x * tamanoCasilla) +
            (tamanoCasilla / 2f);

        float z =
            InicioZ +
            (casilla.y * tamanoCasilla) +
            (tamanoCasilla / 2f);

        return new Vector3(x, altura, z);
    }
}