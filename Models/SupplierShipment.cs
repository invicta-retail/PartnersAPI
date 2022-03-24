using System;
using System.Collections.Generic;

namespace InvictaPartnersAPI.Models
{

    public class ShipmentDetail
    {
        public int lineNumber { get; set; }
        public string itemNumber { get; set; }
        public int orderedQuantity { get; set; }
        public int shippedQuantity { get; set; }
        public int canceledQuantity { get; set; }
        public DateTime shippedDate { get; set; }
        public string carrier { get; set; }
        public string trackingNumber { get; set; }
        public bool prePaidReturnLabelUsed { get; set; }
        public Nullable<decimal> prePaidReturnLabelCost { get; set; }    

    }

    public class ShipmentRoot
    {
        public string orderNumber { get; set; }
        public string customerNumber { get; set; }
        public DateTime orderDate { get; set; }
        public int CompanyId { get; set; }        
        public int shipmentId { get; set; }  //ShipStation Shipment Id
        public int orderId { get; set; }  //ShipStation Order Id
        public int mageShipmentId{get;set;}  //Magento Shipment Id
        public List<string> voidTracking {get;set;} //Shipstation voided tracking associated to Order Id
        public List<ShipmentDetail> details { get; set; }
    }

}