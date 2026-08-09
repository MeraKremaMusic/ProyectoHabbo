using System;

[Serializable]
public class WorldTimeData
{
    public double server_unix;
    public double cycle_seconds;
    public double cycle_epoch;
    public double normalized_time;
    public double game_hour;
    public string weather;
}
