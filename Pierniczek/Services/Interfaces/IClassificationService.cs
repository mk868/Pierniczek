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
        string Knn(IList<RowModel> rows, string xColumn, string yColumn, string classColumn, int k, DistanceMeasureMethodEnum method, decimal xValue, decimal yValue);
        Dictionary<int, double> KnnLeaveOneOut(IList<RowModel> rows, string xColumn, string yColumn, string classColumn, int kMin, int kMax, DistanceMeasureMethodEnum method);
        IList<RowModel> KGroup(string columnX, string columnY, string newColumn, DistanceMeasureMethodEnum method, int k, IList<RowModel> rows);
    }
}
