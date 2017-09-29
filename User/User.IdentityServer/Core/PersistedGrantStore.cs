using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using User.IRepository;

namespace User.IdentityServer.Core
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        IPersistedGrantRepositoryService _iperService;
        private readonly ILogger _logger;
        public PersistedGrantStore(IPersistedGrantRepositoryService iperService, ILoggerFactory loggerFactory)
        {
            _iperService = iperService;
            _logger = loggerFactory.CreateLogger<PersistedGrantStore>();
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            try
            {
                return await _iperService.GetAllAsync(subjectId);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task<PersistedGrant> GetAsync(string key)
        {
            try
            {
                return await _iperService.GetAsync(key);
            }
            catch(Exception err)
            {
                throw err;
            }
        }

        public async Task RemoveAllAsync(string subjectId, string clientId)
        {
            try
            {
                await _iperService.RemoveAllAsync(subjectId, clientId);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            try
            {
                await _iperService.RemoveAllAsync(subjectId, clientId, type);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _iperService.RemoveAsync(key);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        public async Task StoreAsync(PersistedGrant grant)
        {
            try
            {
                PersistedGrant entity = await _iperService.GetAsync(grant.Key);
                if(entity==null || entity.Key==null || entity.Key.Length == 0)
                {
                    await _iperService.SaveAsync(grant);
                }
                else
                {
                    await _iperService.UpdateAsync(grant);
                }
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
                throw err;
            }
        }
    }
}
