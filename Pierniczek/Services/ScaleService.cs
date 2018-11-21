using Pierniczek.Models;
using Pierniczek.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pierniczek.Views;

namespace Pierniczek.Services
{
	public class ScaleService : IScaleService
	{
		public IList<RowModel> ChangeRange(IList<RowModel> rows, string sourceColumn, string newColumn, decimal min, decimal max)
		{
			decimal realMin = rows.Min(r => (decimal)r[sourceColumn]);
            decimal realMax = rows.Max(r => (decimal)r[sourceColumn]);

			foreach (var row in rows)
			{
				var value = (decimal)row[sourceColumn];
				decimal newValue = 0;
                
			    newValue = (value - realMin) * (max - min) / realMax + min;

				row[newColumn] = newValue;
			}

			return rows;
		}

		private int? GetRange(IList<RangeModel> ranges, decimal value)
		{
			for (var i = 0; i < ranges.Count; i++)
			{
				var range = ranges[i];

                if(range.Min == null && value < range.Max)
                {
                    return i;
                }
                if(range.Max == null && value >= range.Min)
                {
                    return i;
                }
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
				var value = (decimal)row[sourceColumn];

				int? rangeNum = null;

				rangeNum = GetRange(ranges, value);

				if (rangeNum == null)
				{
					continue;
				}

				row[newColumn] = rangeNum.Value + 1;
			}

			return rows;
		}

		public IList<RowModel> Normalization(IList<RowModel> rows, string sourceColumn, string newColumn)
		{
			double sum = 0;
			foreach (var row in rows)
			{
				var value = row[sourceColumn];
				sum += Convert.ToDouble(value);
			}

			var mean = sum / rows.Count;

			sum = 0;
			foreach (var row in rows)
			{
				var value = row[sourceColumn];
				sum += Math.Pow((Convert.ToDouble(value) - mean), 2);
			}

			var variation = Math.Sqrt((double)1 / (rows.Count - 1) * sum);

			foreach (var row in rows)
			{
				var value = row[sourceColumn];

				row[newColumn] = (Convert.ToDouble(value) - mean) / variation;
			}

			return rows;
		}

		public IList<RowModel> ShowProcent(IList<RowModel> rows, string sourceColumn, int percent)
		{
			var values = new double[rows.Count];
			for (var index = 0; index < rows.Count; index++)
			{
				var row = rows[index];
				var value = row[sourceColumn];

				values[index] = Convert.ToDouble(value);
			}

			Array.Sort(values);
			var outRows = new List<RowModel>();
			for (int i = 0; i <= values.Length * percent / 100; i++)
			{
				var newRow = new RowModel { ["BOTTOM"] = values[i], ["TOP"] = values[values.Length - 1 - i] };
				outRows.Add(newRow);
			}

			return outRows;
		}
	}
}
