using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetCoreMAUI.Models
{
    internal class RepairViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedAt { get; set; }

        public string FinishedBy { get; set; }

        public string FinishAt { get; set; }
    }
}
