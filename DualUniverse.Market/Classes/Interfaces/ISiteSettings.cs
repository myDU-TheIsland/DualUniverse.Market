// <copyright file="ISiteSettings.cs" company="Paul Layne">
// Copyright (c) Paul Layne. All rights reserved.
// </copyright>

namespace DualUniverse.Market.Classes.Interfaces
{
    using DualUniverse.Market.Classes.Settings;

    public interface ISiteSettings
    {
        string[] Admins { get; set; }

        string[] ApiKey { get; set; }

        string DPKPath { get; set; }

        DiscordSettings Discord { get; set; }

        PostgresSettings Postgres { get; set; }
    }
}
