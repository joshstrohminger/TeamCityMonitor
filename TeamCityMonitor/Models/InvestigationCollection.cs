using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCityMonitor.Models
{
    public class InvestigationCollection
    {
        public int Count { get; set; }
        public List<Investigation> Investigations { get; set; }
    }
}
