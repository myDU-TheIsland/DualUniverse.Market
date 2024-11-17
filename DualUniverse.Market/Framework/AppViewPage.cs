// <copyright file="AppViewPage.cs" company="Paul Layne">
// Copyright (c) Paul Layne. All rights reserved.
// </copyright>

namespace DualUniverse.Market.Framework
{
    using DualUniverse.Market.Services;
    using DualUniverse.Market.Services.SQL;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.AspNetCore.Mvc.Razor.Internal;

    public abstract class AppViewPage<TModel> : RazorPage<TModel> where TModel : class
    {
        public string DiscordId => this.Context?.User?.Claims?.FirstOrDefault(item => item.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "0";

        public string DiscordName => this.Context?.User?.Claims?.FirstOrDefault(item => item.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "0";

        public bool IsAdmin => this.AuthorizationService.AuthorizeAsync(this.Context.User, "Admin").GetAwaiter().GetResult().Succeeded;

        public bool IsUser => this.AuthorizationService.AuthorizeAsync(this.Context.User, "Admin").GetAwaiter().GetResult().Succeeded;

        public bool IsLoggedIn => this.Context?.User?.Identity?.IsAuthenticated ?? false;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        [RazorInject]
        public IAuthorizationService AuthorizationService { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    }
}
