namespace SPT.Launcher.Models.Fika
{
    public interface IDedicatedStatus
    {
        string HeadlessSessionID { get; }
        string Alias { get; }
    }

    public class DedicatedStatus : IDedicatedStatus
    {
        // Property names must match the JSON (case-sensitive)
        public string headlessSessionID { get; set; } = string.Empty;
        public string alias { get; set; } = string.Empty;

        // Implement the interface properties
        public string HeadlessSessionID => headlessSessionID;
        public string Alias => alias;
    }

    public class FikaDedicatedData
    {
        public IDedicatedStatus Available { get; set; } = null!;
    }
}