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

        public string[] Admins { get; set; } = Array.Empty<string>();

        public string[] ApiKey { get; set; } = Array.Empty<string>();

        public string DPKPath { get; set; } = string.Empty;

        public string StaticPath { get; set; } = string.Empty;
    }
}
