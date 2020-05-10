using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace COVIDSafe.Watch.DummyBackend
{
    public class GetTempID
    {
        public GetTempID(ISystemClock systemClock)
        {
            this.systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        }

        readonly ISystemClock systemClock;

        [FunctionName("getTempId")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "dummy/getTempId")] HttpRequest request)
        {
            var subject = request.GetJwtSubject();
            var time = systemClock.UtcNow;

            var input = Encoding.UTF8.GetBytes(subject + "@" + time.ToString("yyyyMMddHHmmssfff"));
            using var algorithm = SHA256.Create();
            var hash = algorithm.ComputeHash(input);

            var response = new ResponseObject
            {
                TempID = Convert.ToBase64String(hash),
                ExpiryTime = time.AddHours(2).ToOffset(TimeSpan.Zero).ToUnixTimeSeconds(),
                RefreshTime = time.AddHours(1).ToOffset(TimeSpan.Zero).ToUnixTimeSeconds(),
            };
            return new JsonResult(response);
        }

        public sealed class ResponseObject
        {
            [JsonProperty("tempId")]
            public string TempID { get; set; }

            [JsonProperty("expiryTime")]
            public long ExpiryTime { get; set; }

            [JsonProperty("refreshTime")]
            public long RefreshTime { get; set; }
        }
    }
}
