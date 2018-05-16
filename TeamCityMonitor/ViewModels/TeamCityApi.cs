using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using TeamCityMonitor.Models;

namespace TeamCityMonitor.ViewModels
{
    public class TeamCityApi
    {
        private readonly RestClient _client;
        private readonly RestRequest _request;

        public string BuildTypeId { get; }

        public TeamCityApi(string host, string buildTypeId)
        {
            if (string.IsNullOrWhiteSpace(host)) throw new ArgumentException(nameof(host));
            if (string.IsNullOrWhiteSpace(buildTypeId)) throw new ArgumentException(nameof(buildTypeId));
            BuildTypeId = buildTypeId;
            _client = new RestClient($"http://{host}/guestAuth/app/rest/buildTypes/id:{BuildTypeId}?fields=name,webUrl,investigations($locator(count:1),count,investigation(assignee(name))),builds($locator(defaultFilter:false,canceled:false,failedToStart:false,personal:any,count:8,state:any),build(state,number,status,statusText,webUrl,finishDate))");
            _request = new RestRequest();
            _request.AddHeader("Accept", "application/json");
        }

        public override string ToString() => BuildTypeId;

        public async Task<BuildTypeStatusSummary> RefreshAsync()
        {
            return await RefreshAsync(CancellationToken.None);
        }

        public async Task<BuildTypeStatusSummary> RefreshAsync(CancellationToken cancellationToken)
        {
            var taskCompletion = new TaskCompletionSource<IRestResponse<BuildTypeStatusSummary>>();
            var handle = _client.ExecuteAsync<BuildTypeStatusSummary>(_request, r => taskCompletion.SetResult(r));
            cancellationToken.Register(() => handle.Abort());
            var response = await taskCompletion.Task;
            return ExtractData(response);
        }

        public BuildTypeStatusSummary Refresh()
        {
            var response = _client.Execute<BuildTypeStatusSummary>(_request);
            return ExtractData(response);
        }

        private BuildTypeStatusSummary ExtractData(IRestResponse<BuildTypeStatusSummary> response)
        {
            return response.IsSuccessful ? response.Data : new BuildTypeStatusSummary { ErrorMessage = response.ErrorMessage };
        }
    }
}
