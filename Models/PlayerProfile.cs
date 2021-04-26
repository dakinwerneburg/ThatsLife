using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ThatsLife.Models
{
    public class PlayerProfile : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("IdentityUser")]
        public string UserId { get; set; }

        public string ProfileName { get; set; }

        public byte[] ProfileImage { get; set; }

        [Required]
        public int PrestigeScore { get; set; }

        [Required]
        
        [Column(TypeName = "decimal(8,2)")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Currency { get; set; }

    }
}
