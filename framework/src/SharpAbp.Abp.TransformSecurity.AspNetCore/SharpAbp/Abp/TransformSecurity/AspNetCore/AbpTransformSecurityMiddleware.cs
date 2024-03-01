﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Middleware;
using Volo.Abp.DependencyInjection;

namespace SharpAbp.Abp.TransformSecurity.AspNetCore
{
    public class AbpTransformSecurityMiddleware : AbpMiddlewareBase, ITransientDependency
    {
        protected AbpTransformSecurityOptions Options { get; }
        protected AbpTransformSecurityAspNetCoreOptions AspNetCoreOptions { get; }
        protected IServiceProvider ServiceProvider { get; }
        public AbpTransformSecurityMiddleware(
            IOptions<AbpTransformSecurityOptions> options,
            IOptions<AbpTransformSecurityAspNetCoreOptions> aspNetCoreOptions,
            IServiceProvider serviceProvider)
        {
            Options = options.Value;
            AspNetCoreOptions = aspNetCoreOptions.Value;
            ServiceProvider = serviceProvider;
        }

        public override async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (Options.Enabled)
            {
                var transformSecurityId = context.Request.Headers[AspNetCoreOptions.TransformSecurityIdName];
                if (transformSecurityId.ToString().IsNullOrEmpty())
                {
                    throw new AbpException("Could not find transformSecurityId");
                }

                foreach (var type in AspNetCoreOptions.MiddlewareHandlers)
                {
                    var handler = ServiceProvider.GetService(type).As<IAbpTransformSecurityMiddlewareHandler>();
                    await handler.HandleAsync(context, transformSecurityId, default);
                }
            }

            await next(context);
        }
    }
}
