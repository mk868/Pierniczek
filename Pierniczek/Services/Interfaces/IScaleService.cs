using Pierniczek.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Services.Interfaces
{
    public interface IScaleService
    {
        IList<RowModel> ChangeRange(IList<RowModel> rows, string sourceColumn, string newColumn, decimal min, decimal max);
        IList<RowModel> Discretization(IList<RowModel> rows, string sourceColumn, string newColumn, IList<RangeModel> ranges);
        IList<RowModel> Normalization(IList<RowModel> rows, string sourceColumn, string newColumn);
        IList<RowModel> ShowProcent(IList<RowModel> rows, string sourceColumn, int percent);
    }
}
