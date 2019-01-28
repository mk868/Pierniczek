using Catel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Models
{
    public class DecisionTreeNodeModel : DecisionTreeBaseModel
    {
        public string Attribute { get; set; }
        public IList<Tuple<double, DecisionTreeBaseModel>> Paths { get; set; }

        public DecisionTreeNodeModel()
        {
            Paths = new List<Tuple<double, DecisionTreeBaseModel>>();
        }

        public override string ToString()
        {
            var rules = "";
            foreach(var path in Paths)
            {
                rules += "if " + path.Item1 + " -> " + path.Item2.Name + "; ";
            }

            return $"{Name}: Attr: {Attribute}; " + rules;
        }
    }
}
