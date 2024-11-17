// <copyright file="TestController.cs" company="Paul Layne">
// Copyright (c) Paul Layne. All rights reserved.
// </copyright>

namespace DualUniverse.Market.Areas.API.Controllers
{
    using DualUniverse.Market.Classes;
    using DualUniverse.Market.Framework.Attributes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Area("Api")]
    [Route("~/[area]/[controller]/[action]")]
    [ApiExplorerSettings(IgnoreApi = false)]
    public class TestController : DUMPController
    {
        public TestController(IAuthorizationService authorizationService) : base(authorizationService)
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return this.Json(true);
        }
    }
}
