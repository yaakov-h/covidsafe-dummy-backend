using COVIDSafe.Watch.DummyBackend;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Startup))]

namespace COVIDSafe.Watch.DummyBackend
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ISystemClock, SystemClock>();
        }
    }
}
