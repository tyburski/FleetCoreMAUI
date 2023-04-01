using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetCoreMAUI.Models
{
    internal class VehicleViewModel
    {
        public string Plate { get; set; }
        public long Mileage { get; set; }
        public string VIN { get; set; }
        public IEnumerable<Event>? Events { get; set; }
        public IEnumerable<Repair>? Repairs { get; set; }

        public bool isWarning { get; set; }
        public bool isToRepair { get; set; }
    }
}
