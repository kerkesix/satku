namespace Web.QueryModels
{
    using Newtonsoft.Json;
    using System;
    
    public class DashboardVisit : IPersonOwned
    {
        [JsonProperty(PropertyName = "a")]
        public string PersonId { get; set; }

        [JsonProperty(PropertyName = "i")]
        public DateTimeOffset? TimeIn { get; set; }

        [JsonProperty(PropertyName = "o")]
        public DateTimeOffset? TimeOut { get; set; }

        [JsonProperty(PropertyName = "quit")]
        public DashboardQuit Quit { get; set; }
        
        [JsonIgnore]
        public double Duration
        {
            get
            {
                if (!this.TimeIn.HasValue || !this.TimeOut.HasValue)
                {
                    return 0;
                }

                return (this.TimeOut.Value - this.TimeIn.Value).TotalSeconds;
            }
        }
    }
}