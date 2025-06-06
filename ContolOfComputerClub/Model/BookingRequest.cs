using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlOfComputerClub.Model
{
    public class BookingRequest
    {
        public int BookingRequestId { get; set; }
        public int EmployeeId { get; set; }
        public int WorkplaceId { get; set; }
        public int ClientId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string RequestStatus { get; set; }
    }
}