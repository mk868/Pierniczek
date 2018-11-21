﻿using Catel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Models
{
    public class RangeModel : ModelBase
    {
        public RangeModel()
        {
        }

        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
    }
}
