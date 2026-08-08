using UnityEngine;

public class FurnitureCatalog : MonoBehaviour
{
    [Header("Muebles disponibles")]
    public GameObject[] muebles;

    public GameObject ObtenerMueble(int indice)
    {
        if (
            indice < 0 ||
            indice >= muebles.Length
        )
        {
            return null;
        }

        return muebles[indice];
    }
}