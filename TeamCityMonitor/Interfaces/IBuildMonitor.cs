using Windows.UI;
using Api;
using Interfaces;

namespace TeamCityMonitor.Interfaces
{
    public interface IBuildMonitor
    {
        IBuildStatus Status { get; }
        ITeamCityApi Api { get; }
        IBuildViewModel Setup { get; }
        Color GetOverallStatusColor();
    }
}
