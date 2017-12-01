using Newtonsoft.Json;

namespace Web.QueryModels
{
   
    public interface IPersonOwned
    {
        [JsonProperty(PropertyName = "id")]
        string PersonId { get; }
    }
}