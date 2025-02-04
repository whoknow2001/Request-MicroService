﻿using Request.Domain.Base;
using System.Diagnostics.CodeAnalysis;

namespace Request.Domain.Entities.Requests
{
    public class Status : Entity
    {
        public string Name { get; set; }

        public string? Description { get; set; }
    }
}