using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
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
        public int Price { get; set; }
        public int Date { get; set; }

        public DataRecord(int id, string coin, string currency, string time, int price, int date)
        {
            Id = id;
            Coin = coin;  
            Currency = currency;
            Time = time;
            Price = price;
            Date = date;
        }
    }

}
