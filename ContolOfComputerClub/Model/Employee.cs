using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlOfComputerClub.Model
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public string JobTitle { get; set; }
        public string NumberPassport { get; set; }
        public byte[]? Photo { get; set; } 
    }
}