using Catel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Models
{
    public class SingleDataModel : ModelBase
    {
        public SingleDataModel()
        {
            Columns = new List<ColumnModel>();
        }

        public RowModel Row { get; set; }
        public IList<ColumnModel> Columns { get; set; }
    }
}
