using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ThatsLife.Models
{
    public class PlayerAsset : BaseEntity
    {
 
        public int Id { get; set; }

        [ForeignKey("PlayerProfile")]
        public int PlayerProfileId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Value { get; set; }

    }
}
