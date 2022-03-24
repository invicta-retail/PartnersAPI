using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using InvictaPartnersAPI.Models.Magento.Order;
using InvictaPartnersAPI.Models;
using Microsoft.Extensions.Configuration;

namespace InvictaPartnersAPI.Controllers
{
    public static class MagentoSupport
    {
        public static async Task<string> GetMagentoToken(string site,IConfiguration configuration)
        {
            var uri = configuration[site+":magentobaseurl"] + "integration/admin/token";
            var user = configuration[site+":magentouser"];
            var password = configuration[site+":magentopassword"];
            Console.WriteLine("uri:" + uri);
            Uri u = new Uri(uri);
            var payload = "{\"username\": \"" + user + "\",\"password\": \"" + password + "\"}";

            HttpContent c = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

            var t = await SendURI(u, c, null);
            return "{\"token\": " + t + "}";
        }

        public static async Task<string> SendURI(Uri u, HttpContent c, string token)
        {

            var response = string.Empty;
            using (var client = new HttpClient())
            {

                if (!String.IsNullOrEmpty(token))
                {  //use the token as authorization if provided
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                HttpResponseMessage result = await client.PostAsync(u, c);
                if (result.IsSuccessStatusCode)
                {
                    response = await result.Content.ReadAsStringAsync();
                }

            }

            return response;
        }

        public static async Task<string> GetMagentoOrder(string site, string token, string orderNumber,IConfiguration configuration)
        {


            string response = "";

            using (var client = new HttpClient())
            {
                var uri = configuration[site+":magentobaseurl"] + "orders?searchCriteria[filter_groups][0][filters][0][field]=increment_id&searchCriteria[filter_groups][0][filters][0][value]=" + orderNumber + "&searchCriteria[filter_groups][0][filters][0][condition_type]=eq";
                Console.WriteLine("OrderURL:" + uri.ToString());

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Uri u = new Uri(uri);
                HttpResponseMessage result = await client.GetAsync(u);
                if (result.IsSuccessStatusCode)
                {
                    response = await result.Content.ReadAsStringAsync();
                }

            }
            return response;
        }

        public static async Task<string> GetMagentoShipments(string site, string token, string orderNumber,IConfiguration configuration)
        {


            string response = "";

            using (var client = new HttpClient())
            {
                var uri = configuration[site+":magentobaseurl"] + "shipments?searchCriteria[filter_groups][0][filters][0][field]=increment_id&searchCriteria[filter_groups][0][filters][0][value]=" + orderNumber + "&searchCriteria[filter_groups][0][filters][0][condition_type]=eq";
                Console.WriteLine("OrderURL:" + uri.ToString());

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Uri u = new Uri(uri);
                HttpResponseMessage result = await client.GetAsync(u);
                if (result.IsSuccessStatusCode)
                {
                    response = await result.Content.ReadAsStringAsync();
                }

            }
            return response;
        }

        public static async  Task<Int32> PostMagentoShipment(string site,string token,string source,int orderId, OrderRoot magentoOrder, ShipmentRoot _shipmentRoot, IConfiguration configuration)
        {

            try {

                List<ShipItem> shipitems = new List<ShipItem>();
                List<Track> trackList = new List<Track>();

                foreach (var orderItem in magentoOrder.items[0].items)
                {
                    foreach (var shipRow in _shipmentRoot.details)
                    {

                        if (shipRow.itemNumber.Replace("ZG-","") == orderItem.sku.Replace("ZG-",""))
                        {
                            ShipItem _item = new ShipItem()
                            {
                                order_item_id = orderItem.item_id,
                                qty = shipRow.shippedQuantity
                            };
                            
                            shipitems.Add(_item);
                            Console.WriteLine("Item:"+orderItem.item_id+" "+shipRow.shippedQuantity);

                            var findtrac = trackList.Find(tr=>tr.track_number==shipRow.trackingNumber);
                            if (findtrac == null) {
                                Track trac = new Track()
                                {
                                    extension_attributes = null,
                                    track_number = shipRow.trackingNumber,
                                    carrier_code = shipRow.carrier
                                };
                                trackList.Add(trac);
                            }

                        }
                    }
                }

                if (shipitems.Count > 0)
                {

                    Comment comment = new Comment()
                    {
                        is_visible_on_front = 0,
                        extension_attributes = null,
                        comment = "Item(s) has been shipped"
                    };

                    ShipExtensionAttributes exattributes = new ShipExtensionAttributes()
                    {
                        source_code = source  //all shipments for invicta use cw
                    };
                    Arguments arguments = new Arguments()
                    {
                        extension_attributes = exattributes
                    };
                    ShipRoot shipRoot = new ShipRoot()
                    {
                        notify = true,
                        appendComment = true,
                        comment = comment,
                        tracks = trackList,
                        arguments = arguments,
                        items = shipitems
                    };

                    var uri = configuration[site+":magentobaseurl"] + "order/" + orderId + "/ship";
                    Console.WriteLine("url: "+uri);
                    HttpContent c = new StringContent(System.Text.Json.JsonSerializer.Serialize(shipRoot), System.Text.Encoding.UTF8, "application/json");
                    Console.WriteLine("Payload:"+System.Text.Json.JsonSerializer.Serialize(shipRoot));
                    Uri u = new Uri(uri);
                    var response = await SendURI(u, c, token);
                    Int32 shipmentID = 0;
                    //Update ShippersConfirmnationEntry
                    if (!String.IsNullOrEmpty(response)) {
                        // remove quotes from the response ID
                        if (response.Length > 2) {
                            response = response.Remove(0, 1).Remove(response.Length - 2);
                        } 
                        // if the response string cannot be converted to an int, return false with shipmentID = 0
                        if (!Int32.TryParse(response, out shipmentID)) shipmentID = 0;
                    }
                    Console.WriteLine("Shipment Created:"+shipmentID);
                    return shipmentID;

                } else
                {
                    return 0;
                }

            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return 0;
            }

        }

        public async static Task<string> PostTrackingUpdate(string site, string token,TrackUpdate tracUpdate,IConfiguration configuration)
        {
            try {
                var uri = configuration[site+":magentobaseurl"] + "shipment/track";
                HttpContent c = new StringContent(System.Text.Json.JsonSerializer.Serialize(tracUpdate), System.Text.Encoding.UTF8, "application/json");
                Uri u = new Uri(uri);
                var response = await SendURI(u, c, token);
                return response;
                
            } catch {

                return null;

            }

        }

        public async static Task<string> PostComment(string site,string token,string comment, int orderId,IConfiguration configuration)
        {
            try {
                Models.Magento.Order.StatusHistory statusHistory = new Models.Magento.Order.StatusHistory(){
                    comment=comment,
                    parent_id=orderId,
                    is_customer_notified=1,
                    is_visible_on_front=1
                };

                StatusHistoryRoot rootComment = new StatusHistoryRoot(){
                    statusHistory=statusHistory
                };

                var uri = configuration[site+":magentobaseurl"] + "orders/"+orderId+"/comments";
                HttpContent c = new StringContent(System.Text.Json.JsonSerializer.Serialize(rootComment), System.Text.Encoding.UTF8, "application/json");
                Uri u = new Uri(uri);
                var response = await SendURI(u, c, token);
                return response;
                
            } catch {

                return null;

            }

        }


    }

}
