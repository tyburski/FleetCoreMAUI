using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetCoreMAUI.Models
{
    internal class RefuelModel
    {
        public string Plate { get; set; }

        public long Mileage { get; set; }

        public double Quantity { get; set; }

        public string userId { get; set; }
    }
}
