﻿using AspNetCore.ApiBase.Helpers;
using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.MemoryStorage;
using Hangfire.Server;
using Hangfire.SQLite;
using Hangfire.SqlServer;
using Hangfire.States;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;

namespace AspNetCore.ApiBase.Hangfire
{
    public static class HangfireHelper
    {
        public static (BackgroundJobServer server, IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient) StartHangfireServer(
            string serverName,
            string connectionString,
            IApplicationLifetime applicationLifetime,
            IJobFilterProvider jobFilters,
            JobActivator jobActivator,
            IBackgroundJobFactory backgroundJobFactory,
            IBackgroundJobPerformer backgroundJobPerformer,
            IBackgroundJobStateChanger backgroundJobStateChanger,
            IBackgroundProcess[] additionalProcesses
            )
        {
            var options = new BackgroundJobServerOptions
            {
                ServerName = serverName,
                Queues = new string[] { "default" }
            };

            return StartHangfireServer(
                  options,
                 connectionString,
                 applicationLifetime,
                 jobFilters,
                 jobActivator,
                backgroundJobFactory,
                backgroundJobPerformer,
                backgroundJobStateChanger,
                additionalProcesses
                );
        }

        public static (BackgroundJobServer server, IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient) StartHangfireServer(
            BackgroundJobServerOptions options,
            string connectionString,
            IApplicationLifetime applicationLifetime,
            IJobFilterProvider jobFilters,
            JobActivator jobActivator,
            IBackgroundJobFactory backgroundJobFactory,
            IBackgroundJobPerformer backgroundJobPerformer,
            IBackgroundJobStateChanger backgroundJobStateChanger,
            IBackgroundProcess[] additionalProcesses
            )
        {
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

            var server = new BackgroundJobServer(options, storage, additionalProcesses,
                options.FilterProvider ?? jobFilters,
                options.Activator ?? jobActivator,
               backgroundJobFactory,
                backgroundJobPerformer,
                backgroundJobStateChanger);

            applicationLifetime.ApplicationStopping.Register(() => server.SendStop());
            applicationLifetime.ApplicationStopped.Register(() => server.Dispose());

            var recurringJobManager = new RecurringJobManager(storage, backgroundJobFactory);

            var backgroundJobClient = new BackgroundJobClient(storage, backgroundJobFactory, backgroundJobStateChanger);

            return (server, recurringJobManager, backgroundJobClient);
        }

        public static (BackgroundJobServer server, IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient) StartHangfireServerInMemory(string serverName)
        {
            return StartHangfireServer(serverName, "");
        }

        public static (BackgroundJobServer server, IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient) StartHangfireServer(string serverName, string connectionString)
        {
            var options = new BackgroundJobServerOptions
            {
                ServerName = serverName,
                Queues = new string[] { "default" }
            };
            return StartHangfireServer(options, connectionString);
        }

        public static (BackgroundJobServer server, IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient) StartHangfireServer(BackgroundJobServerOptions options, string connectionString)
        {
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

            var filterProvider = JobFilterProviders.Providers;
            var activator = JobActivator.Current;

            var backgroundJobFactory = new BackgroundJobFactory(filterProvider);
            var performer = new BackgroundJobPerformer(filterProvider, activator);
            var backgroundJobStateChanger = new BackgroundJobStateChanger(filterProvider);
            IEnumerable<IBackgroundProcess> additionalProcesses = null;

            var server = new BackgroundJobServer(options, storage, additionalProcesses,
                options.FilterProvider ?? filterProvider,
                options.Activator ?? activator,
                backgroundJobFactory,
                performer,
                backgroundJobStateChanger);

            var recurringJobManager = new RecurringJobManager(storage, backgroundJobFactory);

            var backgroundJobClient = new BackgroundJobClient(storage, backgroundJobFactory, backgroundJobStateChanger);

            return (server, recurringJobManager, backgroundJobClient);
        }
    }
}