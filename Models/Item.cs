using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace InvictaPartnersAPI.Models
{

    public class Item    
    {

        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string type { get; set; }  

        [JsonProperty(PropertyName = "sku")]
        public string sku { get; set; }

        [JsonProperty(PropertyName = "active")]
        public bool active { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "totalAvailableQuantity")]
        public int totalAvailableQuantity { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string currency { get; set; }

        [JsonProperty(PropertyName = "unitCost")]
        public double unitCost { get; set; }

        [JsonProperty(PropertyName = "sourceCode")]
        public string sourceCode { get; set; }
        
        [JsonProperty(PropertyName = "_ts")]
        public Int32 _ts { get; set; }
        
    }

    public class PriceEntry {
        public string id { get; set; }
        public string type { get; set; }  
        public string sku {get;set;}
        public string validDate {get;set;}
        public double unitCost {get;set;}

    }

    public class ItemCommitment {
        public string sku{get;set;}

        public int commited {get;set;}
    }

    public class SellwareProduct {
        public List<SellwareItem> product {get;set;}
        
        public int pageNumber{get;set;}
        
        public int pageSize{get;set;}

        public int totalResults{get;set;}

    }
    public class SellwareItem    
    {
        
        [JsonProperty(PropertyName = "sku")]
        public string sku {get;set;}


        [JsonProperty(PropertyName = "quantity")]
        public int quantity {get;set;}

        
        [JsonProperty(PropertyName = "status")]

        public int status{get;set;}

    }


}