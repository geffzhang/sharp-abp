﻿using Localization.Resources.AbpUi;
using Microsoft.Extensions.DependencyInjection;
using SharpAbp.Abp.MapTenancyManagement.Localization;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace SharpAbp.Abp.MapTenancyManagement
{
    [DependsOn(
        typeof(MapTenancyManagementApplicationContractsModule),
        typeof(AbpAspNetCoreMvcModule)
        )]
    public class MapTenancyManagementHttpApiModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => PreConfigureServicesAsync(context));
        }

        public override Task PreConfigureServicesAsync(ServiceConfigurationContext context)
        {
            PreConfigure<IMvcBuilder>(mvcBuilder =>
            {
                mvcBuilder.AddApplicationPartIfNotExists(typeof(MapTenancyManagementHttpApiModule).Assembly);
            });
            return Task.CompletedTask;
        }

        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            AsyncHelper.RunSync(() => ConfigureServicesAsync(context));
        }

        public override Task ConfigureServicesAsync(ServiceConfigurationContext context)
        {
            Configure<AbpLocalizationOptions>(options =>
            {
                options.Resources
                    .Get<MapTenancyManagementResource>()
                    .AddBaseTypes(typeof(AbpUiResource));
            });
            return Task.CompletedTask;
        }

    }
}
