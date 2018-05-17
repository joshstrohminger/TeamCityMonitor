using System;
using Api;
using Interfaces;
using TeamCityMonitor.Interfaces;

namespace TeamCityMonitor.Models
{
    public class BuildMonitor : IBuildMonitor
    {
        public IBuildStatus Status { get; }
        public ITeamCityApi Api { get; }
        public IBuildViewModel Setup { get; }

        public BuildMonitor(IBuildStatus status, ITeamCityApi api, IBuildViewModel setup)
        {
            Status = status ?? throw new ArgumentNullException(nameof(status));
            Api = api ?? throw new ArgumentNullException(nameof(api));
            Setup = setup ?? throw new ArgumentNullException(nameof(setup));
        }

    }
}
