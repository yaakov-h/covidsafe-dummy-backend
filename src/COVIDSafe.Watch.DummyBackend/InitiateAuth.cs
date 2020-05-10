using System;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace COVIDSafe.Watch.DummyBackend
{
    public static class InitiateAuth
    {
        [FunctionName("initiateAuth")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request)
        {
            var requestObject = request.ReadAsJson<RequestObject>();
            var sessionID = Convert.ToBase64String(Encoding.UTF8.GetBytes(requestObject.PhoneNumber));

            var response = new ResponseObject
            {
                Session = sessionID,
                ChallengeName = "OTP"
            };

            return new JsonResult(response);
        }

        public sealed class RequestObject
        {
            [JsonProperty("age")]
            public string Age { get; set; }

            [JsonProperty("device_id")]
            public string DeviceID { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("phone_number")]
            public string PhoneNumber { get; set; }

            [JsonProperty("postcode")]
            public string Postcode { get; set; }
        }

        public sealed class ResponseObject
        {
            [JsonProperty("session")]
            public string Session { get; set; }

            [JsonProperty("challengeName")]
            public string ChallengeName { get; set; }
        }
    }
}
