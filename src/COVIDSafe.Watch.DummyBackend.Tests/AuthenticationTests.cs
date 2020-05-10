using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Moq;
using NUnit.Framework;

namespace COVIDSafe.Watch.DummyBackend.Tests
{
    public class Authentication
    {
        [Test]
        public void ReturnsTokenAndTempIDsIfOTPMatchesLastDigitsOfPhoneNumber()
        {
            var initResponse = RunInitiateAuth(@"{""age"":""123"",""device_id"":""MyFunkyDevice"",""name"":""firstname surname"",""phone_number"":""401234567"",""postcode"":""2000""}");
            Assert.That(initResponse.ChallengeName, Is.EqualTo("OTP"));

            var respActionResult = RunRespondToAuthChallenge($@"{{""code"":""234567"",""session"":""{initResponse.Session}""}}");
            Assert.That(respActionResult, Is.TypeOf<JsonResult>());

            var jsonResult = (JsonResult)respActionResult;
            Assert.That(jsonResult.Value, Is.TypeOf<RespondToAuthChallenge.ResponseObject>());

            var response = (RespondToAuthChallenge.ResponseObject)jsonResult.Value;
            Assert.That(response.Token, Is.Not.Null.Or.Empty);

            var time = new DateTime(2020, 05, 10, 13, 54, 01, 123, DateTimeKind.Utc);
            var expectedTempID = CalculateSHA256Hash("401234567@20200510135401123");

            var tempIDResult = RunGetTempID(response.Token, time);
            Assert.That(tempIDResult.TempID, Is.EqualTo(expectedTempID));

            Assert.That(DateTimeOffset.FromUnixTimeSeconds(tempIDResult.RefreshTime).UtcDateTime, Is.EqualTo(time.AddHours(1)).Within(TimeSpan.FromSeconds(1)));
            Assert.That(DateTimeOffset.FromUnixTimeSeconds(tempIDResult.ExpiryTime).UtcDateTime, Is.EqualTo(time.AddHours(2)).Within(TimeSpan.FromSeconds(1)));
        }

        [Test]
        public void ReturnsErrorIfInvalidOTP()
        {
            var initResponse = RunInitiateAuth(@"{""age"":""123"",""device_id"":""MyFunkyDevice"",""name"":""firstname surname"",""phone_number"":""401234567"",""postcode"":""2000""}");
            Assert.That(initResponse.ChallengeName, Is.EqualTo("OTP"));

            var respActionResult = RunRespondToAuthChallenge($@"{{""code"":""000000"",""session"":""{initResponse.Session}""}}");
            Assert.That(respActionResult, Is.TypeOf<StatusCodeResult>());

            var statusCodeResult = (StatusCodeResult)respActionResult;
            Assert.That(statusCodeResult.StatusCode, Is.GreaterThan(400).And.LessThan(599));
        }

        static InitiateAuth.ResponseObject RunInitiateAuth(string requestBody)
        {
            var context = new DefaultHttpContext();
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));

            var actionResult = InitiateAuth.Run(context.Request);
            Assert.That(actionResult, Is.TypeOf<JsonResult>());

            var jsonResult = (JsonResult)actionResult;
            Assert.That(jsonResult.Value, Is.TypeOf<InitiateAuth.ResponseObject>());

            return (InitiateAuth.ResponseObject)jsonResult.Value;
        }

        static IActionResult RunRespondToAuthChallenge(string requestBody)
        {
            var context = new DefaultHttpContext();
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
            var actionResult = RespondToAuthChallenge.Run(context.Request);
            return actionResult;
        }

        static GetTempID.ResponseObject RunGetTempID(string token, DateTime time)
        {
            var mockSystemClock = new Mock<ISystemClock>();
            mockSystemClock.Setup(x => x.UtcNow).Returns(time);

            var context = new DefaultHttpContext();
            context.Request.Headers[HeaderNames.Authorization] = "Bearer " + token;

            var actionResult = new GetTempID(mockSystemClock.Object).Run(context.Request);
            Assert.That(actionResult, Is.TypeOf<JsonResult>());

            var jsonResult = (JsonResult)actionResult;
            Assert.That(jsonResult.Value, Is.TypeOf<GetTempID.ResponseObject>());

            return (GetTempID.ResponseObject)jsonResult.Value;
        }

        static string CalculateSHA256Hash(string input)
        {
            using var algorithm = SHA256.Create();
            return Convert.ToBase64String(algorithm.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }
    }
}
