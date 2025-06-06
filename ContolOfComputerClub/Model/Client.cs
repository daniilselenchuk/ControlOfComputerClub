using System;
using System.Collections.Generic;
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
        public decimal Discount { get; set; }
        public decimal AmountSpent { get; set; }
    }
}