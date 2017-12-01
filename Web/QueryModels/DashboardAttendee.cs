namespace Web.QueryModels
{
    using System;
    using System.Security.Claims;

    using Web.Logic;
    using Newtonsoft.Json;
    
    public class DashboardAttendee : IPersonOwned
    {
        [JsonIgnore()]
        public Person Person { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string PersonId
        {
            get
            {
                return Person != null ? Person.PersonId : null;
            }
        }

        [JsonProperty(PropertyName = "nfc")]
        public string NfcId
        {
            get
            {
                return Person != null ? Person.NfcId : null;
            }
        }
        
        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get
            {
                if (Person == null)
                {
                    return null;
                }

                // For unauthenticated users show Displayname instead of full name.
                // Problem is, that sorting is done by displayname.
                if (ClaimsPrincipal.Current.IsContributor())
                {
                    string name = Person.Lastname + " " + Person.Firstname;

                    return name.Equals(Person.DisplayName, StringComparison.OrdinalIgnoreCase) ?
                        Person.DisplayName : Person.DisplayName + " (" + name + ")";
                }

                return Person.DisplayName;
            }
        }

        [JsonProperty(PropertyName = "phone")]
        public string Phone
        {
            get
            {
                // For unauthenticated users do not show phone
                if (Person == null || !ClaimsPrincipal.Current.IsContributor())
                {
                    return null;
                }

                return Person.Phone;
            }
        }

        [JsonProperty(PropertyName = "dstr")]
        public int DestructionPercent { get; set; }

        [JsonProperty(PropertyName = "hash")]
        public string GravatarHash
        {
            get
            {
                return Person != null ? Person.GravatarHash : null;
            }
        }
    }
}