using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ThatsLife.Models.Entity
{
    public class PlayerQuiz : BaseEntity
    {
        public int Id { get; set; }
        public PlayerProfile Player { get; set; }
        public Category Category { get; set; }

        public Difficulty Difficulty { get; set; }

        public QuestionType QuestionType { get; set; }

        public int Attempted { get; set; }
        public int Correct { get; set; }


    }
}
