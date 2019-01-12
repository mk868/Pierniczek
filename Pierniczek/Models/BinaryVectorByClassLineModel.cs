using Catel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Models
{
    public class BinaryVectorByClassLineModel : ModelBase
    {
        public double Value { get; set; }
        public string Dimension { get; set; }
        // false:
        //  if(object.Value <= line.Value) Vector.Itemvalue = 1 else Vector.Itemvalue = 0
        //  beginning of the range
        // true:
        //  if(object.Value >= line.Value) Vector.Itemvalue = 1 else Vector.Itemvalue = 0
        //  true - end of the range
        public bool Direction { get; set; }
    }
}
