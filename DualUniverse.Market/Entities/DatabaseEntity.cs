// <copyright file="DatabaseEntity.cs" company="Paul Layne">
// Copyright (c) Paul Layne. All rights reserved.
// </copyright>

namespace DualUniverse.Market.Entities
{
    using Dapper.Contrib.Extensions;

    public class DatabaseEntity
    {
        [Key]
        public double id { get; set; } = 0;
    }
}
