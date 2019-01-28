using Catel.Data;
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

        public RangeModel(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public RangeModel(double? min, double? max, bool first, bool last)
        {
            if (!First)
            {
                Min = min;
            }
            if (!Last)
            {
                Max = max;
            }
        }

        public bool First { get; set; }
        public bool Last { get; set; }

        private double? _min = null;
        public double? Min
        {
            get => _min;
            set
            {
                if (First)
                {
                    return;
                }
                _min = value;
            }
        }

        private double? _max = null;
        public double? Max
        {
            get => _max;
            set
            {
                if (Last)
                {
                    return;
                }
                _max = value;
            }
        }
    }
}
