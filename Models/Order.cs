using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace InvictaPartnersAPI.Models
{


    public class SSWebHook
    {
        public string resource_url { get; set; }
        public string resource_type { get; set; }
    }


    public class TimeStamp 
    {
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string type { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public Int32 timestamp { get; set; }

    }


    public class Sequence 
    {
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string type { get; set; }

        [JsonProperty(PropertyName = "sequence")]
        public long sequence { get; set; }

    }

    public class PackageItem
    {

        [JsonProperty(PropertyName = "sku")]
        public string sku { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }


        [JsonProperty(PropertyName = "quantity")]
        public int quantity { get; set; }
    }
    public class Package
    {

        [JsonProperty(PropertyName = "packageId")]
        public Int32 packageId { get; set; }

        [JsonProperty(PropertyName = "carrier")]
        public string carrier { get; set; }
        [JsonProperty(PropertyName = "serviceLevel")]
        public string serviceLevel { get; set; }
        [JsonProperty(PropertyName = "cost")]
        public double cost { get; set; }
        [JsonProperty(PropertyName = "tracking")]
        public string tracking { get; set; }
        [JsonProperty(PropertyName = "shippedDate")]
        public string shippedDate { get; set; }
        [JsonProperty(PropertyName = "products")]
        public List<PackageItem> products { get; set; }
    }

    public class Address
    {
        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "company")]
        public string company { get; set; }

        [JsonProperty(PropertyName = "addressLine1")]
        public string addressLine1 { get; set; }

        [JsonProperty(PropertyName = "addressLine2")]
        public string addressLine2 { get; set; }

        [JsonProperty(PropertyName = "city")]
        public string city { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string state { get; set; }

        [JsonProperty(PropertyName = "postalCode")]
        public string postalCode { get; set; }

        [JsonProperty(PropertyName = "phone")]
        public string phone { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string email { get; set; }

        [JsonProperty(PropertyName = "countryCode")]
        public string countryCode { get; set; }


    }

    public class OrderItem
    {

        [JsonProperty(PropertyName = "sku")]
        public string sku { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string status { get; set; }

        [JsonProperty(PropertyName = "quantityOrdered")]
        public int quantityOrdered { get; set; }
        [JsonProperty(PropertyName = "quantityShip")]
        public int quantityShip { get; set; }

        [JsonProperty(PropertyName = "quantityCanceled")]
        public int quantityCanceled { get; set; }

        [JsonProperty(PropertyName = "unitPrice")]
        public double unitPrice { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string currency { get; set; }

    }

    public class Order
    {

        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string type { get; set; }

        [JsonProperty(PropertyName = "orderNumber")]
        public string orderNumber { get; set; }

        [JsonProperty(PropertyName = "customerId")]
        public string customerId { get; set; }

        [JsonProperty(PropertyName = "customerName")]
        public string customerName { get; set; }

        [JsonProperty(PropertyName = "customerEmail")]
        public string customerEmail { get; set; }

        [JsonProperty(PropertyName = "customerRefNumber")]
        public string customerRefNumber { get; set; }

        [JsonProperty(PropertyName = "requestedServiceLevel")]
        public string requestedServiceLevel { get; set; }

        [JsonProperty(PropertyName = "orderDate")]
        public DateTime orderDate { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string status { get; set; }

        [JsonProperty(PropertyName = "shippingAddress")]
        public Address shippingAddress { get; set; }

        [JsonProperty(PropertyName = "products")]
        public List<OrderItem> products { get; set; }

        [JsonProperty(PropertyName = "packages")]
        public List<Package> packages { get; set; }

        [JsonProperty(PropertyName = "ssOrderId")]
        public string ssOrderId { get; set; }   

    }

}