using AspNetCore.ApiBase.ActionResults;
using AspNetCore.ApiBase.Authorization;
using AspNetCore.ApiBase.Cqrs;
using AspNetCore.ApiBase.DependencyInjection.Modules;
using AspNetCore.ApiBase.Email;
using AspNetCore.ApiBase.Extensions;
using AspNetCore.ApiBase.Filters;
using AspNetCore.ApiBase.Hangfire;
using AspNetCore.ApiBase.Hosting;
using AspNetCore.ApiBase.Localization;
using AspNetCore.ApiBase.Middleware;
using AspNetCore.ApiBase.MultiTenancy;
using AspNetCore.ApiBase.Reflection;
using AspNetCore.ApiBase.Routing;
using AspNetCore.ApiBase.Routing.Constraints;
using AspNetCore.ApiBase.Settings;
using AspNetCore.ApiBase.SignalR;
using AspNetCore.ApiBase.Swagger;
using AspNetCore.ApiBase.Tasks;
using AspNetCore.ApiBase.Validation.Providers;
using AspNetCoreRateLimit;
using Autofac;
using Hangfire;
using IdentityServer4.AccessTokenValidation;
using LeacockSite.DependencyInjection.Autofac.Modules;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace AspNetCore.ApiBase
{
    public abstract class AppStartup
    {
        public AppStartup(ILoggerFactory loggerFactory, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            Logger = loggerFactory.CreateLogger("Startup");

            Configuration = configuration;
            Configuration.PopulateStaticConnectionStrings();
            AppSettings = GetSettings<AppSettings>("AppSettings");

            HostingEnvironment = hostingEnvironment;

            BinPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            Logger.LogInformation($"Bin Folder: {BinPath}");
            PluginsPath = Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), PluginsFolder);
            Logger.LogInformation($"Plugins Folder: {PluginsPath}");
            if (!Directory.Exists(PluginsPath)) Directory.CreateDirectory(PluginsPath);
            AssemblyName = this.GetType().Assembly.GetName().Name;
            AppAssemblyPrefix = configuration.GetValue<string>("AppSettings:AssemblyPrefix");

            AssemblyBoolFilter = (a => a.FullName.Contains(AppAssemblyPrefix) || a.FullName.Contains(CommonAssemblyPrefix));
            AssemblyStringFilter = (s => (new FileInfo(s)).Name.Contains(AppAssemblyPrefix) || (new FileInfo(s)).Name.Contains(CommonAssemblyPrefix));

            //Load Assemblies into current AppDomain
            var binAssemblies = Directory.EnumerateFiles(BinPath, "*.*", SearchOption.TopDirectoryOnly)
              .Where(file => new[] { ".dll", ".exe" }.Any(file.ToLower().EndsWith))
              .Where(AssemblyStringFilter)
              .Select(Assembly.LoadFrom).ToList();

            //Load plugins into current AppDomain
            var pluginAssemblies = Directory.EnumerateFiles(PluginsPath, "*.*", SearchOption.TopDirectoryOnly)
                               .Where(file => new[] { ".dll", ".exe" }.Any(file.ToLower().EndsWith))
                               .Select(Assembly.LoadFrom).ToList();

            ApplicationParts = binAssemblies.Concat(pluginAssemblies).Where(AssemblyBoolFilter).ToList();
        }

        public Microsoft.Extensions.Logging.ILogger Logger { get; }
        public IHostingEnvironment HostingEnvironment { get; }
        public IConfiguration Configuration { get; }
        public AppSettings AppSettings { get; }

        public string BinPath { get; }
        public string PluginsPath { get; }
        public string AssemblyName { get; }
        public string CommonAssemblyPrefix { get; } = "AspNetCore.ApiBase";
        public string PluginsFolder { get; } = @"plugins\";
        public string AppAssemblyPrefix { get; }
        public Func<Assembly, Boolean> AssemblyBoolFilter { get; }
        public Func<string, Boolean> AssemblyStringFilter { get; }
        public List<Assembly> ApplicationParts { get; }
        public string AssetsFolder { get; } = "/files";

        // Add services to the collection. Don't build or return
        // any IServiceProvider or the ConfigureContainer method
        // won't get called.
        //https://stackoverflow.com/questions/40844151/when-are-net-core-dependency-injected-instances-disposed
        public virtual void ConfigureServices(IServiceCollection services)
        {
            ConfigureSettingsServices(services);
            ConfigureDatabaseServices(services);
            ConfigureAuthenticationServices(services);

            //This will override defaultauthentication
            ConfigureIdentityServices(services);

            ConfigureAuthorizationServices(services);
            ConfigureSecurityServices(services);
            ConfigureEmailServices(services);
            ConfigureCachingServices(services);
            ConfigureResponseCompressionServices(services);
            ConfigureLocalizationServices(services);
            ConfigureMvcServices(services);
            ConfigureCqrsServices(services);
            ConfigureSignalRServices(services);
            ConfigureApiServices(services);
            ConfigureHttpClients(services);

            //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.1
            AddHostedServices(services);

            AddHangfireJobServices(services);
        }

        #region Settings
        public virtual void ConfigureSettingsServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Settings");

            services.AddSingleton(Configuration);

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<AppSettings>>().Value);

            services.Configure<TokenSettings>(Configuration.GetSection("TokenSettings"));
            services.Configure<TokenSettings>(options =>
            {
                ManipulateTokenSettings(options);
            });
            services.AddTransient(sp => sp.GetService<IOptions<TokenSettings>>().Value);

            services.Configure<DisplayConventionsDisableSettings>(Configuration.GetSection("DisplayConventionsDisableSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<DisplayConventionsDisableSettings>>().Value);

            services.Configure<CORSSettings>(Configuration.GetSection("CORSSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<CORSSettings>>().Value);

            services.Configure<PasswordSettings>(Configuration.GetSection("PasswordSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<PasswordSettings>>().Value);

            services.Configure<UserSettings>(Configuration.GetSection("UserSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<UserSettings>>().Value);

            services.Configure<AuthenticationSettings>(Configuration.GetSection("AuthenticationSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<AuthenticationSettings>>().Value);

            services.Configure<AuthorizationSettings>(Configuration.GetSection("AuthorizationSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<AuthorizationSettings>>().Value);

            services.Configure<CacheSettings>(Configuration.GetSection("CacheSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<CacheSettings>>().Value);

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<EmailSettings>(options =>
            {
                ManipulateEmailSettings(options);
            });
            services.AddTransient(sp => sp.GetService<IOptions<EmailSettings>>().Value);

            services.Configure<EmailTemplates>(Configuration.GetSection("EmailTemplates"));
            services.Configure<EmailTemplates>(options =>
            {
                ManipluateEmailTemplateSettings(options);
            });
            services.AddTransient(sp => sp.GetService<IOptions<EmailTemplates>>().Value);

            services.Configure<SwitchSettings>(Configuration.GetSection("SwitchSettings"));
            services.AddTransient(sp => sp.GetService<IOptions<SwitchSettings>>().Value);

            services.Configure<AssemblyProviderOptions>(options =>
            {
                options.BinPath = BinPath;
                options.AssemblyFilter = AssemblyStringFilter;
            });

            services.AddTransient(sp => sp.GetService<IOptions<AssemblyProviderOptions>>().Value);
        }

        private TokenSettings ManipulateTokenSettings(TokenSettings options)
        {
            if (!string.IsNullOrEmpty(options.PrivateKeyPath))
            {
                options.PrivateKeyPath = HostingEnvironment.MapContentPath(options.PrivateKeyPath);
            }

            if (!string.IsNullOrEmpty(options.PublicKeyPath))
            {
                options.PublicKeyPath = HostingEnvironment.MapContentPath(options.PublicKeyPath);
            }

            if (!string.IsNullOrEmpty(options.PrivateCertificatePath))
            {
                options.PrivateCertificatePath = HostingEnvironment.MapContentPath(options.PrivateCertificatePath);
            }

            if (!string.IsNullOrEmpty(options.PublicCertificatePath))
            {
                options.PublicCertificatePath = HostingEnvironment.MapContentPath(options.PublicCertificatePath);
            }

            return options;
        }

        private void ManipluateEmailTemplateSettings(EmailTemplates options)
        {
            if (!string.IsNullOrEmpty(options.Welcome))
            {
                options.Welcome = HostingEnvironment.MapContentPath(options.Welcome);
            }

            if (!string.IsNullOrEmpty(options.ResetPassword))
            {
                options.ResetPassword = HostingEnvironment.MapContentPath(options.ResetPassword);
            }
        }

        private void ManipulateEmailSettings(EmailSettings options)
        {
            if (!options.FileSystemFolder.Contains(@":\"))
            {
                options.FileSystemFolder = Path.Combine(BinPath, options.FileSystemFolder);
            }
        }

        public TSettings GetSettings<TSettings>(string sectionKey) where TSettings : class
        {
            var settingsSection = Configuration.GetSection(sectionKey);
            return settingsSection.Get<TSettings>();
        }

        #endregion

        #region Database
        public virtual void ConfigureDatabaseServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Databases");

            var tenantsConnectionString = Configuration.GetSection("ConnectionStrings").GetChildren().Any(x => x.Key == "TenantsConnection") ? Configuration.GetConnectionString("TenantsConnection") : null;
            var identityConnectionString = Configuration.GetSection("ConnectionStrings").GetChildren().Any(x => x.Key == "IdentityConnection") ? Configuration.GetConnectionString("IdentityConnection") : null;
            var defaultConnectionString = Configuration.GetSection("ConnectionStrings").GetChildren().Any(x => x.Key == "DefaultConnection") ? Configuration.GetConnectionString("DefaultConnection") : null;
            var hangfireConnectionString = Configuration.GetSection("ConnectionStrings").GetChildren().Any(x => x.Key == "HangfireConnection") ? Configuration.GetConnectionString("HangfireConnection") : null;

            AddDatabases(services, tenantsConnectionString, identityConnectionString, hangfireConnectionString, defaultConnectionString);
            AddUnitOfWorks(services);

            services.AddHangfire(hangfireConnectionString);
        }
        #endregion

        #region Authentication
        public virtual void ConfigureAuthenticationServices(IServiceCollection services)
        {
            //https://docs.microsoft.com/en-us/aspnet/core/migration/1x-to-2x/identity-2x?view=aspnetcore-2.1#cookie-based-authentication
            //Define a default scheme in 2.0 if one of the following conditions is true:
            //You use the [Authorize] attribute or authorization policies without specifying schemes

            //IdentityConstants.ApplicationScheme = "Identity.Application"
            //CookieAuthenticationDefaults.AuthenticationScheme = "Cookies"
            //JwtBearerDefaults.AuthenticationScheme = "Bearer"

            Logger.LogInformation("Configuring Authentication");

            var appSettings = GetSettings<AppSettings>("AppSettings");
            var authenticationSettings = GetSettings<AuthenticationSettings>("AuthenticationSettings");
            var tokenSettings = ManipulateTokenSettings(GetSettings<TokenSettings>("TokenSettings"));

            if (authenticationSettings.Application.Enable)
            {
                Logger.LogInformation("Configuring Cookie Authentication");

                services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.Cookie.Name = appSettings.CookieAuthName;
                });
            }

            services.ConfigureExternalCookie(options =>
            {
                options.Cookie.Name = appSettings.CookieExternalAuthName;
            });

            var authenticationBuilder = services.AddAuthentication();

            if (authenticationSettings.JwtToken.Enable)
            {
                Logger.LogInformation($"Configuring JWT Authentication" + Environment.NewLine +
                                       $"Key:{tokenSettings.Key ?? ""}" + Environment.NewLine +
                                       $"PublicKeyPath: {tokenSettings.PublicKeyPath ?? ""}" + Environment.NewLine +
                                       $"PublicCertificatePath: {tokenSettings.PublicCertificatePath ?? ""}" + Environment.NewLine +
                                       $"ExternalIssuers: {tokenSettings.ExternalIssuers ?? ""}" + Environment.NewLine +
                                       $"LocalIssuer: {tokenSettings.LocalIssuer ?? ""}" + Environment.NewLine +
                                       $"Audiences: {tokenSettings.Audiences ?? ""}");

                services.Configure<AuthenticationOptions>(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                });

                //https://wildermuth.com/2017/08/19/Two-AuthorizationSchemes-in-ASP-NET-Core-2
                authenticationBuilder.AddJwtAuthentication(
                   tokenSettings.Key,
                   tokenSettings.PublicKeyPath,
                   tokenSettings.PublicCertificatePath,
                   tokenSettings.ExternalIssuers,
                   tokenSettings.LocalIssuer,
                   tokenSettings.Audiences);
            }

            if (authenticationSettings.OpenIdConnectJwtToken.Enable)
            {
                Logger.LogInformation("Configuring IdentityServer JWT Authentication");

                //scheme
                services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "https://localhost:44318/";
                    options.ApiName = "api";
                    options.ApiSecret = "apisecret"; //Only need this if AccessTokenType = AccessTokenType.Reference
                });
            }

            if (authenticationSettings.OpenIdConnect.Enable)
            {
                Logger.LogInformation("Configuring OpenIdConnect");

                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // keep original claim types
                services.Configure<AuthenticationOptions>(options =>
                {
                    //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-2.1&tabs=aspnetcore2x
                    //overides "Identity.Application"/IdentityConstants.ApplicationScheme set by AddIdentity
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; // Challenge scheme is how user should login if they arent already logged in.
                });

                //authetication scheme seperate to application cookie
                authenticationBuilder.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, (options) =>
                {
                    options.Cookie.Name = appSettings.CookieAuthName;
                    options.AccessDeniedPath = "Authorization/AccessDenied";
                });

                authenticationBuilder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.Authority = "https://localhost:44318";

                    //options.ResponseType = "code"; //Authorization
                    //options.ResponseType = "id_token"; //Implicit
                    //options.ResponseType = "id_token token"; //Implicit
                    options.ResponseType = "code id_token"; //Hybrid
                    //options.ResponseType = "code token"; //Hybrid
                    //options.ResponseType = "code id_token token"; //Hybrid

                    //options.CallbackPath = new PathString("...")
                    //options.SignedOutCallbackPath = new PathString("...")
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("address");
                    options.Scope.Add("roles");
                    options.Scope.Add("api");
                    options.Scope.Add("subscriptionlevel");
                    options.Scope.Add("country");
                    options.Scope.Add("offline_access");
                    options.SaveTokens = true;

                    options.ClientId = "mvc_client";
                    options.ClientSecret = "secret";
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ClaimActions.Remove("amr");
                    options.ClaimActions.DeleteClaim("sid");
                    options.ClaimActions.DeleteClaim("idp");

                    options.ClaimActions.MapUniqueJsonKey("role", "role");
                    options.ClaimActions.MapUniqueJsonKey("subscriptionlevel", "subscriptionlevel");
                    options.ClaimActions.MapUniqueJsonKey("country", "country");

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        NameClaimType = "given_name",
                        RoleClaimType = "role"
                    };
                });
            }

            if (authenticationSettings.Google.Enable)
            {
                authenticationBuilder.AddGoogle("Google", options =>
                {
                    options.ClientId = authenticationSettings.Google.ClientId;
                    options.ClientSecret = authenticationSettings.Google.ClientSecret;
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                });
            }

            if (authenticationSettings.Facebook.Enable)
            {
                authenticationBuilder.AddFacebook("Facebook", options =>
                {
                    options.ClientId = authenticationSettings.Facebook.ClientId;
                    options.ClientSecret = authenticationSettings.Facebook.ClientSecret;
                    options.SignInScheme = IdentityConstants.ExternalScheme;
                });
            }
        }
        #endregion

        #region Authorization
        public virtual void ConfigureAuthorizationServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Authorization");

            var authorizationSettings = GetSettings<AuthorizationSettings>("AuthorizationSettings");

            //Add this to controller or action using Authorize(Policy = "UserMustBeAdmin")
            //Can create custom requirements by implementing IAuthorizationRequirement and AuthorizationHandler (Needs to be added to services as scoped)
            services.AddAuthorization(options =>
            {

                //https://ondrejbalas.com/authorization-options-in-asp-net-core/
                //The default policy will only be executed on requests prior to entering protected actions such as those wrapped by an [Authorize] attribute.
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

                options.AddPolicy("UserMustBeAdmin", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser();
                    policyBuilder.RequireRole("Admin");
                    //policyBuilder.AddRequirements();
                });
            });

            //https://docs.microsoft.com/en-us/aspnet/core/security/authorization/resourcebased?view=aspnetcore-2.1&tabs=aspnetcore2x
            services.AddSingleton<IAuthorizationHandler, ResourceOwnerAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, AnonymousAuthorizationHandler>();
            //Scope name as policy
            //https://www.jerriepelser.com/blog/creating-dynamic-authorization-policies-aspnet-core/
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
        }
        #endregion

        #region Security
        public virtual void ConfigureSecurityServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Security");

            var switchSettings = GetSettings<SwitchSettings>("SwitchSettings");
            var corsSettings = GetSettings<CORSSettings>("CORSSettings");

            if (switchSettings.EnableIFramesGlobal)
            {
                services.AddAntiforgery(o => o.SuppressXFrameOptionsHeader = true);
            }

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            });

            services.ConfigureCorsAllowAnyOrigin("AllowAnyOrigin");
            services.ConfigureCorsAllowSpecificOrigin("AllowSpecificOrigin", corsSettings.Domains);
        }
        #endregion

        #region Email
        public virtual void ConfigureEmailServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Email");

            var emailSettings = GetSettings<EmailSettings>("EmailSettings");

            if (emailSettings.SendEmailsViaSmtp)
            {
                services.AddTransient<IEmailService, EmailServiceSmtp>();
            }
            else
            {
                services.AddTransient<IEmailService, EmailServiceFileSystem>();
            }
        }
        #endregion

        #region Caching
        public virtual void ConfigureCachingServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Caching");

            var appSettings = GetSettings<AppSettings>("AppSettings");
            services.AddResponseCaching(options =>
            {
                options.SizeLimit = appSettings.ResponseCacheSizeMB * 1024 * 1024; //100Mb
                options.MaximumBodySize = 64 * 1024 * 1024; //64Mb
            });

            //https://stackoverflow.com/questions/46492736/asp-net-core-2-0-http-response-caching-middleware-nothing-cached
            //Client Side Cache Time
            //services.AddHttpCacheHeaders(opt => opt.MaxAge = 600, opt => opt.MustRevalidate = true);
            services.AddHttpCacheHeaders();
        }
        #endregion

        #region Compression
        public virtual void ConfigureResponseCompressionServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Response Compression");

            //Compression
            services.AddResponseCompression(options =>
            {
                options.Providers.Add<GzipCompressionProvider>();
                //options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { ""});
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest;
            });
        }
        #endregion

        #region Localization
        public virtual void ConfigureLocalizationServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Localization");

            //https://github.com/RickStrahl/Westwind.Globalization

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            //CultureInfo.CurrentUICulture.TwoLetterISOLanguageName
            //Globalization(G11N): The process of making an app support different languages and regions.
            //Localization(L10N): The process of customizing an app for a given language and region.
            //Internationalization(I18N): Describes both globalization and localization.
            //Culture: It's a language and, optionally, a region.
            //Neutral culture: A culture that has a specified language, but not a region. (for example "en", "es")
            //                    Specific culture: A culture that has a specified language and region. (for example "en-US", "en-GB", "es-CL")
            //                Parent culture: The neutral culture that contains a specific culture. (for example, "en" is the parent culture of "en-US" and "en-GB")
            //            Locale: A locale is the same as a culture.
        }
        #endregion

        #region Mvc
        public virtual void ConfigureMvcServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Mvc");

            var appSettings = GetSettings<AppSettings>("AppSettings");
            var authorizationSettings = GetSettings<AuthorizationSettings>("AuthorizationSettings");

            // Add framework services.
            var mvc = services.AddMvc(options =>
            {
                //https://github.com/aspnet/Security/issues/1764
                options.AllowCombiningAuthorizeFilters = false;

                options.Filters.Add<ExceptionHandlingFilter>();
                options.Filters.Add<OperationCancelledExceptionFilter>();

                //options.Filters.Add(typeof(ModelValidationFilter));
                ConfigureMvcCachingProfiles(options);
                ConfigureMvcVariableResourceRepresentations(options);

                if (authorizationSettings.UserMustBeAuthorizedByDefault)
                {
                    //https://ondrejbalas.com/authorization-options-in-asp-net-core/
                    //The authorization filter will be executed on any request that enters the MVC middleware and maps to a valid action.
                    var globalPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                    options.Filters.Add(new AuthorizeFilter(globalPolicy));
                }

            })
            .AddXmlSerializerFormatters() //XML Opt out. Contract Serializer is Opt in
            .AddJsonOptions(opt =>
            {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                opt.SerializerSettings.Formatting = Formatting.Indented;
                opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            })
            .AddCookieTempDataProvider(options =>
            {
                // new API
                options.Cookie.Name = appSettings.CookieTempDataName;
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
            .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization()
            //By default, ASP.NET Core will resolve the controller parameters from the container but doesn’t actually resolve the controller from the container.
            //https://andrewlock.net/controller-activation-and-dependency-injection-in-asp-net-core-mvc/
            //The AddControllersAsServices method does two things - it registers all of the Controllers in your application with the DI container as Transient (if they haven't already been registered) and replaces the IControllerActivator registration with the ServiceBasedControllerActivator
            .AddControllersAsServices();

            services.AddSingleton<IConfigureOptions<MvcOptions>, ConfigureMvcOptions>();

            services.AddCookiePolicy(appSettings.CookieConsentName);

            //singleton
            services.AddHttpContextAccessor();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(factory =>
            {
                var actionContext = factory.GetService<IActionContextAccessor>()
                                           .ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddViewLocationExpander(appSettings.MvcImplementationFolder);

            //services.AddCustomModelMetadataProvider();
            //services.AddCustomObjectValidator();
            services.AddRequiredFieldAsterixHtmlGenerator();

            services.AddInheritanceValidationAttributeAdapterProvider();

            ConfigureMvcModelValidation(services);
            ConfigureMvcApplicationParts(mvc, services);
            ConfigureMvcRouting(services);

        }

        public virtual void ConfigureMvcCachingProfiles(MvcOptions options)
        {
            //Cache-control: no-cache = store response on client browser but recheck with server each request 
            //Cache-control: no-store = dont store response on client
            options.CacheProfiles.Add("Cache24HourNoParams", new CacheProfile()
            {
                VaryByHeader = "Accept,Accept-Language,X-Requested-With",
                //VaryByQueryKeys = "", Only used for server side caching
                Duration = 60 * 60 * 24, // 24 hour,
                Location = ResponseCacheLocation.Any,// Any = Cached on Server, Client and Proxies. Client = Client Only
                NoStore = false
            });

            options.CacheProfiles.Add("Cache24HourParams", new CacheProfile()
            {
                //IIS DynamicCompressionModule and StaticCompressionModule add the Accept-Encoding Vary header.
                VaryByHeader = "Accept,Accept-Language,X-Requested-With",
                VaryByQueryKeys = new string[] { "*" }, //Only used for server side caching
                Duration = 60 * 60 * 24, // 24 hour,
                Location = ResponseCacheLocation.Any,// Any = Cached on Server, Client and Proxies. Client = Client Only
                NoStore = false
            });
        }

        public virtual void ConfigureMvcVariableResourceRepresentations(MvcOptions options)
        {
            //Accept = Response MIME type client is able to understand.
            //Accept-Language = Response Language client is able to understand.
            //Accept-Encoding = Response Compression client is able to understand.


            //Prevents returning object representation in default format when request format isn't available
            options.ReturnHttpNotAcceptable = true;

            //Variable resource representations
            //Use RequestHeaderMatchesMediaTypeAttribute to route for Accept header. Version media types not URI!
            var jsonOutputFormatter = options.OutputFormatters
               .OfType<JsonOutputFormatter>().FirstOrDefault();

            if (jsonOutputFormatter != null)
            {

            }

            var jsonInputFormatter = options.InputFormatters
               .OfType<JsonInputFormatter>().FirstOrDefault();
            if (jsonInputFormatter != null)
            {

            }

            options.FormatterMappings.SetMediaTypeMappingForFormat(
                           "xml", "application/xml");
        }

        public virtual void ConfigureMvcRouting(IServiceCollection services)
        {
            services.Configure<RouteOptions>(options =>
            {
                options.ConstraintMap.Add("promo", typeof(PromoConstraint));
                options.ConstraintMap.Add("tokenCheck", typeof(TokenConstraint));
                options.ConstraintMap.Add("versionCheck", typeof(RouteVersionConstraint));
            });
        }

        public virtual void ConfigureMvcModelValidation(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Mvc Model Validation");

            var switchSettings = GetSettings<SwitchSettings>("SwitchSettings");

            //Disable IObjectValidatable and Validation Attributes from being evaluated and populating modelstate
            //https://stackoverflow.com/questions/46374994/correct-way-to-disable-model-validation-in-asp-net-core-2-mvc
            if (!switchSettings.EnableMVCModelValidation)
            {
                var validator = services.FirstOrDefault(s => s.ServiceType == typeof(IObjectModelValidator));
                if (validator != null)
                {
                    services.Remove(validator);
                    services.Add(new ServiceDescriptor(typeof(IObjectModelValidator), _ => new NonValidatingValidator(), ServiceLifetime.Singleton));
                }
            }
        }

        public virtual void ConfigureMvcApplicationParts(IMvcBuilder mvcBuilder, IServiceCollection services)
        {
            Logger.LogInformation("Configuring Application Parts");

            //Add Controllers from other assemblies
            foreach (var assembly in ApplicationParts)
            {
                mvcBuilder.AddApplicationPart(assembly).AddControllersAsServices();
            }

            //Add Embedded views from other assemblies
            services.Configure<RazorViewEngineOptions>(options =>
            {
                //Add Embedded Views from other assemblies
                //Edit and Continue wont work with these views.
                foreach (var assembly in ApplicationParts)
                {
                    options.FileProviders.Add(new EmbeddedFileProvider(assembly));
                }
            });
        }
        #endregion

        #region Cqrs
        public virtual void ConfigureCqrsServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Cqrs");

            services.AddCqrs(ApplicationParts);
        }
        #endregion


        #region SignalR
        public virtual void ConfigureSignalRServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring SignalR");

            services.AddSignalR();
        }
        #endregion

        #region Api
        public virtual void ConfigureApiServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Api");

            var appSettings = GetSettings<AppSettings>("AppSettings");

            //Automatic Api Validation Response when ApiController attribute is applied.
            //https://blogs.msdn.microsoft.com/webdev/2018/02/27/asp-net-core-2-1-web-apis/
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    return new UnprocessableEntityAngularObjectResult(Messages.RequestInvalid, context.ModelState);
                };
            });

            services.AddApiVersioning(option =>
            {
                option.ReportApiVersions = true;
                //Header then QueryString is consistent with how Accept header/FormatFilter works.
                option.ApiVersionReader = ApiVersionReader.Combine(new HeaderApiVersionReader("api-version"), new QueryStringApiVersionReader("api-version"));
                option.DefaultApiVersion = new ApiVersion(1, 0);
                option.AssumeDefaultVersionWhenUnspecified = true;
            });

            //API rate limiting
            services.AddMemoryCache();
            services.Configure<IpRateLimitOptions>((options) =>
            {
                options.GeneralRules = new List<RateLimitRule>()
                {
                    new RateLimitRule()
                    {
                        Endpoint = "*",
                        Limit = 3,
                        Period = "5m"
                    },
                     new RateLimitRule()
                    {
                        Endpoint = "*",
                        Limit = 2,
                        Period = "10s"
                    }
                };
            });
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();

            string xmlDocumentationFileName = AssemblyName + ".xml";
            var xmlDocumentationPath = Path.Combine(BinPath, xmlDocumentationFileName);
            services.AddSwagger(appSettings.AssemblyPrefix + " API", "", "", "", "v1", xmlDocumentationPath);
        }
        #endregion

        #region HttpClients
        //https://www.codemag.com/article/1807041/What%E2%80%99s-New-in-ASP.NET-Core-2.1
        public virtual void ConfigureHttpClients(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Http Clients");
        }
        #endregion

        #region Identity
        //https://www.codemag.com/article/1807041/What%E2%80%99s-New-in-ASP.NET-Core-2.1
        public virtual void ConfigureIdentityServices(IServiceCollection services)
        {
            Logger.LogInformation("Configuring Identity");

        }
        #endregion

        public void ConfigureContainer(ContainerBuilder builder)
        {
            Logger.LogInformation("Configuring Autofac Modules");

            builder.RegisterModule(new AutofacConventionsDependencyInjectionModule() { Paths = new string[] { BinPath, PluginsPath }, Filter = AssemblyStringFilter });
            builder.RegisterModule(new AutofacTasksModule() { Paths = new string[] { BinPath, PluginsPath }, Filter = AssemblyStringFilter });
            builder.RegisterModule(new AutofacDomainEventHandlerModule() { Paths = new string[] { BinPath, PluginsPath }, Filter = AssemblyStringFilter });
            builder.RegisterModule(new AutofacDisplayConventionsMetadataModule() { Paths = new string[] { BinPath, PluginsPath }, Filter = AssemblyStringFilter });
            builder.RegisterModule(new AutofacConventionsSignalRHubModule() { Paths = new string[] { BinPath, PluginsPath }, Filter = AssemblyStringFilter });
            builder.RegisterModule(new AutofacAutomapperModule() { Filter = AssemblyBoolFilter });

            builder.RegisterTaskRunners();
        }

        private static bool IsStreamRequest(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var stream = false;

            var filename = Path.GetFileName(context.Request.Path.ToString());
          

            return stream;
        }

        private static bool AreCookiesConsentedCallback(Microsoft.AspNetCore.Http.HttpContext context, string cookieConsentName)
        {
            return context.Request.Path.ToString().StartsWith("/api") || (context.Request.Cookies.Keys.Contains(cookieConsentName));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, AppSettings appSettings, CacheSettings cacheSettings,
            SwitchSettings switchSettings, TaskRunnerAfterApplicationConfiguration taskRunner, ISignalRHubMapper signalRHubMapper, ILoggerFactory loggerFactory)
        {
            Logger.LogInformation("Configuring Request Pipeline");

            foreach (var publicUploadFolder in appSettings.PublicUploadFolders.Split(','))
            {
                var path = env.WebRootPath + publicUploadFolder;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            if (!env.IsProduction())
            {
                app.UsePing("/ping");

                //Imortant: Must be before exception handling
                //1. download profiler from https://stackify.com/prefix/
                //2. enable .NET profiler in windows tray
                //3. access results at http://localhost:2012
                app.UseStackifyPrefix();

                // Non Api
                app.UseWhen(context => !context.Request.Path.ToString().StartsWith("/api"),
                    appBranch =>
                    {
                        appBranch.UseDeveloperExceptionPage();
                    }
               );

                // Web Api
                app.UseWhen(context => context.Request.Path.ToString().StartsWith("/api"),
                    appBranch =>
                    {
                        appBranch.UseWebApiExceptionHandler(true);
                    }
               );

               app.UseBrowserLink();
            }
            else
            {
                // Non Api
                app.UseWhen(context => !context.Request.Path.ToString().StartsWith("/api"),
                     appBranch =>
                     {
                         appBranch.UseExceptionHandler("/Error");
                     }
                );

                // Web Api
                app.UseWhen(context => context.Request.Path.ToString().StartsWith("/api"),
                    appBranch =>
                    {
                        appBranch.UseWebApiExceptionHandler(false);
                    }
               );

                if (switchSettings.EnableHsts)
                {
                    //Only ever use HSTS in production!!!!!
                    //https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-2.1&tabs=visual-studio
                    app.UseHsts();
                }
            }

            if (switchSettings.EnableRedirectHttpToHttps)
            {
                //https://docs.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-2.1&tabs=visual-studio
                //picks up port automatically
                app.UseHttpsRedirection();
            }

            if (switchSettings.EnableRedirectNonWwwToWww)
            {
                var options = new RewriteOptions();
                options.AddRedirectToWww();
                //options.AddRedirectToHttps(StatusCodes.Status307TemporaryRedirect); // Does not pick up port automatically
                app.UseRewriter(options);
            }

            if (switchSettings.EnableHelloWorld)
            {
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            }

            app.UseRequestTasks();

            if (switchSettings.EnableSwagger)
            {
                var swaggerPrefix = "api";
                var swaggerEndpoint = "/swagger/v1/swagger.json";

                if (!string.IsNullOrWhiteSpace(appSettings.SwaggerUIUsername) && !string.IsNullOrWhiteSpace(appSettings.SwaggerUIPassword))
                {
                    var swaggerSecured = new[]{
                        swaggerEndpoint,
                        $"/{swaggerPrefix}",
                        $"/{swaggerPrefix}/index.html" };

                    app.UseWhen(context => swaggerSecured.Contains(context.Request.Path.ToString()),
                     appBranch =>
                     {
                         appBranch.UseBasicAuth(appSettings.SwaggerUIUsername, appSettings.SwaggerUIPassword);
                     });
                }

                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {

                    c.SwaggerEndpoint(swaggerEndpoint, appSettings.AssemblyPrefix + " API V1");
                    c.RoutePrefix = swaggerPrefix;
                    c.DocExpansion(DocExpansion.None);
                });
            }

            //Use Response Compression Middleware when you're:
            //Unable to use the following server-based compression technologies:
            //IIS Dynamic Compression module
            //http://www.talkingdotnet.com/how-to-enable-gzip-compression-in-asp-net-core/
            // General
            //"text/plain",
            // Static files
            //"text/css",
            //"application/javascript",
            // MVC
            //"text/html",
            //"application/xml",
            //"text/xml",
            //"application/json",
            //"text/json",
            if (switchSettings.EnableResponseCompression)
            {
                //https://www.softfluent.com/blog/dev/Enabling-gzip-compression-with-ASP-NET-Core
                //Concerning performance, the middleware is about 28% slower than the IIS compression (source). Additionally, IIS or nginx has a threshold for compression to avoid compressing very small files.
                app.UseResponseCompression();
            }

            //API rate limiting
            if (switchSettings.EnableIpRateLimiting)
            {
                app.UseIpRateLimiting();
            }

            if (switchSettings.EnableCors)
            {
                if (HostingEnvironment.IsProduction())
                {
                    app.UseCors("AllowSpecificOrigin");
                }
                else
                {
                    app.UseCors("AllowAnyOrigin");
                }
            }

            if (switchSettings.EnableSignalR)
            {
                app.UseSignalR(routes =>
                {
                    routes.MapHub<NotificationHub>(appSettings.SignalRUrlPrefix + "/signalr/notifications");
                    signalRHubMapper.MapHubs(routes, appSettings.SignalRUrlPrefix);
                });
            }

            //Cache-Control:max-age=0
            //This is equivalent to clicking Refresh, which means, give me the latest copy unless I already have the latest copy.
            //Cache-Control:no-cache 
            //This is holding Shift while clicking Refresh, which means, just redo everything no matter what.

            //Should only be used for server side HTML cachcing or Read Only API. Doesn't really make sense to use Response Caching for CRUD API. 

            //Will only attempt serve AND store caching if:
            //1. Controller or Action has ResponseCache attribute with Location = ResponseCacheLocation.Any
            //2. Request method is GET OR HEAD
            //3. AND Authorization header is not included

            //Will only attempt to serve from cache if:
            //1. Request header DOES NOT contain Cache-Control: no-cache (HTTP/1.1) AND Pragma: no-cache (HTTP/1.0)
            //2. AND Request header DOES NOT contain Cache-Control: max-age=0. Postman automatically has setting 'send no-cache header' switched on. This should be switched off to test caching.
            //3. AND Request header If-None-Match != Cached ETag
            //4. AND Request header If-Modified-Since < Cached Last Modified (Time it was stored in cache)

            //Will only attempt to store in cache if:
            //1. Request header DOES NOT contain Cache-Control: no-store
            //2. AND Response header DOES NOT contain Cache-Control: no-store
            //3. AND Response header does not contain Set-Cookie
            //4. AND Response Status is 200

            //When storing
            //1. Stores all headers except Age
            //2. Stores Body
            //3. Stores Length

            //In memory cache
            //https://www.devtrends.co.uk/blog/a-guide-to-caching-in-asp.net-core
            //Unfortunately, the built-in response caching middleware makes this very difficult. 
            //Firstly, the same cache duration is used for both client and server caches. Secondly, currently there is no easy way to invalidate cache entries.
            //app.UseResponseCaching();
            //Request Header Cache-Control: max-age=0 or no-cache will bypass Response Caching. Postman automatically has setting 'send no-cache header' switched on. This should be switched off to test caching.
            if (switchSettings.EnableResponseCaching)
            {
                if (switchSettings.EnableCookieConsent)
                {
                    app.UseWhen(context => AreCookiesConsentedCallback(context, appSettings.CookieConsentName) && !IsStreamRequest(context),
                      appBranch =>
                      {
                          appBranch.UseResponseCachingCustom(); //Allows Invalidation
                      }
                    );
                }
                else
                {
                    app.UseWhen(context => !IsStreamRequest(context),
                       appBranch =>
                       {
                           appBranch.UseResponseCachingCustom(); //Allows Invalidation
                       }
                     );
                }
            }

            //Works for: GET, HEAD (efficiency, and saves bandwidth)
            //Works for: PUT, PATCH (Concurrency)s
            //This is Etags
            //Generating ETags is expensive. Putting this after response caching makes sense.
            if (switchSettings.EnableETags)
            {
                if (switchSettings.EnableCookieConsent)
                {
                    app.UseWhen(context => AreCookiesConsentedCallback(context, appSettings.CookieConsentName) && !IsStreamRequest(context),
                      appBranch =>
                      {
                          appBranch.UseHttpCacheHeaders();
                      }
                    );
                }
                else
                {
                    app.UseWhen(context => !IsStreamRequest(context),
                     appBranch =>
                     {
                         appBranch.UseHttpCacheHeaders();
                     }
                   );
                }
            }

            app.MapWhen(
               context => context.Request.Path.ToString().StartsWith(AssetsFolder),
               appBranch =>
               {
                   // ... optionally add more middleware to this branch
                   char[] seperator = { ',' };
                   List<string> publicUploadFolders = appSettings.PublicUploadFolders.Split(seperator).ToList();
                   appBranch.UseContentHandler(env, AppSettings, publicUploadFolders, cacheSettings.UploadFilesDays);
               });

            app.UseRequestLocalizationCustom(appSettings.DefaultCulture);
        
            app.UseDefaultFiles();

            //versioned files can have large cache expiry time
            app.UseVersionedStaticFiles(cacheSettings.VersionedStaticFilesDays);

            //non versioned files
            app.UseNonVersionedStaticFiles(cacheSettings.NonVersionedStaticFilesDays);

            app.UseAuthentication();

            if (switchSettings.EnableHangfire)
            {
                app.UseWhen(context => context.RequestServices.GetService<ITenantService>() == null,
                     appBranch =>
                     {
                         appBranch.UseHangfire();
                     }
                     );

                app.UseWhen(context => context.RequestServices.GetService<ITenantService>() != null,
                   appBranch =>
                   {
                       appBranch.UseHangfireDashboardMultiTenant();
                   }
                   );
            }

            if (switchSettings.EnableCookieConsent)
            {
                app.UseCookiePolicy();
            }

            app.UseMvc(routes =>
            {
                routes.AllRoutes("/all-routes");

                //Angular templates
                //First matches url pattern and then substitutes in missing values. Always need Controller and Action
                routes.MapRoute(
                    name: "Templates",
                    template: "{feature}/Template/{name}",
                    defaults: new { controller = "Template", action = "Render" }
                );
            });

            taskRunner.RunTasksAfterApplicationConfiguration();
        }

        public abstract void AddDatabases(IServiceCollection services, string tenantsConnectionString, string identityConnectionString, string hangfireConnectionString, string defaultConnectionString);
        public abstract void AddUnitOfWorks(IServiceCollection services);
        public abstract void AddHostedServices(IServiceCollection services);
        public abstract void AddHangfireJobServices(IServiceCollection services);
    }
}
