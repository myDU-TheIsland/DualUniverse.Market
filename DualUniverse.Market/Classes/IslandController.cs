// <copyright file="IslandController.cs" company="Paul Layne">
// Copyright (c) Paul Layne. All rights reserved.
// </copyright>

namespace DualUniverse.Market.Classes
{
    using DualUniverse.Market.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [ApiExplorerSettings(IgnoreApi = true)]
    public class IslandController : Controller
    {
        protected string DiscordId => this.HttpContext?.User?.Claims?.FirstOrDefault(item => item.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value ?? "0";

        protected string DiscordName => this.HttpContext?.User?.Claims?.FirstOrDefault(item => item.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value ?? "No Name";

        protected bool IsAdmin => this.AuthorizationService.AuthorizeAsync(this.HttpContext.User, "Admin").GetAwaiter().GetResult().Succeeded;

        protected bool IsUser => this.AuthorizationService.AuthorizeAsync(this.HttpContext.User, "User").GetAwaiter().GetResult().Succeeded;

        protected bool IsLoggedIn => this.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        private IAuthorizationService AuthorizationService { get; set; }

        public IslandController(IAuthorizationService authorizationService)
        {
            this.AuthorizationService = authorizationService;
        }
    }
}
