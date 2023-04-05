using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetCoreMAUI.Models
{
    internal class EventViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public Nullable<DateTime> Date { get; set; }
        public string ConvertedDate { get; set; }

        public Color color { get; set; }
    }
}
