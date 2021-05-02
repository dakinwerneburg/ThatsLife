using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ThatsLife.Models
{
    public class PlayerCash :BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("PlayerProfile")]
        public int? ProfileId { get; set; }

        [Required]
        [Column(TypeName = "decimal(16,2)")]
        public decimal Balance { get; set; }
    }
}
