using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace User.IRepository
{
    public interface IPersistedGrantRepositoryService
    {
        Task<int> SaveAsync(PersistedGrant model);

        Task<int> UpdateAsync(PersistedGrant model);

        Task<int> RemoveAllAsync(string subjectId, string clientId);

        Task<int> RemoveAllAsync(string subjectId, string clientId, string type);

        Task<int> RemoveAsync(string key);


        Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId);

        Task<PersistedGrant> GetAsync(string key);
    }
}
