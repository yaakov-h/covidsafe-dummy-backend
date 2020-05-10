using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace COVIDSafe.Watch.DummyBackend.Tests
{
    public class Authentication
    {
        [Test]
        public void ReturnsTokenIfOTPMatchesLastDigitsOfPhoneNumber()
        {
            var initResponse = RunInitiateAuth(@"{""age"":""123"",""device_id"":""MyFunkyDevice"",""name"":""firstname surname"",""phone_number"":""401234567"",""postcode"":""2000""}");
            Assert.That(initResponse.ChallengeName, Is.EqualTo("OTP"));

            var respResponse = RunRespondToAuthChallenge($@"{{""code"":""234567"",""session"":""{initResponse.Session}""}}");
            Assert.That(respResponse.Token, Is.Not.Null.Or.Empty);
        }

        [Test]
        public void ReturnsErrorIfInvalidOTP()
        {
            var initResponse = RunInitiateAuth(@"{""age"":""123"",""device_id"":""MyFunkyDevice"",""name"":""firstname surname"",""phone_number"":""401234567"",""postcode"":""2000""}");
            Assert.That(initResponse.ChallengeName, Is.EqualTo("OTP"));

            var respResponse = RunRespondToAuthChallenge($@"{{""code"":""000000"",""session"":""{initResponse.Session}""}}");
            Assert.That(respResponse.Token, Is.Not.Null.Or.Empty);
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

        static RespondToAuthChallenge.ResponseObject RunRespondToAuthChallenge(string requestBody)
        {
            var context = new DefaultHttpContext();
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
            var actionResult = RespondToAuthChallenge.Run(context.Request);

            var jsonResult = (JsonResult)actionResult;
            Assert.That(jsonResult.Value, Is.TypeOf<RespondToAuthChallenge.ResponseObject>());

            return (RespondToAuthChallenge.ResponseObject)jsonResult.Value;
        }
    }
}