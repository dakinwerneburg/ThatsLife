using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThatsLife.Models
{
    public class Company
    {
            public string symbol { get; set; }
            public string companyName { get; set; }
            public string exchange { get; set; }
            public string industry { get; set; }
            public string website { get; set; }
            public string description { get; set; }
            public string CEO { get; set; }
            public string securityName { get; set; }
            public string issueType { get; set; }
            public string sector { get; set; }
            public int primarySicCode { get; set; }
            public int employees { get; set; }
            public string[] tags { get; set; }
            public string address { get; set; }
            public object address2 { get; set; }
            public string state { get; set; }
            public string city { get; set; }
            public string zip { get; set; }
            public string country { get; set; }
            public string phone { get; set; }

    }
}
