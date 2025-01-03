﻿// <copyright file="HangfireActivator.cs" company="Paul Layne">
// Copyright (c) Paul Layne. All rights reserved.
// </copyright>
//<auto-generated />
namespace DualUniverse.Market.Framework.Activators
{
    public class HangfireActivator : Hangfire.JobActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public HangfireActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}
