using Catel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Models
{
    public class DecisionTreeLeafModel : DecisionTreeBaseModel
    {
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Name}: Value: {Value}";
        }
    }
}
