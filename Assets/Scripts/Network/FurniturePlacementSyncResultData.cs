using System;

[Serializable]
public class FurniturePlacementSyncResultData
{
    public bool success;

    public string code;

    public string message;

    public string item_id;

    public bool placed;

    public string room_id;

    public int grid_x;

    public int grid_z;

    public int rotation_y;
}