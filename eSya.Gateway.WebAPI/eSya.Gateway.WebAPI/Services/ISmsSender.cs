using eSya.Gateway.IF;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace eSya.Gateway.WebAPI.Services
{
    public interface ISmsSender
    {
        Task<SMSReponse> SendAsync(string mobileNumber, string messageText);
        Task<SMSReponse> SendSMSAsync(string mobileNumber, string messageText, string SMSProviderUserID, string SMSProviderPassword, string SMSProviderAPI, string SMSProviderSenderID);
    }
    public class SmsSender : ISmsSender
    {
        readonly IConfiguration _configuration;
        public SmsSender(
        IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //private readonly ISmsStatementRepository _smsStatementRepository;
        //public SmsSender(ISmsStatementRepository smsStatementRepository)
        //{
        //    _smsStatementRepository = smsStatementRepository;
        //}

        HttpClient client = new HttpClient();
        //https://www.smscountry.com/bulk-smsc-api-documentation
        string _smsBaseURL = "http://api.smscountry.com/SMSCwebservice_bulk.aspx?User={0}&Passwd={1}&Mobilenumber={3}&Message={4}&Sid={2}&Mtype=N&DR=Y";
        string username = "GESTALTTECH";
        string password = "gtpl@12345";
        string sender = "GTPLTL";
        public async Task<SMSReponse> SendAsync(string mobileNumber, string messageText)
        {
            _smsBaseURL = _configuration.GetValue<string>("smsconfig:requesturl");
            username = _configuration.GetValue<string>("smsconfig:username");
            password = _configuration.GetValue<string>("smsconfig:password");
            sender = _configuration.GetValue<string>("smsconfig:senderid");

            _smsBaseURL = String.Format(_smsBaseURL, username, password, sender, mobileNumber, messageText.Replace("&", "%26"));
            var uri = new Uri(_smsBaseURL);

            var content = new StringContent(string.Empty);
            var Items = new SMSReponse();
            try
            {
                var result = "";
                HttpResponseMessage response = await client.PostAsync(uri, content);
                Items.RequestMessage = _smsBaseURL;
                if (response.IsSuccessStatusCode)
                {
                    Items.ResponseMessage = await response.Content.ReadAsStringAsync();
                    if(Items.ResponseMessage.StartsWith("OK"))
                        Items.SendStatus = true;
                    else
                        Items = JsonConvert.DeserializeObject<SMSReponse>(result);
                    Items.SendStatus = true;
                }
                else
                    Items.SendStatus = false;

                //_smsStatementRepository.Insert_SmsLog(new NG.Gateway.DO.DO_SMSLog
                //{
                //    MessageType = "GC",
                //    MobileNumber = mobileNumber,
                //    RequestMessage = _smsBaseURL,
                //    ResponseMessage = result,
                //    SendStatus = Items.ResponseMessage,
                //});
            }
            catch 
            {
            }
            return Items;
           // return Task.FromResult(0);
        }

        public async Task<SMSReponse> SendSMSAsync(string mobileNumber, string messageText, string SMSProviderUserID, string SMSProviderPassword, string SMSProviderAPI, string SMSProviderSenderID)
        {
            _smsBaseURL = SMSProviderAPI;
            username = SMSProviderUserID;
            password = SMSProviderPassword;
            sender = SMSProviderSenderID;

            _smsBaseURL = String.Format(_smsBaseURL, username, password, sender, mobileNumber, messageText.Replace("&", "%26"));
            var uri = new Uri(_smsBaseURL);

            var content = new StringContent(string.Empty);
            var Items = new SMSReponse();
            try
            {
                var result = "";
                HttpResponseMessage response = await client.PostAsync(uri, content);
                Items.RequestMessage = _smsBaseURL;
                if (response.IsSuccessStatusCode)
                {
                    Items.ResponseMessage = await response.Content.ReadAsStringAsync();
                    if (Items.ResponseMessage.StartsWith("OK"))
                        Items.SendStatus = true;
                    else
                        Items = JsonConvert.DeserializeObject<SMSReponse>(result);
                    Items.SendStatus = true;
                }
                else
                    Items.SendStatus = false;

                //_smsStatementRepository.Insert_SmsLog(new NG.Gateway.DO.DO_SMSLog
                //{
                //    MessageType = "GC",
                //    MobileNumber = mobileNumber,
                //    RequestMessage = _smsBaseURL,
                //    ResponseMessage = result,
                //    SendStatus = Items.ResponseMessage,
                //});
            }
            catch
            {
            }
            return Items;
            // return Task.FromResult(0);
        }
    }

    public class SMSParameter
    {
        public string username { get; set; }
        public string password { get; set; }
        public string request { get; set; }
        public string SMSID { get; set; }
        public string language { get; set; }
        public string sender { get; set; }
        public string mobile { get; set; }
        public string message { get; set; }
        public string DelayUntil { get; set; }
    }

    public class SMSReponse
    {
        public string code { get; set; }
        public string SMSID { get; set; }
        public string RequestMessage { get; set; }
        public string ResponseMessage { get; set; }
        public bool SendStatus { get; set; }

    }
}
