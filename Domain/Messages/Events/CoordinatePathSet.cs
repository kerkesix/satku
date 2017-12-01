namespace KsxEventTracker.Domain.Messages.Events
{
    public class CoordinatePathSet : EventBase, IEvent
    {
        public string HappeningId { get; set; }

        public string Path { get; set; }

        public CoordinatePathSet()
        {
        }

        public CoordinatePathSet(string happeningId, string path)
        {
            this.HappeningId = happeningId;
            this.Path = path;
        }
    }
}