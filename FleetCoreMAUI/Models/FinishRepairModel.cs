using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetCoreMAUI.Models
{
    internal class FinishRepairModel
    {
        public int repairId { get; set; }

        public string userId { get; set; }

        public bool isBonus { get; set; }
    }
}
