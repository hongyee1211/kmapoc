using System.Collections.Generic;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Services.ARM
{
    public interface IArmOperations
    {
        Task<IEnumerable<string>> EnumerateTenantsIdsAccessibleByUser(string accessToken);
    }
}
