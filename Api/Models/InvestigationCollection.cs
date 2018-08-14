using System.Collections.Generic;

namespace Api.Models
{
    public class InvestigationCollection
    {
        public int Count { get; set; }
        public List<Investigation> Investigation { get; set; }
    }
}
