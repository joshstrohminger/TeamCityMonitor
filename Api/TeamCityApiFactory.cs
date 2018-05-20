using System;

namespace Api
{
    public static class TeamCityApiFactory
    {
        public static ITeamCityApi Create(string host, string buildTypeId, int offset)
        {
            if(string.IsNullOrWhiteSpace(host)) throw new ArgumentNullException(nameof(host));
            if (string.IsNullOrWhiteSpace(buildTypeId)) throw new ArgumentNullException(nameof(buildTypeId));

            switch (host.ToLower().Trim())
            {
                case "sim":
                case "simulator":
                    return new TeamCityApiSimulator(buildTypeId, offset);
                default:
                    return new TeamCityApi(host, buildTypeId);
            }
        }
    }
}
