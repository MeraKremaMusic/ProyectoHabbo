using System;

[Serializable]
public class FurnitureInventoryItemData
{
    public string item_id;

    public string product_id;

    public string name;

    public string category;

    public long acquired_at;

    public string source;

    public bool placed;

    public string room_id;

    public int grid_x;

    public int grid_z;

    public int rotation_y;
}