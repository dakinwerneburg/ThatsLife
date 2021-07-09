using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ThatsLife.Models.Entity
{
    public class PlayerPrestigeScore : BaseEntity
    {
        public int Id { get; set; }

        [ForeignKey("PlayerProfile")]
        public int? ProfileId { get; set; }
        public int Score { get; set; }
        public int PointsEarned { get; set; }
        public string Source { get; set; }
    }
}
