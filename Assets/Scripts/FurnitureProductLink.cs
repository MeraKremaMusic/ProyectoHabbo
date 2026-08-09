using UnityEngine;

public class FurnitureProductLink : MonoBehaviour
{
    [Header("Identificador de tienda")]
    [Tooltip("Debe coincidir exactamente con el ID usado por Nakama.")]
    public string productId;


    public bool CoincideCon(
        string id)
    {
        if (
            string.IsNullOrWhiteSpace(productId) ||
            string.IsNullOrWhiteSpace(id)
        )
        {
            return false;
        }

        return productId == id;
    }
}