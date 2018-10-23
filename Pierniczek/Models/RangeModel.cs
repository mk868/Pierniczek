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

        public double Min { get; set; }
        public double Max { get; set; }
    }
}
