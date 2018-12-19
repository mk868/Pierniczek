using Accord.Math;
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
        public string Knn(IList<RowModel> rows, string xColumn, string yColumn, string classColumn, int k, DistanceMeasureMethodEnum method, decimal xValue, decimal yValue)
        {
            List<Tuple<RowModel, decimal>> found = new List<Tuple<RowModel, decimal>>();
            //point, className, distance


            var distances = CalculateDistances(
                rows
                    .Select(s => new Tuple<decimal, decimal>((decimal)s[xColumn], (decimal)s[yColumn]))
                    .ToList(),
                new Tuple<decimal, decimal>(xValue, yValue),
                method
                );

            int i = 0;
            foreach (var row in rows)
            {
                var rowX = (decimal)row[xColumn];
                var rowY = (decimal)row[yColumn];

                decimal distance = distances[i++];
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

        public Dictionary<int, double> KnnLeaveOneOut(IList<RowModel> rows, string xColumn, string yColumn, string classColumn, int kMin, int kMax, DistanceMeasureMethodEnum method)
        {
            var result = new Dictionary<int, double>();

            for (var k = kMin; k <= kMax; k++)
            {
                var count = rows.Count;
                double found = 0;

                foreach (var row in rows)
                {
                    var rowX = (decimal)row[xColumn];
                    var rowY = (decimal)row[yColumn];
                    var rowClass = row[classColumn] as string;

                    var limitedRows = rows.Where(r => r != row).ToList();

                    var foundClass = this.Knn(limitedRows, xColumn, yColumn, classColumn, k, method, rowX, rowY);
                    if (foundClass == rowClass)
                    {
                        found++;
                    }
                }

                result[k] = 1.0 * found / count;
            }

            return result;
        }



        private List<decimal> CalculateDistances(List<Tuple<decimal, decimal>> values, Tuple<decimal, decimal> p2, DistanceMeasureMethodEnum method)
        {
            var result = new List<decimal>();

            foreach (var p1 in values)
            {
                if (method == DistanceMeasureMethodEnum.Euclidean)
                {
                    var distance = (decimal)Math.Sqrt(Math.Pow((double)p1.Item1 - (double)p2.Item1, 2) + Math.Pow((double)p1.Item2 - (double)p2.Item2, 2));
                    result.Add(distance);
                    continue;
                }

                if (method == DistanceMeasureMethodEnum.Manhattan)
                {
                    var distance = Math.Abs(p1.Item1 - p2.Item1) + Math.Abs(p1.Item2 - p2.Item2);
                    result.Add(distance);
                    continue;
                }

                if (method == DistanceMeasureMethodEnum.Infinity)
                {
                    var distance = Math.Max(Math.Abs(p1.Item1 - p2.Item1), Math.Abs(p1.Item2 - p2.Item2));
                    result.Add(distance);
                    continue;
                }

                if (method == DistanceMeasureMethodEnum.Mahalanobis)
                {
                    var distance = (decimal)Distance.Mahalanobis(new double[] { (double)p1.Item1, (double)p1.Item2 }, new double[] { (double)p2.Item1, (double)p2.Item2 }, new double[,] { });
                    result.Add(distance);
                    continue;
                }

                throw new NotImplementedException();
            }

            return result;
        }




        public IList<RowModel> KGroup(string columnX, string columnY, string newColumn, DistanceMeasureMethodEnum method, int k, IList<RowModel> rows)
        {
            var centers = new Dictionary<int, RowModel>();
            for (var i = 0; i < k; i++)
            {
                centers[i] = rows[i];
            }

            var assignedCenterPoint = new Dictionary<RowModel, int>(); //object to closest center


            bool centersChanged = true;
            while (centersChanged)
            {

                foreach (var row in rows)
                {
                    var rowX = (decimal)row[columnX];
                    var rowY = (decimal)row[columnY];

                    var distances = CalculateDistances(
                        centers.Select(s => new Tuple<decimal, decimal>((decimal)s.Value[columnX], (decimal)s.Value[columnY])).ToList(),
                        new Tuple<decimal, decimal>(rowX, rowY),
                        method
                        );

                    //find the nearest center point
                    assignedCenterPoint[row] = distances.IndexOf(distances.Min());
                }

                var lastCenters = new Dictionary<int, RowModel>(centers);
                //search for new center point
                for (var i = 0; i < k; i++)
                {
                    //the new center point will be the average of all points assigned to this central point
                    var allAssignedPoints = assignedCenterPoint.Where(w => w.Value == i).Select(s => s.Key).ToList();

                    var newX = allAssignedPoints.Select(s => (decimal)s[columnX]).Average();
                    var newY = allAssignedPoints.Select(s => (decimal)s[columnY]).Average();

                    //find new center point (closest to [newX, newY])
                    var distances = CalculateDistances(
                        allAssignedPoints.Select(s => new Tuple<decimal, decimal>((decimal)s[columnX], (decimal)s[columnY])).ToList(),
                        new Tuple<decimal, decimal>(newX, newY),
                        DistanceMeasureMethodEnum.Euclidean
                        );

                    centers[i] = allAssignedPoints[distances.IndexOf(distances.Min())];
                }

                centersChanged = false;
                for (var x = 0; x < k; x++)
                {
                    if (centers[x] != lastCenters[x])
                    {
                        centersChanged = true;
                        break;
                    }
                }

            }

            foreach (var row in assignedCenterPoint)
            {
                row.Key[newColumn] = (decimal)row.Value;
            }

            return rows;
        }
    }
}
