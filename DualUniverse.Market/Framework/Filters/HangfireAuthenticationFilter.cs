﻿// <copyright file="HangfireAuthenticationFilter.cs" company="Paul Layne">
// Copyright (c) Paul Layne. All rights reserved.
// </copyright>
namespace DualUniverse.Market.Framework.Filters
{
    using DualUniverse.Market;
    using DualUniverse.Market.Classes.Interfaces;
    using Hangfire.Dashboard;

    public class HangfireAuthenticationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            HttpContext httpContext = context.GetHttpContext();
            const string nameClaim = "nameidentifier";
            ISiteSettings siteSettings = Startup.SiteSettings;

            bool isAdmin = httpContext.User.Claims.Any(item => item.Type == nameClaim) ? siteSettings.Admins.ToList().Contains(httpContext.User.Claims.First(claim => claim.Type == nameClaim).Value) : false;

            // Allow all authenticated users to see the Dashboard (potentially dangerous).
            return httpContext.User.Identity?.IsAuthenticated ?? false;
        }
    }
}