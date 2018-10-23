using Catel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Models
{
    public class ColumnModel : ModelBase
    {
        public TypeEnum Type { get; set; }
        public string Name { get; set; }
        public bool Use { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{Name} ({Type})";
        }
    }
}
