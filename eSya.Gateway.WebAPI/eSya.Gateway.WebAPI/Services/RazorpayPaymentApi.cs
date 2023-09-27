using eSya.Gateway.WebAPI.Utility;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eSya.Gateway.WebAPI.Services
{
    public interface IRazorpayPaymentApi
    {
        DO_Order_response FetchOrder(string orderKey);
    }
    public class RazorpayPaymentApi : IRazorpayPaymentApi
    {
        IConfiguration configuration;
        public RazorpayPaymentApi(IConfiguration iConfig)
        {
            configuration = iConfig;
        }
        //public string _key = "rzp_test_YNEWSySc436ahk";
        //public string _secret = "c7VMdtEAfuTd5gHZS5FGUkQR";

        public DO_Order_response CreateOrder(long bookingKey, decimal transactionAmount)
        {

            string _key = CryptGeneration.DecryptWithSecretKey(configuration.GetValue<string>("razorpay:key"));
            string _secret = CryptGeneration.DecryptWithSecretKey(configuration.GetValue<string>("razorpay:secret"));

            RazorpayClient client = new RazorpayClient(_key, _secret);

            Dictionary<string, object> options = new Dictionary<string, object>();

            options.Add("amount", transactionAmount * 100);
            options.Add("currency", "INR");
            options.Add("receipt", bookingKey.ToString());
            options.Add("payment_capture", 1);

            Order order = client.Order.Create(options);

            DO_Order_response obj = new DO_Order_response();
            obj.id = order.Attributes["id"];
            obj.amount = order.Attributes["amount"] ?? 0;
            obj.currency = order.Attributes["currency"];
            obj.status = order.Attributes["status"];
            obj.created_at = order.Attributes["created_at"];


            return obj;
        }

        public DO_Order_response FetchOrder(string orderKey)
        {

            string _key = CryptGeneration.DecryptWithSecretKey(configuration.GetValue<string>("razorpay:key"));
            string _secret = CryptGeneration.DecryptWithSecretKey(configuration.GetValue<string>("razorpay:secret"));

            RazorpayClient client = new RazorpayClient(_key, _secret);

            Order order = client.Order.Fetch(orderKey);

            DO_Order_response obj = new DO_Order_response();
            obj.id = order.Attributes["id"];
            obj.entity = order.Attributes["entity"];
            obj.amount = order.Attributes["amount"] ?? 0;
            obj.currency = order.Attributes["currency"];
            obj.status = order.Attributes["status"];
            obj.created_at = order.Attributes["created_at"];


            return obj;
        }
    }

    public class DO_Order_response
    {
        public string id { get; set; }
        public string entity { get; set; }
        public decimal amount { get; set; }
        public decimal amount_paid { get; set; }
        public decimal amount_due { get; set; }
        public string currency { get; set; }
        public string receipt { get; set; }
        public string offer_id { get; set; }
        public string status { get; set; }
        public string attempts { get; set; }
        public string created_at { get; set; }
    }
}
