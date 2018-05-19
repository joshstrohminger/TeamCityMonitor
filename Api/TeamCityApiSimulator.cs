using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Api.Models;

namespace Api
{
    public class TeamCityApiSimulator : ITeamCityApi
    {
        private int _offset;

        private readonly BuildTypeStatusSummary[] _statuses;

        public string BuildTypeId { get; }

        public TeamCityApiSimulator(string host, string buildTypeId, int offset)
        {
            _offset = offset;
            BuildTypeId = buildTypeId;

            _statuses = new []
            {
                new BuildTypeStatusSummary
                {
                    Builds = new BuildCollection{Builds = new List<Build>{new Build{StatusText = "One - Success", Status = "SUCCESS", State = "finished", FinishDate = DateTime.Now.ToString("yyyyMMdd'T'HHmmsszzzz") }}}
                },
                new BuildTypeStatusSummary
                {
                    Builds = new BuildCollection{Builds = new List<Build>
                    {
                        new Build{StatusText = "Two - Queued", Status = "SUCCESS", State = "finished", FinishDate = DateTime.Now.ToString("yyyyMMdd'T'HHmmsszzzz") },
                        new Build{State = "queued", FinishDate = DateTime.Now.ToString("yyyyMMdd'T'HHmmsszzzz")}
                    }}
                },
                new BuildTypeStatusSummary
                {
                    Builds = new BuildCollection{Builds = new List<Build>
                    {
                        new Build{StatusText = "Three - Running", Status = "SUCCESS", State = "finished", FinishDate = DateTime.Now.ToString("yyyyMMdd'T'HHmmsszzzz") },
                        new Build{StatusText = "Three - Running", Status = "SUCCESS", State = "running", WebUrl = "a"},
                    }}
                },
                new BuildTypeStatusSummary
                {
                    Builds = new BuildCollection{Builds = new List<Build>
                    {
                        new Build{StatusText = "Four - Both", Status = "SUCCESS", State = "finished", FinishDate = DateTime.Now.ToString("yyyyMMdd'T'HHmmsszzzz") },
                        new Build{StatusText = "Four - Both", Status = "SUCCESS", State = "running", WebUrl = "a"},
                        new Build{StatusText = "Four - Both", Status = "SUCCESS", State = "queued"},
                    }}
                },
                new BuildTypeStatusSummary
                {
                    Builds = new BuildCollection{Builds = new List<Build>{new Build{StatusText = "Five - Stale", Status = "SUCCESS", State = "finished", FinishDate = DateTime.Now.AddDays(-1).ToString("yyyyMMdd'T'HHmmsszzzz") }}}
                },
                new BuildTypeStatusSummary
                {
                    Builds = new BuildCollection{Builds = new List<Build>{new Build{StatusText = "Six - Failed", Status = "FAILED", State = "finished", FinishDate = DateTime.Now.AddHours(-1).ToString("yyyyMMdd'T'HHmmsszzzz") }}}
                },
                new BuildTypeStatusSummary
                {
                    Builds = new BuildCollection{Builds = new List<Build>{new Build{StatusText = "Seven - Investigating", Status = "FAILED", State = "finished", FinishDate = DateTime.Now.AddHours(-1).ToString("yyyyMMdd'T'HHmmsszzzz") }}},
                    Investigations = new InvestigationCollection{Count = 1, Investigations = new List<Investigation>{new Investigation{Assignee = new Assignee{Name = "Someone's name"}}}}
                },
                new BuildTypeStatusSummary
                {
                    ErrorMessage = "something went wrong"
                }
            };
        }

        public async Task<BuildTypeStatusSummary> RefreshAsync()
        {
            return await Task.FromResult(Refresh());
        }

        public async Task<BuildTypeStatusSummary> RefreshAsync(CancellationToken cancellationToken)
        {
            return await Task.FromResult(Refresh());
        }

        public BuildTypeStatusSummary Refresh()
        {
            return _statuses[(_offset++/2) % _statuses.Length];
        }
    }
}
