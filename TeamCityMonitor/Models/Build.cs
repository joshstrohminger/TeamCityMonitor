using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamCityMonitor.Models
{
    public class Build
    {
        public string Number { get; set; }
        public string Status { get; set; }
        public string State { get; set; }
        public string WebUrl { get; set; }
        public string StatusText { get; set; }
        public string FinishDate { get; set; }
    }
}
