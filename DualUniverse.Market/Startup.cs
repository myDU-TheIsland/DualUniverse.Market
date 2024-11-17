﻿// <copyright file="Startup.cs" company="Paul Layne">
// Copyright (c) Paul Layne. All rights reserved.
// </copyright>

namespace DualUniverse.Market
{
    using System;
    using System.Globalization;
    using System.Security.Claims;
    using DualUniverse.Market.Classes;
    using DualUniverse.Market.Classes.Interfaces;
    using DualUniverse.Market.Framework.Activators;
    using DualUniverse.Market.Framework.Filters;
    using Hangfire;
    using Hangfire.Dashboard;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.DataProtection;
    using Microsoft.AspNetCore.Http.Features;
    using Microsoft.AspNetCore.HttpOverrides;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Logging;
    using Microsoft.OpenApi.Models;
    using StackExchange.Exceptional.Stores;

    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env">
        /// The env.
        /// </param>
        public Startup(IWebHostEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            this.Configuration = builder.Build();
            this.HostingEnvironment = env;

            // Build settings object (pulls from appsettings.*)
            SiteSettings setupSettings = new SiteSettings();
            this.Configuration.GetSection("Site").Bind(setupSettings);
            SiteSettings = setupSettings;

            Initializer.InitializeCore();
        }

        /// <summary>
        /// Gets or sets the site settings.
        /// </summary>
        /// <value>The setup settings.</value>
        public static ISiteSettings SiteSettings { get; set; } = new SiteSettings();

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        /// <summary>
        /// Gets the current HostingEnvironment.
        /// </summary>
        public IWebHostEnvironment HostingEnvironment { get; }

        public string AppName => this.HostingEnvironment.IsDevelopment() ? "DualUniverse.Market.Local" : "DualUniverse.Market";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddExceptional(settings =>
            {
                settings.DefaultStore = new PostgreSqlErrorStore(SiteSettings.Postgres.GetConnectionString(), this.AppName);
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue;
            });
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.MultipartBodyLengthLimit = int.MaxValue; // if don't set default value is: 128 MB
                options.MultipartHeadersLengthLimit = int.MaxValue;
            });
            services.AddMemoryCache();
            services.AddRouting();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/signin";
                options.LogoutPath = "/signout";
            })
            .AddDiscord(options =>
            {
                options.ClientId = SiteSettings.Discord.ClientId;
                options.ClientSecret = SiteSettings.Discord.ClientSecret;
                options.ClaimActions.MapCustomJson("urn:discord:avatar:url", user =>
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "https://cdn.discordapp.com/avatars/{0}/{1}.{2}",
                        user.GetString("id"),
                        user.GetString("avatar"),
                        (user.GetString("avatar") ?? string.Empty).StartsWith("a_") ? "gif" : "png"));
                options.Events.OnTicketReceived = ctx =>
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Role, "User"),
                    };

                    if (SiteSettings.Admins.Contains(ctx.Principal?.Claims.FirstOrDefault(item => item.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "-1"))
                    {
                        //Add claim if they are
                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    }

                    var appIdentity = new ClaimsIdentity(claims);

                    ctx.Principal?.AddIdentity(appIdentity);

                    return Task.CompletedTask;
                };
            });

            services.AddSession();

            services.AddAuthorizationBuilder()
                .AddPolicy("Admin", policy => policy.RequireRole("Admin"))
                .AddPolicy("User", policy => policy.RequireRole("User"));

            if (this.HostingEnvironment.IsDevelopment())
            {
                services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(@"./dpk"));
            }
            else
            {
                services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(SiteSettings.DPKPath));

                services.AddResponseCompression(options =>
                {
                    options.EnableForHttps = true;
                });
            }

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<AddAuthorizationHeaderOperationHeader>();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "The Island", Version = "v1" });
                c.AddSecurityDefinition(
                    "Api Key",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please insert your api key",
                        Name = "X-API-Key",
                        Type = SecuritySchemeType.ApiKey,
                    });
            });

            // settings
            services.AddSingleton(SiteSettings);
            services.AddSingleton(SiteSettings.Postgres);
            services.AddSingleton<ApiKeyAuthorizationFilter>();

            // repositories
            services.AddCoreDependencies();

            if (this.HostingEnvironment.IsDevelopment())
            {
                services.AddMvc().AddRazorRuntimeCompilation();
            }
            else
            {
                services.AddMvc();
            }

            services.AddHangfire(opts => opts
               .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
               .UseSimpleAssemblyNameTypeSerializer()
               .UseRecommendedSerializerSettings()
               .UseInMemoryStorage());

            // Add the processing server as IHostedService
            services.AddHangfireServer();
        }

        public void Configure(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            GlobalConfiguration.Configuration
                .UseActivator(new HangfireActivator(serviceProvider));

            app.UseExceptional();

            // Configure the HTTP request pipeline.
            if (!this.HostingEnvironment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (this.HostingEnvironment.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
            }
            else
            {
                app.UseResponseCompression();
            }

            app.UseHttpsRedirection();
            app.UseSession();

            // Required to serve files with no extension in the .well-known folder
            StaticFileOptions options = new StaticFileOptions()
            {
                ServeUnknownFileTypes = true,
            };

            app.UseStaticFiles(options);

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All,
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "MyArea",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseHangfireDashboard("/admin/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthenticationFilter() },
                IsReadOnlyFunc = (DashboardContext context) => true,
            });

            app.UseSwagger();
            app.UseSwaggerUI();

            var recurringJobOptions = new RecurringJobOptions()
            {
                TimeZone = TimeZoneInfo.Local,
            };

            if (!this.HostingEnvironment.IsDevelopment())
            {
                //RecurringJob.AddOrUpdate("buyStuff", (IMarketBot client) => client.BuyStuffAsync(0), Cron.Minutely, options: recurringJobOptions);
                //RecurringJob.AddOrUpdate("hotTime", (MarketService service) => service.HotTimeEvent(), "0 */3 * * *", options: recurringJobOptions);
            }
        }
    }
}