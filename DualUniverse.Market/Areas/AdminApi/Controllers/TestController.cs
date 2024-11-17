// <copyright file="TestController.cs" company="Paul Layne">
// Copyright (c) Paul Layne. All rights reserved.
// </copyright>

namespace DualUniverse.Market.Areas.AdminApi.Controllers
{
    using DualUniverse.Market.Classes;
    using DualUniverse.Market.Framework.Attributes;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Area("AdminApi")]
    [Route("~/admin/api/[controller]/[action]")]
    [Authorize(Policy = "Admin")]
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
