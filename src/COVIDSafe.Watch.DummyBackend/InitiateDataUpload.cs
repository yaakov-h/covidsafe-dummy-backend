using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace COVIDSafe.Watch.DummyBackend
{
    public static class InitiateDataUpload
    {
        [FunctionName("initiateDataUpload")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dummy/initiateDataUpload")] HttpRequest request)
        {
            var subject = request.GetJwtSubject();
            var pin = request.Headers["pin"];

            // TODO: What happens here?

            var response = new ResponseObject
            {
                Message = "DeviceDidNotRequestUpload",
            };

            return new JsonResult(response)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }

        public sealed class ResponseObject
        {
            [JsonProperty("message")]
            public string Message { get; set; }
        }
    }
}
