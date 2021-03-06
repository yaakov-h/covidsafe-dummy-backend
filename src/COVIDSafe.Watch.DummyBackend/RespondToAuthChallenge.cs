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
    public static class RespondToAuthChallenge
    {
        [FunctionName("respondToAuthChallenge")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "dummy/respondToAuthChallenge")] HttpRequest request)
        {
            var requestObject = request.ReadAsJson<RequestObject>();

            var originalPhoneNumber = Encoding.UTF8.GetString(Convert.FromBase64String(requestObject.Session));
            var expectedCode = originalPhoneNumber[^6..];

            if (expectedCode == requestObject.Code)
            {
                var response = new ResponseObject
                {
                    Token = JsonExtensions.CreateWebToken(originalPhoneNumber),
                };
                return new JsonResult(response);
            }
            else
            {
                // TODO: What does the prod API use here?
                return new StatusCodeResult(StatusCodes.Status401Unauthorized);
            }
        }

        public sealed class RequestObject
        {
            [JsonProperty("code")]
            public string Code { get; set; }

            [JsonProperty("session")]
            public string Session { get; set; }
        }

        public sealed class ResponseObject
        {
            [JsonProperty("token")]
            public string Token { get; set; }
        }
    }
}
