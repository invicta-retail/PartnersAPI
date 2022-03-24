using Newtonsoft.Json;

namespace InvictaPartnersAPI.Models
{
    public class User
    {
        
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string type {get;set;}

        
        [JsonProperty(PropertyName = "firstName")]
        public string firstName { get; set; }

        
        [JsonProperty(PropertyName = "lastName")]
        public string lastName { get; set; }
        
        [JsonProperty(PropertyName = "userName")]
        public string userName { get; set; }


        [JsonProperty(PropertyName = "password")]
        //[Newtonsoft.Json.JsonIgnore]
        public string password { get; set; }

        [JsonProperty(PropertyName = "role")]
        public string role { get; set; }


        [JsonProperty(PropertyName = "defaulShippingAddress")]
        public bool defaulShippingAddress {get;set;}

        [JsonProperty(PropertyName = "customerId")]
        public string customerId { get; set; }

        [JsonProperty(PropertyName = "company")]

        public string company{get;set;}


        [JsonProperty(PropertyName = "addressLine1")]

        public string addressLine1{get;set;}

        [JsonProperty(PropertyName = "addressLine2")]


        public string addressLine2{get;set;}

        [JsonProperty(PropertyName = "city")]


        public string city {get;set;}

        [JsonProperty(PropertyName = "state")]

        public string state{get;set;}

        [JsonProperty(PropertyName = "postalCode")]

        public string postalCode{get;set;}

        [JsonProperty(PropertyName = "countryCode")]


        public string countryCode{get;set;}

        [JsonProperty(PropertyName = "phone")]

        public string phone {get;set;}

        [JsonProperty(PropertyName = "email")]

        public string email{get;set;}
    }
}