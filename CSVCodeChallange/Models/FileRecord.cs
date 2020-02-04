using System;
using System.Collections.Generic;
using System.Text;

namespace CSVCodeChallange.Models
{
    class FileRecord
    {
        public string Date { get; set; }
        public string Type { get; set; }
        public ICollection<Order> Orders { get; set; }
        public Ender Ender { get; set; }

    }
}
