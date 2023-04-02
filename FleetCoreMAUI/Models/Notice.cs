﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetCoreMAUI.Models
{
    internal class Notice
    {
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ConvertedDate { get; set; }
        public User User { get; set; }
    }
}
