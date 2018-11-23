using AspNetCore.ApiBase.Helpers;
using AspNetCore.ApiBase.Tasks;
using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.MemoryStorage;
using Hangfire.Server;
using Hangfire.SQLite;
using Hangfire.SqlServer;
using Hangfire.States;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace DatingApp.Data.Tenants
{
    public class AppTenantsHangireInitializer : IAsyncInitializer
    {
        private readonly AppTenantsContext _context;

        private readonly IApplicationLifetime _applicationLifetime;
        private readonly IJobFilterProvider _jobFilters;
        private readonly JobActivator _jobActivator;
        private readonly IBackgroundJobFactory _backgroundJobFactory;
        private readonly IBackgroundJobPerformer _backgroundJobPerformer;
        private readonly IBackgroundJobStateChanger _backgroundJobStateChanger;
        private readonly IBackgroundProcess[] _additionalProcesses;

        public AppTenantsHangireInitializer(
            AppTenantsContext context,
            IApplicationLifetime applicationLifetime,
            IJobFilterProvider jobFilters,
            JobActivator jobActivator,
            IBackgroundJobFactory backgroundJobFactory,
            IBackgroundJobPerformer backgroundJobPerformer,
            IBackgroundJobStateChanger backgroundJobStateChanger,
            IBackgroundProcess[] additionalProcesses)
        {
            _context = context;
            _applicationLifetime = applicationLifetime;
            _jobFilters = jobFilters;
            _jobActivator = jobActivator;
            _backgroundJobFactory = backgroundJobFactory;
            _backgroundJobPerformer = backgroundJobPerformer;
            _backgroundJobStateChanger = backgroundJobStateChanger;
            _additionalProcesses = additionalProcesses;
        }

        public async Task ExecuteAsync()
        {
            foreach (var tenant in _context.Tenants)
            {
                var connectionString = tenant.GetConnectionString("DefaultConnection");

                JobStorage storage;
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    storage = new MemoryStorage();
                }
                else if (ConnectionStringHelper.IsSQLite(connectionString))
                {
                    storage = new SQLiteStorage(connectionString);
                }
                else
                {
                    storage = new SqlServerStorage(connectionString);
                }

                var options = new BackgroundJobServerOptions
                {
                    ServerName = tenant.Id//string.Format("{0}.{1}", tenant, Guid.NewGuid().ToString())
                };

                var server = new BackgroundJobServer(options, storage, _additionalProcesses,
                    _jobFilters,
                    _jobActivator,
                   _backgroundJobFactory,
                    _backgroundJobPerformer,
                    _backgroundJobStateChanger);

                _applicationLifetime.ApplicationStopping.Register(() => server.SendStop());
                _applicationLifetime.ApplicationStopped.Register(() => server.Dispose());
            }
        }
    }
}
