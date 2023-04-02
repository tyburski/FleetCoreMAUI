using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetCoreMAUI.Models
{
    internal class Refueling
    {
        public int Id { get; set; }

        public User User { get; set; }

        public Vehicle Vehicle { get; set; }

        public long Mileage { get; set; }

        public double Quantity { get; set; }

        public DateTime CreatedAt { get; set; }

        public string ConvertedDate { get; set; }
    }
}
