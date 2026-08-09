using UnityEngine;

public class FurnitureInventoryInstance :
    MonoBehaviour
{
    [SerializeField]
    private string itemId;

    [SerializeField]
    private string productId;


    public string ItemId
    {
        get
        {
            return itemId;
        }
    }


    public string ProductId
    {
        get
        {
            return productId;
        }
    }


    public bool TieneIdentidad
    {
        get
        {
            return
                !string.IsNullOrWhiteSpace(
                    itemId
                )
                &&
                !string.IsNullOrWhiteSpace(
                    productId
                );
        }
    }


    public void Configurar(
        string nuevoItemId,
        string nuevoProductId)
    {
        itemId =
            nuevoItemId;

        productId =
            nuevoProductId;
    }
}