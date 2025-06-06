using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlOfComputerClub.Model
{
    public class Workplace
    {
        public int WorkplaceId { get; set; }
        public string Status { get; set; }
        public string Tariff { get; set; }
        public decimal PriceOfWorkplace { get; set; }
        public string Configuration { get; set; }
    }
}
