using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Models
{
    public class DataRecord
    {
        public int Id { get; set; }
        public string Coin { get; set; }
        public string Currency { get; set; }
        public string Time { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }
    }

}
