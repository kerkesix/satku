namespace Web.QueryModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using KsxEventTracker.Domain.Aggregates.Checkpoint;
    using Newtonsoft.Json;

    public class DashboardCheckpoint
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        
        [JsonProperty(PropertyName = "distanceFromPrevious")]
        public decimal DistanceFromPrevious { get; set; }
        
        [JsonProperty(PropertyName = "distanceFromStart")]
        public decimal DistanceFromStart { get; set; }
        
        [JsonProperty(PropertyName = "latitude")]
        public double Latitude { get; set; }
        
        [JsonProperty(PropertyName = "longitude")]
        public double Longitude { get; set; }
        public int Order { get; set; }

        [JsonProperty(PropertyName = "visits")]
        public List<DashboardVisit> Visits{ get; private set; }

        [JsonProperty(PropertyName = "waitingFor")]
        public Dictionary<string, DateTimeOffset> Expected { get; private set; }

        [JsonProperty(PropertyName = "checkpointType")]
        public CheckpointType CheckpointType { get; set; }

        public DashboardCheckpoint()
        {
            this.Visits = new List<DashboardVisit>();
            this.Expected = new Dictionary<string, DateTimeOffset>();
        }

        public void AddExpected(string personId, DateTimeOffset expectedAt)
        {
            this.Expected[personId] = expectedAt;

            // TODO: Sort by expected time ascending.
        }
    }
}