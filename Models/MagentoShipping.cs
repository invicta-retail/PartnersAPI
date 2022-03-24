// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
using System.Collections.Generic;


namespace InvictaPartnersAPI.Models.Magento.Shipping
{

    public class GetShipItem
    {
        public int billing_address_id { get; set; }
        public string created_at { get; set; }
        public int customer_id { get; set; }
        public int email_sent { get; set; }
        public string increment_id { get; set; }
        public int order_id { get; set; }
        public List<object> packages { get; set; }
        public int shipping_address_id { get; set; }
        public int store_id { get; set; }
        public int total_qty { get; set; }
        public string updated_at { get; set; }
        public List<ShipDtlItem> items { get; set; }
        public List<Track> tracks { get; set; }
        public List<Comment> comments { get; set; }
        public ExtensionAttributes extension_attributes { get; set; }
    }


    public class ShipDtlItem{
        public int entity_id { get; set; }
        public string name { get; set; }
        public int parent_id { get; set; }
        public double price { get; set; }
        public int product_id { get; set; }
        public string sku { get; set; }
        public double weight { get; set; }
        public int order_item_id { get; set; }
        public int qty { get; set; }


    }

    public class Track
    {
        public int order_id { get; set; }
        public string created_at { get; set; }
        public int entity_id { get; set; }
        public int parent_id { get; set; }
        public string updated_at { get; set; }
        public object weight { get; set; }
        public object qty { get; set; }
        public object description { get; set; }
        public string track_number { get; set; }
        public string title { get; set; }
        public string carrier_code { get; set; }
    }

    public class Comment
    {
        public int is_customer_notified { get; set; }
        public int parent_id { get; set; }
        public string comment { get; set; }
        public int is_visible_on_front { get; set; }
        public string created_at { get; set; }
        public int entity_id { get; set; }
    }

    public class ExtensionAttributes
    {
        public string source_code { get; set; }
    }

    public class Filter
    {
        public string field { get; set; }
        public string value { get; set; }
        public string condition_type { get; set; }
    }

    public class FilterGroup
    {
        public List<Filter> filters { get; set; }
    }

    public class SearchCriteria
    {
        public List<FilterGroup> filter_groups { get; set; }
    }

    public class GetShippingRoot
    {
        public List<GetShipItem> items { get; set; }
        public SearchCriteria search_criteria { get; set; }
        public int total_count { get; set; }
    }


}
