using System;
using Windows.UI;
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

        public Color GetOverallStatusColor()
        {
            switch (Status.OverallStatus)
            {
                case Interfaces.Status.Success:
                    return Setup.Success;
                case Interfaces.Status.Failure:
                    return Setup.Failure;
                case Interfaces.Status.Stale:
                    return Setup.Stale;
                case Interfaces.Status.UnderInvestigation:
                    return Setup.Investigating;
                case Interfaces.Status.ApiError:
                    return Setup.ApiError;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Status.OverallStatus));
            }
        }

    }
}
