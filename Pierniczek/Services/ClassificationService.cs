using Pierniczek.Models;
using Pierniczek.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Services
{
    public class ClassificationService : IClassificationService
    {
        public string Knn(IList<RowModel> rows, string xColumn, string yColumn, string classColumn, int k, KnnMethodEnum method, decimal xValue, decimal yValue)
        {
            List<Tuple<RowModel, decimal>> found = new List<Tuple<RowModel, decimal>>();
            //point, className, distance

            foreach (var row in rows)
            {
                var rowX = (decimal)row[xColumn];
                var rowY = (decimal)row[yColumn];

                decimal distance = CalculateDistance(xValue, yValue, rowX, rowY, method);
                found.Add(new Tuple<RowModel, decimal>(
                    row,
                    distance
                    ));
            }

            found = found.OrderBy(o => o.Item2).ToList();//sort by distance

            while (found.Count > k)
            {
                found.RemoveAt(found.Count - 1);
            }

            var result = found
                .GroupBy(g => (string)g.Item1[classColumn])
                .ToDictionary(d => d.Key, d => d.Count())
                .OrderByDescending(o => o.Value)
                .Select(s => s.Key)
                .FirstOrDefault();

            return result;
        }

        private decimal CalculateDistance(decimal x1, decimal y1, decimal x2, decimal y2, KnnMethodEnum method)//TODO: replace enum by class, move calculateDistance to this class
        {
            if (method == KnnMethodEnum.EuclideanDistance)
            {
                return (decimal)Math.Sqrt(Math.Pow((double)x1 - (double)x2, 2) + Math.Pow((double)y1 - (double)y2, 2));
            }
            if (method == KnnMethodEnum.Manhattan)
            {
                return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
            }
            throw new NotImplementedException();
        }
    }
}
