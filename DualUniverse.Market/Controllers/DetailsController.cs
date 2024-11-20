// <copyright file="DetailsController.cs" company="Paul Layne">
// Copyright (c) Paul Layne. All rights reserved.
// </copyright>

namespace DualUniverse.Market.Controllers
{
  using System.Diagnostics;
  using Microsoft.AspNetCore.Mvc;

  public class DetailsController : Controller
  {
    public IActionResult Index()
    {
      return this.View();
    }
  }
}
