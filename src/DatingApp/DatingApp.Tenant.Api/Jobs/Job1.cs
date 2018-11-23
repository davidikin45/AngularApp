using Microsoft.Extensions.Logging;

namespace DatingApp.Api.Jobs
{
    public class Job1
    {
        private ILogger _Logger;

        public Job1(ILogger<Job1> logger)
        {
            _Logger = logger;
        }

        public void Execute()
        {
            _Logger.LogInformation("Executing...");
        }
    }
}
