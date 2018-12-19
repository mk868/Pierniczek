using Catel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Models
{
    public class DataModel : ModelBase
    {
        public DataModel()
        {
            Rows = new List<RowModel>();
            Columns = new List<ColumnModel>();
        }

        public IList<RowModel> Rows { get; private set; }
        public IList<ColumnModel> Columns { get; private set; }
    }
}
