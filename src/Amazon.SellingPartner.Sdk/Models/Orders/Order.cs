using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.SellingPartner.Sdk.Models.Orders
{
    public class Order
    {
        public string AmazonOrderId { get; set; }

        public string PurchaseDate { get; set; }
     
        public string LastUpdateDate { get; set; }
       
        public string OrderStatus { get; set; }
      
        public string SellerOrderId { get; set; }

        public string FulfillmentChannel { get; set; }
        
        public string SalesChannel { get; set; }
      
        public string ShipServiceLevel { get; set; }
    }
    public class OrderList
    {
        public List<Order> Orders { get; set; }
    }
}
