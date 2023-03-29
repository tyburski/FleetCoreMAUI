using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetCoreMAUI.Models
{
    internal class CreateVehicleModel
    {
        public string Plate { get; set; }
        public long? Mileage { get; set; }
        public string VIN { get; set; }
    }
}
