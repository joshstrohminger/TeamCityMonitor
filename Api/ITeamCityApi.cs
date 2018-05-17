using System.Threading;
using System.Threading.Tasks;
using Api.Models;

namespace Api
{
    public interface ITeamCityApi
    {
        string BuildTypeId { get; }
        Task<BuildTypeStatusSummary> RefreshAsync();
        Task<BuildTypeStatusSummary> RefreshAsync(CancellationToken cancellationToken);
        BuildTypeStatusSummary Refresh();
    }
}