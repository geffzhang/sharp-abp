﻿using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.IdentityServer.ApiResources;

namespace SharpAbp.Abp.IdentityServer.ApiResources
{
    [Authorize(IdentityServerPermissions.ApiResources.Default)]
    public class ApiResourceAppService : IdentityServerAppServiceBase, IApiResourceAppService
    {
        protected IApiResourceRepository ApiResourceRepository { get; }
        public ApiResourceAppService(
            IApiResourceRepository apiResourceRepository)
        {
            ApiResourceRepository = apiResourceRepository;
        }

        /// <summary>
        /// Get apiResource
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.ApiResources.Default)]
        public virtual async Task<ApiResourceDto> GetAsync(Guid id)
        {
            var apiResource = await ApiResourceRepository.GetAsync(id);
            return ObjectMapper.Map<ApiResource, ApiResourceDto>(apiResource);
        }

        /// <summary>
        /// Find by apiSourceName
        /// </summary>
        /// <param name="apiResourceName"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.ApiResources.Default)]
        public virtual async Task<ApiResourceDto> FindByNameAsync(string apiResourceName)
        {
            var apiResource = await ApiResourceRepository.FindByNameAsync(apiResourceName);
            return ObjectMapper.Map<ApiResource, ApiResourceDto>(apiResource);
        }

        /// <summary>
        /// Get paged list
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.ApiResources.Default)]
        public virtual async Task<PagedResultDto<ApiResourceDto>> GetPagedListAsync(ApiResourcePagedRequestDto input)
        {
            var count = await ApiResourceRepository.GetCountAsync(input.Filter);
            var apiResources = await ApiResourceRepository.GetListAsync(
                input.Sorting,
                input.SkipCount,
                input.MaxResultCount,
                input.Filter);

            return new PagedResultDto<ApiResourceDto>(
              count,
              ObjectMapper.Map<List<ApiResource>, List<ApiResourceDto>>(apiResources)
              );
        }

        /// <summary>
        /// Get list by scopeNames
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.ApiResources.Default)]
        public virtual async Task<List<ApiResourceDto>> GetListByScopesAsync(string[] scopeNames)
        {
            var apiResources = await ApiResourceRepository.GetListByScopesAsync(scopeNames);
            return ObjectMapper.Map<List<ApiResource>, List<ApiResourceDto>>(apiResources);
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.ApiResources.Create)]
        public virtual async Task<Guid> CreateAsync(CreateApiResourceDto input)
        {
            await CheckNameExistAsync(input.Name);

            var apiResource = new ApiResource(
                GuidGenerator.Create(),
                input.Name,
                input.DisplayName,
                input.Description)
            {
                Enabled = input.Enabled,
                AllowedAccessTokenSigningAlgorithms = input.AllowedAccessTokenSigningAlgorithms
            };

            //Add self
            apiResource.AddScope(input.Name);

            foreach (var userClaim in input.UserClaims)
            {
                apiResource.AddUserClaim(userClaim.Type);
            }

            await ApiResourceRepository.InsertAsync(apiResource);
            return apiResource.Id;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.ApiResources.Update)]
        public virtual async Task UpdateAsync(Guid id, UpdateApiResourceDto input)
        {
            await CheckNameExistAsync(input.Name, id);

            var apiResource = await ApiResourceRepository.GetAsync(id);
            apiResource.DisplayName = input.DisplayName;
            apiResource.Description = input.Description;
            apiResource.Enabled = input.Enabled;
            apiResource.AllowedAccessTokenSigningAlgorithms = input.AllowedAccessTokenSigningAlgorithms;

            //scope 

            //user claim
            foreach (var createClaim in input.UserClaims)
            {
                var apiResourceClaim = apiResource.FindClaim(createClaim.Type);
                if (apiResourceClaim == null)
                {
                    apiResource.AddUserClaim(createClaim.Type);
                }
            }

            var removeClaimTypes = apiResource.UserClaims.Select(x => x.Type)
                .Except(input.UserClaims.Select(y => y.Type))
                .ToList();

            foreach (var claimType in removeClaimTypes)
            {
                apiResource.RemoveClaim(claimType);
            }

            //secret
            //TODO x509
            foreach (var createSecret in input.Secrets)
            {
                var secret = apiResource.FindSecret(createSecret.Value, createSecret.Type);
                if (secret == null)
                {
                    apiResource.AddSecret(createSecret.Value, createSecret.Expiration, createSecret.Type, createSecret.Description);
                }
            }

            var removeSecrets = apiResource.Secrets.Select(x => (x.Value, x.Type))
                .Except(input.Secrets.Select(y => (y.Value, y.Type)))
                .ToList();

            foreach (var secret in removeSecrets)
            {
                apiResource.RemoveSecret(secret.Value, secret.Type);
            }

            //properties
            foreach (var createProperty in input.Properties)
            {
                var property = apiResource.FindProperty(createProperty.Key);
                if (property == null)
                {
                    apiResource.AddProperty(createProperty.Key, createProperty.Value);
                }
            }

            var removePropertyKeys = apiResource.Properties.Select(x => x.Key)
                .Except(input.Properties.Select(y => y.Key))
                .ToList();
            foreach (var propertyKey in removePropertyKeys)
            {
                apiResource.RemoveProperty(propertyKey);
            }

            await ApiResourceRepository.UpdateAsync(apiResource);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(IdentityServerPermissions.ApiResources.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await ApiResourceRepository.DeleteAsync(id);
        }


        protected virtual async Task CheckNameExistAsync(string name, Guid? expectedId = null)
        {
            if (await ApiResourceRepository.CheckNameExistAsync(name, expectedId))
            {
                throw new UserFriendlyException(L["IdentityServer.DumplicateApiResourceName", "name"]);
            }
        }


    }
}