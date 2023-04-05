using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetCoreMAUI.Models
{
    class Event
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public Nullable<DateTime> Date { get; set; } 

    }
}
