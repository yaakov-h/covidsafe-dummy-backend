using System.IO;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace COVIDSafe.Watch.DummyBackend
{
    public static class JsonExtensions
    {
        public static T ReadAsJson<T>(this HttpRequest request)
        {
            using var sr = new StreamReader(request.Body);
            using var jtr = new JsonTextReader(sr);
            var serializer = new JsonSerializer();

            return serializer.Deserialize<T>(jtr);
        }
    }
}
