using System;
using System.Collections.Generic;
using System.Text;

namespace CSVCodeChallange.Models
{
    class Order
    {
        public string Date { get; set; }
        public string Code { get; set; }
        public string Number { get; set; }
        public Buyer Buyer { get; set; }
        public ICollection<Item> Items { get; set; }
        public Timing Timings { get; set; }
    }
}
