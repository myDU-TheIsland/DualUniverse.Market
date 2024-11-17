// <copyright file="SiteSettings.cs" company="Paul Layne">
// Copyright (c) Paul Layne. All rights reserved.
// </copyright>

namespace DualUniverse.Market.Classes
{
    using DualUniverse.Market.Classes.Interfaces;
    using DualUniverse.Market.Classes.Settings;

    public class SiteSettings : ISiteSettings
    {
        public DiscordSettings Discord { get; set; } = new DiscordSettings();

        public PostgresSettings Postgres { get; set; } = new PostgresSettings();

        public string[] Admins { get; set; } = { };

        public string[] ApiKey { get; set; } = { "5108b23c-9fec-497e-88a6-07f9d3032dcc" };

        public string DPKPath { get; set; } = "/config/dpk";
    }
}
