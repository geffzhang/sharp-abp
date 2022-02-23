﻿using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.EventBus.Distributed;

namespace SharpAbp.Abp.DbConnectionsManagement
{
    public class DbConnectionHandler :
        IDistributedEventHandler<EntityCreatedEto<DatabaseConnectionInfoEto>>,
        IDistributedEventHandler<EntityUpdatedEto<DatabaseConnectionInfoEto>>,
        IDistributedEventHandler<EntityDeletedEto<DatabaseConnectionInfoEto>>,
        ITransientDependency
    {
        private readonly IDatabaseConnectionInfoCacheManager _connectionInfoCacheManager;
        public DbConnectionHandler(IDatabaseConnectionInfoCacheManager connectionInfoCacheManager)
        {
            _connectionInfoCacheManager = connectionInfoCacheManager;
        }

        public async Task HandleEventAsync(EntityCreatedEto<DatabaseConnectionInfoEto> eventData)
        {
            await _connectionInfoCacheManager.UpdateCacheAsync(eventData.Entity.Id);
        }

        public async Task HandleEventAsync(EntityUpdatedEto<DatabaseConnectionInfoEto> eventData)
        {
            await _connectionInfoCacheManager.UpdateCacheAsync(eventData.Entity.Id);
        }

        public async Task HandleEventAsync(EntityDeletedEto<DatabaseConnectionInfoEto> eventData)
        {
            await _connectionInfoCacheManager.RemoveCacheAsync(eventData.Entity.Name);
        }
    }
}
