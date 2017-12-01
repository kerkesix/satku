namespace Web.QueryModels
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    
    public class HappeningDashboard
    {
        [JsonProperty(PropertyName = "checkpoints")]
        public List<DashboardCheckpoint> Checkpoints { get; private set; }

        [JsonProperty(PropertyName = "attendees")]
        public List<DashboardAttendee> Attendees { get; private set; }

        [JsonProperty(PropertyName = "path")]
        public string Path { get; set; }

        public HappeningDashboard()
        {
            this.Checkpoints = new List<DashboardCheckpoint>();
            this.Attendees = new List<DashboardAttendee>();
        }

        public void AddCheckpoint(DashboardCheckpoint cp)
        {
            this.Checkpoints.Add(cp);
            this.Checkpoints.Sort((x, y) => x.Order.CompareTo(y.Order));
        }
    }
}