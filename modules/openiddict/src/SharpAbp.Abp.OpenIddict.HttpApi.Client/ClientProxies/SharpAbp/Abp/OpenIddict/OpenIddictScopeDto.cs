// This file is automatically generated by ABP framework to use MVC Controllers from CSharp
using SharpAbp.Abp.OpenIddict;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectExtending;

// ReSharper disable once CheckNamespace
namespace SharpAbp.Abp.OpenIddict;

public class OpenIddictScopeDto : ExtensibleEntityDto<Guid>
{
    public string Description { get; set; }

    public string DisplayName { get; set; }

    public string Name { get; set; }

    public Dictionary<string, JsonElement> Properties { get; set; }

    public string[] Resources { get; set; }

    public Dictionary<string, string> Descriptions { get; set; }

    public Dictionary<string, string> DisplayNames { get; set; }
}