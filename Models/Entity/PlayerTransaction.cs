using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ThatsLife.Models.DAL
{
    public class PlayerTransaction :BaseEntity
    {
        public int Id { get; set; }

        [ForeignKey("PlayerProfile")]
        public int? ProfileId { get; set; }

        [ForeignKey("PlayerProfile")]
        public int? ReciepientId { get; set; }

        [Required]
        public string TransactionType { get; set; }

        public string TransactionDescription { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(16,2)")]
        public decimal Price { get; set; }


    }
}
