namespace SPT.Launcher.Models.Fika
{
    public enum EDedicatedStatus {
        READY = 1,
        IN_RAID = 2,
    }
    public class FikaDedicatedData
    {
        public EDedicatedStatus Available { get; set; }
    }
}
