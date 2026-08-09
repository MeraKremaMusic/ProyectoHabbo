using System;

[Serializable]
public class FurniturePurchaseResultData
{
    public bool success;

    public string code;

    public string message;

    public string product_id;

    public string item_id;

    public long price;

    public long coins;
}