namespace SPT.Launcher.Models.Fika
{
    public class FikaPlayer
    {
        public string nickname { get; set; }
        public int level { get; set; }
        public int activity { get; set; }
        public long activityStartedTimeStamp { get; set; }
        public FikaRaid RaidInformation { get; set; }
    }
}
