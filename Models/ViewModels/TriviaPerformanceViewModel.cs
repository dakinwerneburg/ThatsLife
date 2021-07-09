using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ThatsLife.Models.Entity;

namespace ThatsLife.Models.ViewModels
{
    public class TriviaPerformanceViewModel
    {
        public IEnumerable<PlayerQuiz> Quizes { get; set; }

        public int TotalAttempted { get; set; }
        public int TotalCorrect { get; set; }
        public int TotalIncorrect { get; set; }
        public double Percentage { get; set; }
        public string TopCategory { get; set; }



    }
}
