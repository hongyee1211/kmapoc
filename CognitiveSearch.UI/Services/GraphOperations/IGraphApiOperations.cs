using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace CognitiveSearch.UI.Services.GraphOperations
{
    public interface IGraphApiOperations
    {
        Task<dynamic> GetUserInformation(string accessToken);
        Task<string> GetPhotoAsBase64Async(string accessToken);

        Task<IDictionary<string, string>> EnumerateTenantsIdAndNameAccessibleByUser(IEnumerable<string> tenantIds, Func<string, Task<string>> getTokenForTenant);

        Task<List<Group>> GetMyMemberOfGroupsAsync(string accessToken);
    }
}
