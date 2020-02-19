using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitnes.Model
{
    public class UserAppData
    {
        public string User { get; set; }
        public int Average { get; set; }
        public int BestResult { get; set; }
        public int WorstResult { get; set; }
        public int Rank { get; set; }
        public string Status { get; set; }
        public List<int> StepsPerDay { get; set; } = new List<int>();
    }
}
