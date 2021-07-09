using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ThatsLife.Models.Entity
{
    public class Category : BaseEntity
    {
        public int Id { get; set; }
        public int TriviaDbCategoryId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
