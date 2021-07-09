using System;
using System.ComponentModel.DataAnnotations;

namespace ThatsLife.Models.Entity
{
    public class BaseEntity
    {
        [Required]
        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }
    }
}
