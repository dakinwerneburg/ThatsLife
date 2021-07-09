using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ThatsLife.Models.Entity;

namespace ThatsLife.Models.ViewModels
{
    public class TriviaParametersViewModel
    {
        [Required]
        [Range(1,50)]
        public int NumberOfQuestions { get; set; }
        
        [Required]
        public int CategoryId {get;set;}

        public IEnumerable<SelectListItem> Categories { get; set; }

        [Required]
        public string Difficulty { get; set; }
        public IEnumerable<SelectListItem> Difficulies { get; set; }

        [Required]
        public string QuestionType { get; set; }
        public IEnumerable<SelectListItem> TypeList { get; set; }
    }
}
