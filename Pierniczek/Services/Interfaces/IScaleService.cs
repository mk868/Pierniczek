﻿using Pierniczek.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Services.Interfaces
{
    public interface IScaleService
    {
        IList<RowModel> ChangeRange(IList<RowModel> rows, string sourceColumn, string newColumn, double min, double max);
        IList<RowModel> Discretization(IList<RowModel> rows, string sourceColumn, string newColumn, IList<RangeModel> ranges);
    }
}