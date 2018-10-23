using Pierniczek.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Services.Interfaces
{
    public interface IClassService
    {
        IList<RowModel> GroupAlphabetically(ColumnModel strColumn, ColumnModel newColumn, IList<RowModel> rows);
        IList<RowModel> GroupByOrder(ColumnModel strColumn, ColumnModel newColumn, IList<RowModel> rows);
    }
}
