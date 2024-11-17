// <copyright file="AdminController.cs" company="Paul Layne">
// Copyright (c) Paul Layne. All rights reserved.
// </copyright>

namespace DualUniverse.Market.Areas.Admin.Controllers
{
    using DualUniverse.Market.Classes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using StackExchange.Exceptional;

    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    [Route("~/[area]/[action]")]
    public class AdminController : DUMPController
    {
        public AdminController(IAuthorizationService authorizationService) : base(authorizationService)
        {
        }

        [HttpGet]
        [Route("~/[area]")]
        public IActionResult Index()
        {
            return this.View();
        }

        public Task Exceptions() => ExceptionalMiddleware.HandleRequestAsync(this.HttpContext);
    }
}