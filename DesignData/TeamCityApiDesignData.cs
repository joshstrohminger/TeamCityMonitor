using System.Threading;
using System.Threading.Tasks;
using Api;
using Api.Models;

namespace DesignData
{
    public class TeamCityApiDesignData : ITeamCityApi
    {
        public string BuildTypeId { get; }
        public Task<BuildTypeStatusSummary> RefreshAsync()
        {
            return Task.FromResult(Refresh());
        }

        public Task<BuildTypeStatusSummary> RefreshAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Refresh());
        }

        public BuildTypeStatusSummary Refresh()
        {
            return new BuildTypeStatusSummary();
        }
    }
}
