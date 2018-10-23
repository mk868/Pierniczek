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
        IList<RowModel> GroupAlphabetically(string strColumn, string newColumn, IList<RowModel> rows);
        IList<RowModel> GroupByOrder(string strColumn, string newColumn, IList<RowModel> rows);
    }
}
