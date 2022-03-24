using System.Collections.Generic;

namespace InvictaPartnersAPI.Models.Magento.Order
{

    //Main ModelClasses:
    //* ShipRoot
    //* TrackUpdate
    //* OrderRoot

    public class Entity
    {
        public int order_id { get; set; }
        public int parent_id { get; set; }
        public string track_number { get; set; }
        public string title { get; set; }
        public string carrier_code { get; set; }
    }

    public class TrackUpdate
    {
        public Entity entity { get; set; }
    }


    public class Item
    {
        public double amount_refunded { get; set; }
        public double base_amount_refunded { get; set; }
        public double base_cost { get; set; }
        public double base_discount_amount { get; set; }
        public double base_discount_invoiced { get; set; }
        public double base_discount_tax_compensation_amount { get; set; }
        public double base_discount_tax_compensation_invoiced { get; set; }
        public double base_original_price { get; set; }
        public double base_price { get; set; }
        public double base_price_incl_tax { get; set; }
        public double base_row_invoiced { get; set; }
        public double base_row_total { get; set; }
        public double base_row_total_incl_tax { get; set; }
        public double base_tax_amount { get; set; }
        public double base_tax_invoiced { get; set; }
        public string created_at { get; set; }
        public double discount_amount { get; set; }
        public double discount_invoiced { get; set; }
        public double discount_percent { get; set; }
        public int free_shipping { get; set; }
        public double discount_tax_compensation_amount { get; set; }
        public double discount_tax_compensation_invoiced { get; set; }
        public int is_qty_decimal { get; set; }
        public int is_virtual { get; set; }
        public int item_id { get; set; }
        public string name { get; set; }
        public int no_discount { get; set; }
        public int order_id { get; set; }
        public double original_price { get; set; }
        public double price { get; set; }
        public double price_incl_tax { get; set; }
        public int product_id { get; set; }
        public string product_type { get; set; }
        public int qty_canceled { get; set; }
        public int qty_invoiced { get; set; }
        public int qty_ordered { get; set; }
        public int qty_refunded { get; set; }
        public int qty_shipped { get; set; }
        public int quote_item_id { get; set; }
        public double row_invoiced { get; set; }
        public double row_total { get; set; }
        public double row_total_incl_tax { get; set; }
        public double row_weight { get; set; }
        public string sku { get; set; }
        public int store_id { get; set; }
        public double tax_amount { get; set; }
        public double tax_invoiced { get; set; }
        public double tax_percent { get; set; }
        public string updated_at { get; set; }
        public string weee_tax_applied { get; set; }
        public double weight { get; set; }
        public double? base_discount_refunded { get; set; }
        public double? base_discount_tax_compensation_refunded { get; set; }
        public double? base_tax_refunded { get; set; }
        public double? discount_refunded { get; set; }
        public double? discount_tax_compensation_refunded { get; set; }
        public double? tax_refunded { get; set; }
    }

    public class BillingAddress
    {
        public string address_type { get; set; }
        public string city { get; set; }
        public string country_id { get; set; }
        public string email { get; set; }
        public int entity_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public int parent_id { get; set; }
        public string postcode { get; set; }
        public string region { get; set; }
        public string region_code { get; set; }
        public int region_id { get; set; }
        public List<string> street { get; set; }
        public string telephone { get; set; }
    }

    public class Payment
    {
        public object account_status { get; set; }
        public List<string> additional_information { get; set; }
        public double amount_ordered { get; set; }
        public double amount_paid { get; set; }
        public double amount_refunded { get; set; }
        public double base_amount_ordered { get; set; }
        public double base_amount_paid { get; set; }
        public double base_amount_refunded { get; set; }
        public double base_amount_refunded_online { get; set; }
        public double base_shipping_amount { get; set; }
        public double base_shipping_captured { get; set; }
        public double base_shipping_refunded { get; set; }
        public string cc_last4 { get; set; }
        public string cc_type { get; set; }
        public int entity_id { get; set; }
        public string last_trans_id { get; set; }
        public string method { get; set; }
        public int parent_id { get; set; }
        public double shipping_amount { get; set; }
        public double shipping_captured { get; set; }
        public double shipping_refunded { get; set; }
    }

    public class StatusHistory
    {
        public string comment { get; set; }
        public string created_at { get; set; }
        public int entity_id { get; set; }
        public string entity_name { get; set; }
        public int? is_customer_notified { get; set; }
        public int is_visible_on_front { get; set; }
        public int parent_id { get; set; }
        public string status { get; set; }
    }

    public class StatusHistoryRoot
    {
        public StatusHistory statusHistory { get; set; }
    }

    public class Address
    {
        public string address_type { get; set; }
        public string city { get; set; }
        public string country_id { get; set; }
        public string email { get; set; }
        public int entity_id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public int parent_id { get; set; }
        public string postcode { get; set; }
        public string region { get; set; }
        public string region_code { get; set; }
        public int region_id { get; set; }
        public List<string> street { get; set; }
        public string telephone { get; set; }
    }

    public class Total
    {
        public double base_shipping_amount { get; set; }
        public double base_shipping_discount_amount { get; set; }
        public double base_shipping_discount_tax_compensation_amnt { get; set; }
        public double base_shipping_incl_tax { get; set; }
        public double base_shipping_invoiced { get; set; }
        public double base_shipping_refunded { get; set; }
        public double base_shipping_tax_amount { get; set; }
        public double base_shipping_tax_refunded { get; set; }
        public double shipping_amount { get; set; }
        public double shipping_discount_amount { get; set; }
        public double shipping_discount_tax_compensation_amount { get; set; }
        public double shipping_incl_tax { get; set; }
        public double shipping_invoiced { get; set; }
        public double shipping_refunded { get; set; }
        public double shipping_tax_amount { get; set; }
        public double shipping_tax_refunded { get; set; }
    }

    public class Shipping
    {
        public Address address { get; set; }
        public string method { get; set; }
        public Total total { get; set; }
    }

    public class ShippingAssignment
    {
        public Shipping shipping { get; set; }
        public List<Item> items { get; set; }
    }

    public class PaymentAdditionalInfo
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class AppliedTax
    {
        public string code { get; set; }
        public string title { get; set; }
        public double percent { get; set; }
        public double amount { get; set; }
        public double base_amount { get; set; }
    }

    public class ItemAppliedTax
    {
        public string type { get; set; }
        public List<AppliedTax> applied_taxes { get; set; }
        public int? item_id { get; set; }
    }

    public class AmGiftcardOrder
    {
        public int entity_id { get; set; }
        public int order_id { get; set; }
        public List<object> gift_cards { get; set; }
        public double gift_amount { get; set; }
        public double base_gift_amount { get; set; }
        public double invoice_gift_amount { get; set; }
        public double base_invoice_gift_amount { get; set; }
        public double refund_gift_amount { get; set; }
        public double base_refund_gift_amount { get; set; }
        public List<object> applied_accounts { get; set; }
    }

    public partial class FdxcbData
    {
        public int order_id { get; set; }
        public string fxcb_order_number { get; set; }
        public string tracking_link { get; set; }
        public string status { get; set; }

    }
    public class ExtensionAttributes
    {
        public List<ShippingAssignment> shipping_assignments { get; set; }
        public List<PaymentAdditionalInfo> payment_additional_info { get; set; }
        public List<AppliedTax> applied_taxes { get; set; }
        public List<ItemAppliedTax> item_applied_taxes { get; set; }
        public bool converting_from_quote { get; set; }
        public AmGiftcardOrder am_giftcard_order { get; set; }
        public FdxcbData fdxcData { get; set; }
    }

    public class Orderitem
    {
        public double adjustment_negative { get; set; }
        public double adjustment_positive { get; set; }
        public double base_adjustment_negative { get; set; }
        public double base_adjustment_positive { get; set; }
        public string base_currency_code { get; set; }
        public double base_discount_amount { get; set; }
        public double base_discount_invoiced { get; set; }
        public double base_discount_refunded { get; set; }
        public double base_grand_total { get; set; }
        public double base_discount_tax_compensation_amount { get; set; }
        public double base_discount_tax_compensation_invoiced { get; set; }
        public double base_discount_tax_compensation_refunded { get; set; }
        public double base_shipping_amount { get; set; }
        public double base_shipping_discount_amount { get; set; }
        public double base_shipping_discount_tax_compensation_amnt { get; set; }
        public double base_shipping_incl_tax { get; set; }
        public double base_shipping_invoiced { get; set; }
        public double base_shipping_refunded { get; set; }
        public double base_shipping_tax_amount { get; set; }
        public double base_shipping_tax_refunded { get; set; }
        public double base_subtotal { get; set; }
        public double base_subtotal_incl_tax { get; set; }
        public double base_subtotal_invoiced { get; set; }
        public double base_subtotal_refunded { get; set; }
        public double base_tax_amount { get; set; }
        public double base_tax_invoiced { get; set; }
        public double base_tax_refunded { get; set; }
        public double base_total_due { get; set; }
        public double base_total_invoiced { get; set; }
        public double base_total_invoiced_cost { get; set; }
        public double base_total_online_refunded { get; set; }
        public double base_total_paid { get; set; }
        public double base_total_refunded { get; set; }
        public double base_to_global_rate { get; set; }
        public double base_to_order_rate { get; set; }
        public double billing_address_id { get; set; }
        public string created_at { get; set; }
        public string customer_email { get; set; }
        public int customer_group_id { get; set; }
        public int customer_is_guest { get; set; }
        public int customer_note_notify { get; set; }
        public double discount_amount { get; set; }
        public double discount_invoiced { get; set; }
        public double discount_refunded { get; set; }
        public int entity_id { get; set; }
        public string global_currency_code { get; set; }
        public double grand_total { get; set; }
        public double discount_tax_compensation_amount { get; set; }
        public double discount_tax_compensation_invoiced { get; set; }
        public double discount_tax_compensation_refunded { get; set; }
        public string increment_id { get; set; }
        public int is_virtual { get; set; }
        public string order_currency_code { get; set; }
        public string protect_code { get; set; }
        public int quote_id { get; set; }
        public string remote_ip { get; set; }
        public double shipping_amount { get; set; }
        public string shipping_description { get; set; }
        public double shipping_discount_amount { get; set; }
        public double shipping_discount_tax_compensation_amount { get; set; }
        public double shipping_incl_tax { get; set; }
        public double shipping_invoiced { get; set; }
        public double shipping_refunded { get; set; }
        public double shipping_tax_amount { get; set; }
        public double shipping_tax_refunded { get; set; }
        public string state { get; set; }
        public string status { get; set; }
        public string store_currency_code { get; set; }
        public int store_id { get; set; }
        public string store_name { get; set; }
        public double store_to_base_rate { get; set; }
        public double store_to_order_rate { get; set; }
        public double subtotal { get; set; }
        public double subtotal_incl_tax { get; set; }
        public double subtotal_invoiced { get; set; }
        public double subtotal_refunded { get; set; }
        public double tax_amount { get; set; }
        public double tax_invoiced { get; set; }
        public double tax_refunded { get; set; }
        public double total_due { get; set; }
        public double total_invoiced { get; set; }
        public int total_item_count { get; set; }
        public double total_online_refunded { get; set; }
        public double total_paid { get; set; }
        public int total_qty_ordered { get; set; }
        public double total_refunded { get; set; }
        public string updated_at { get; set; }
        public double weight { get; set; }
        public string x_forwarded_for { get; set; }
        public List<Item> items { get; set; }
        public BillingAddress billing_address { get; set; }
        public Payment payment { get; set; }
        public List<StatusHistory> status_histories { get; set; }
        public ExtensionAttributes extension_attributes { get; set; }
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

    public class OrderRoot
    {
        public List<Orderitem> items { get; set; }
        public SearchCriteria search_criteria { get; set; }
        public int total_count { get; set; }
    }

    public class ShipItem
    {
        public int order_item_id { get; set; }
        public int qty { get; set; }
    }

    public class Comment
    {
        public object extension_attributes { get; set; }
        public string comment { get; set; }
        public int is_visible_on_front { get; set; }
    }

    public class Track
    {
        public object extension_attributes { get; set; }
        public string track_number { get; set; }
        public string title { get; set; }
        public string carrier_code { get; set; }
    }

    public class ShipExtensionAttributes
    {
        public string source_code { get; set; }
    }

    public class Arguments
    {
        public ShipExtensionAttributes extension_attributes { get; set; }
    }

    public class ShipRoot
    {
        public List<ShipItem> items { get; set; }
        public bool notify { get; set; }
        public bool appendComment { get; set; }
        public Comment comment { get; set; }
        public List<Track> tracks { get; set; }
        public Arguments arguments { get; set; }
    }

}


