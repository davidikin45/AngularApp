﻿using AspNetCore.ApiBase.Tasks;
using DatingApp.Data.Identity;
using DatingApp.Data.Identity.Initializers;
using DatingApp.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace DatingApp.Data
{
    public class IdentityContextInitializer : IAsyncInitializer
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IdentityContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public IdentityContextInitializer(IdentityContext context, IPasswordHasher<User> passwordHasher, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task ExecuteAsync()
        {
            if (_hostingEnvironment.IsDevelopment())
            {
                var migrationInitializer = new IdentityContextInitializerDropCreate(_passwordHasher);
                await migrationInitializer.InitializeAsync(_context);
            }
            else
            {
                var migrationInitializer = new IdentityContextInitializerMigrate(_passwordHasher);
                await migrationInitializer.InitializeAsync(_context);
            }
        }
    }
}
