// <copyright file="ApiKeyAttribute.cs" company="Paul Layne">
// Copyright (c) Paul Layne. All rights reserved.
// </copyright>

namespace DualUniverse.Market.Framework.Attributes
{
    using DualUniverse.Market.Framework.Filters;
    using Microsoft.AspNetCore.Mvc;

    public class ApiKeyAttribute : ServiceFilterAttribute
    {
        public ApiKeyAttribute()
            : base(typeof(ApiKeyAuthorizationFilter))
        {
        }
    }
}
