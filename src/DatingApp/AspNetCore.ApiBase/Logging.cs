using Microsoft.Extensions.Configuration;
using Serilog;

namespace AspnetCore.ApiBase
{
    //http://mygeekjourney.com/asp-net-core/integrating-serilog-asp-net-core/
    //https://www.carlrippon.com/asp-net-core-logging-with-serilog-and-sql-server/
    //Logging
    //Trace = 0
    //Debug = 1 -- Developement Standard
    //Information = 2 -- LogFactory Default
    //Warning = 3 -- Production Standard
    //Error = 4
    //Critical = 5

    public class Logging
    {
        public static void Init(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
             .ReadFrom.Configuration(configuration)
             .Enrich.FromLogContext()
             .CreateLogger();
        }
    }
}
