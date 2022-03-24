using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace InvictaPartnersAPI.Models
{

    public class Buyer
    {
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string name { get; set; }
    }

    public class BillingAddress
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }
        public string company { get; set; }
        public string street1 { get; set; }
        public string street2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
    }

    public class ShippingAddress
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }
        public string company { get; set; }
        public string street1 { get; set; }
        public string street2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string postalCode { get; set; }
        public string country { get; set; }
        public string phone { get; set; }
    }

    public class SellwareOrderItem
    {
        public string productId { get; set; }
        public string sku { get; set; }
        public string name { get; set; }
        public int qtyPurchased { get; set; }
        public Decimal taxRate { get; set; }
        public Decimal taxAmount { get; set; }
        public Decimal itemPrice { get; set; }
        public Decimal itemTotal{get;set;}
        public string extRefNum{get;set;}
        public string sourceCode{get;set;}
    }

    public class Tax
    {
        public string label { get; set; }
        public Decimal amount { get; set; }
    }

    public class Shipping
    {
        public string label { get; set; }
        public Decimal amount { get; set; }
        public string carrier { get; set; }
        public string serviceLevel { get; set; }
    }

    public class Adjustment
    {
        public string type { get; set; }
        public Tax tax { get; set; }
        public Shipping shipping { get; set; }
    }

    public class PaymentTransaction
    {
        public string provider { get; set; }
        public string label { get; set; }
        public string transactionId { get; set; }
        public string status { get; set; }
        public string currency { get; set; }
        public Decimal amount { get; set; }
        public Decimal fee { get; set; }
        public DateTime createdOn { get; set; }
    }

    public class StatusHistory
    {
        public string status { get; set; }
        public DateTime createdOn { get; set; }
        public string comment { get; set; }
    }


    public class Referral
    {
        public string partner { get; set; }
        public string type { get; set; }
        public string id { get; set; }
        public Decimal fee { get; set; }
        public string currency { get; set; }
    }


    public class Fulfillment
    {
        public string status {get;set;}
        public DateTime  createdOn {get;set;}
        public string carrier {get;set;}
        public string serviceLevel{get;set;}
        public string label{get;set;}
        public string trackingNumber{get;set;}

    }


    public class SellwareRoot {
        public List<SellwareOrder> order{get;set;}
    }
    public class SellwareOrder
    {
        public string id { get; set; }
        public string type {get;set;}
        public string auctionbloxOrderNumber { get; set; }
        public string merchantOrderNumber{get;set;}
        public DateTime createdOn { get; set; }
        public DateTime updatedOn { get; set; }
        public DateTime orderedOn { get; set; }
        public string status { get; set; }
        public string currency { get; set; }
        public Decimal subtotal { get; set; }
        public Decimal total { get; set; }
        public string buyerComment { get; set; }
        public Buyer buyer { get; set; }
        public BillingAddress billingAddress { get; set; }
        public ShippingAddress shippingAddress { get; set; }
        public List<SellwareOrderItem> orderItemList { get; set; }
        public List<Adjustment> adjustmentList { get; set; }
        public List<PaymentTransaction> paymentTransactionList { get; set; }
        public List<StatusHistory> statusHistoryList { get; set; }
        public List<Referral> referralList { get; set; }
        public List<Fulfillment> fulfillmentList {get;set;}
        public List<Package> packages { get; set; }

    }

        public class OrderResponse {
            
            [JsonProperty(PropertyName = "auctionbloxOrderNumber")]  
            public String auctionbloxOrderNumber {get;set;}

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "merchantOrderNumber")]  
            public String  merchantOrderNumber{get;set;}

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore, PropertyName = "error")]  
            public Error error {get;set;}

        }

        public class OrderValidation {
            
            [JsonProperty(PropertyName = "valid")]  
            public bool valid {get;set;}

            [JsonProperty(PropertyName = "error")]  
            public Error  error{get;set;}
        }

        public class Error {

            [JsonProperty(PropertyName = "code")]
            public String code {get;set;}

            [JsonProperty(PropertyName = "severity")]
            public String severity {get;set;}

            [JsonProperty(PropertyName = "message")]
            public String message {get;set;}
        }


        public class OrderFeed
        {
            public string auctionbloxOrderNumber { get; set; }
            public string merchantOrderNumber {get;set;}
            public DateTime createdOn {get;set;}
            public DateTime updatedOn {get;set;}
            public string status {get;set;}
            public List<Fulfillment> fulfillmentList {get;set;} 
        } 


}