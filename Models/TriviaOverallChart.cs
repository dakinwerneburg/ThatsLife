using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThatsLife.Models
{
    public class TriviaOverallChart
    {
        public string type { get; set; }
        public bool responsive { get; set; }
        public Data data { get; set; }
    }

    public class Data
    {
        public string[] labels { get; set; }
        public Dataset[] datasets { get; set; }
    }

    public class Dataset
    {
        public string label { get; set; }
        public int[] data { get; set; }
        public string[] backgroundColor { get; set; }
    }
}
