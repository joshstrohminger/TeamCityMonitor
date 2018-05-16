using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Deserializers;

namespace TeamCityMonitor.Models
{
    public class BuildCollection
    {
        [DeserializeAs(Name = "build")]
        public List<Build> Builds { get; set; }
    }
}
