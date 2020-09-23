using CognitiveSearch.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CognitiveSearch.UI.Services.PowerBIEmbed
{
    public interface IEmbedService
    {
        EmbedConfig EmbedConfig { get; }

        Task<bool> EmbedReport(string userName, string roles);
    }
}
