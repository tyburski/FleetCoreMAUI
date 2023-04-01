using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetCoreMAUI.Models
{
    class Repair
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public User User { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime FinishAt { get; set; }

        public User UserFinished { get; set; }

    }
}
