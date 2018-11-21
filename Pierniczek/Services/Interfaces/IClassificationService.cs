using Pierniczek.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Services.Interfaces
{
    public interface IClassificationService
    {
        string Knn(IList<RowModel> rows, string xColumn, string yColumn, string classColumn, int k, KnnMethodEnum method, decimal xValue, decimal yValue);
    }
}
