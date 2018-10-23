using Pierniczek.Models;
using Pierniczek.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pierniczek.Services
{
    public class ScaleService : IScaleService
    {
        public IList<RowModel> ChangeRange(IList<RowModel> rows, string sourceColumn, string newColumn, double min, double max)
        {
            double realMin = 0;
            double realMax = 0;

            var firstRow = rows.First();
            if (firstRow[sourceColumn] is int)
            {
                realMin = (int)firstRow[sourceColumn];
                realMax = (int)firstRow[sourceColumn];
            }
            else
            if (firstRow[sourceColumn] is double)
            {
                realMin = (double)firstRow[sourceColumn];
                realMax = (double)firstRow[sourceColumn];
            }

            foreach (var row in rows)
            {
                var value = row[sourceColumn];
                if (value is int)
                {
                    int iValue = (int)value;
                    if (iValue < realMin) realMin = iValue;
                    if (iValue > realMax) realMax = iValue;
                }
                else
                if (value is double)
                {
                    double dValue = (double)value;
                    if (dValue < realMin) realMin = dValue;
                    if (dValue > realMax) realMax = dValue;
                }
            }


            foreach (var row in rows)
            {
                var value = row[sourceColumn];
                double newValue = 0;
                if (value is int)
                {
                    int iValue = (int)value;
                    newValue = (iValue - realMin) * (max - min) / realMax + min;
                }
                else
                if (value is double)
                {
                    double dValue = (double)value;
                    newValue = (dValue - realMin) * (max - min) / realMax + min;
                }

                row[newColumn] = newValue;
            }

            return rows;
        }

        private int? GetRange(IList<RangeModel> ranges, decimal value)
        {
            for (var i = 0; i < ranges.Count; i++)
            {
                var range = ranges[i];

                if (value >= range.Min && value < range.Max)
                {
                    return i;
                }
            }

            return null;
        }

        public IList<RowModel> Discretization(IList<RowModel> rows, string sourceColumn, string newColumn, IList<RangeModel> ranges)
        {

            foreach (var row in rows)
            {
                var value = row[sourceColumn];

                int? rangeNum = null;

                if (value is int)
                {
                    rangeNum = GetRange(ranges, (int)value);
                }
                else
                    if (value is decimal)
                {
                    rangeNum = GetRange(ranges, (decimal)value);
                }

                if (rangeNum == null)
                {
                    continue;
                }

                row[newColumn] = rangeNum.Value + 1;
            }

            return rows;
        }
    }
}
