using System;

[Serializable]
public class FurniturePlacementSyncRequestData
{
    public string item_id;

    public string room_id;

    public int grid_x;

    public int grid_z;

    public int rotation_y;
}