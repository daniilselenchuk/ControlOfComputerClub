using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlOfComputerClub.Model
{
    public class Client
    {
        public int ClientId { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public decimal Discount { get; }
        public decimal AmountSpent { get; set; }

        public Client()
        {
            PhoneNumber = string.Empty;
            Name = string.Empty;
            AmountSpent = 0.0m;
        }
    }
}