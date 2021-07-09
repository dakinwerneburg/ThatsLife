using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThatsLife.Models.ViewModels
{
    public class OverallChartViewModel
    {
        public TriviaOverallChart Chart { get; set; }

        public string ChartJson { get; set; }
    }
}
