using System.Collections.Generic;
using Api.Models;
using RestSharp.Deserializers;

namespace Api.Models
{
    public class BuildCollection
    {
        [DeserializeAs(Name = "build")]
        public List<Build> Builds { get; set; }
    }
}
