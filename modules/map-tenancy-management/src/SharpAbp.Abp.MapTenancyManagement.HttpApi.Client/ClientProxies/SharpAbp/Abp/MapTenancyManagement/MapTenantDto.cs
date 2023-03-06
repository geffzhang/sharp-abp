// This file is automatically generated by ABP framework to use MVC Controllers from CSharp
using SharpAbp.Abp.MapTenancyManagement;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using Volo.Abp.ObjectExtending;

// ReSharper disable once CheckNamespace
namespace SharpAbp.Abp.MapTenancyManagement;

public class MapTenantDto : ExtensibleAuditedEntityDto<Guid>
{
    public string Code { get; set; }

    public Guid? TenantId { get; set; }

    public string MapCode { get; set; }
}