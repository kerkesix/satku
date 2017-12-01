namespace Web.QueryModels
{
    using Newtonsoft.Json;
    using System;
    
    public class DashboardQuit
    {
        [JsonProperty(PropertyName = "t")]
        public DateTimeOffset Timestamp { get; set; }
        
        [JsonProperty(PropertyName = "wsl")]
        public decimal WalkedSinceLastCheckpoint { get; set; }

        // TODO: Do not show for unauthenticated users
        [JsonProperty(PropertyName = "d")]
        public string Description { get; set; }
    }
}