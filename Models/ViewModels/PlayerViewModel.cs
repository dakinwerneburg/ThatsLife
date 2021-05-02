using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThatsLife.Models.ViewModels
{
    public class PlayerViewModel
    {
        public string ProfileName { get; set; }
        public byte[] ProfileImage { get; set; }

        public decimal Cash { get; set; }

        public int Score { get; set; }

        public string Title { get; set; }
    }
}
