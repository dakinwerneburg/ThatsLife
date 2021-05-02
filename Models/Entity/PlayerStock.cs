using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ThatsLife.Models
{
    public class PlayerStock: BaseEntity
    {
        public int Id { get; set; }
        
        [Required]
        public string StockSymbol { get; set; }

        [ForeignKey("Profile")]
        public int ProfileId { get; set; }

        [Required]
        public int Shares { get; set; }

        [Required]
        [Column(TypeName = "decimal(16,2)")]
        public decimal PurchasePrice { get; set; }
    }
}
