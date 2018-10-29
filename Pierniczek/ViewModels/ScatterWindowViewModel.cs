using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using OxyPlot;
using OxyPlot.Series;
using Pierniczek.Models;
using Pierniczek.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Pierniczek.ViewModels
{
    public class ScatterWindowViewModel : ViewModelBase
    {
        private readonly IColorService _colorService;

        public ScatterWindowViewModel(IColorService colorService)
        {
            this._colorService = colorService;
        }

        public PlotModel ScatterModel { get; set; }

        public void SetDataWithClasses(IList<RowModel> rows, string columnX, string columnY)
        {
            var tmp = new PlotModel
            {
                Title = $"Scatter plot Y: {columnY}, X: {columnX}"
            };

            var i = 0;
            var lines = rows
                .Select(s => s[columnY] as string)
                .Distinct()
                .ToDictionary(x => x, x => {
                    var color = _colorService.GetUniqueColorByName(x);
                    return new LineSeries
                    {
                        StrokeThickness = 0,
                        MarkerSize = 3,
                        MarkerStroke = OxyColor.FromRgb(color.R, color.G, color.B),
                        MarkerType = MarkerType.Diamond
                    };
                });
            
            var rowValues = rows.GroupBy(x =>
                new { xVal = (decimal)x[columnX], yClass = x[columnY] as string }
            ) //hardcore ;)
                .ToDictionary(x => new ValueTuple<decimal, string>(x.Key.xVal, x.Key.yClass), x => x.Count());

            foreach (var row in rows)
            {
                var x = (decimal)row[columnX];
                var yclass = row[columnY] as string;
                var val = rowValues[new ValueTuple<decimal, string>(x, yclass)];
                var line = lines[yclass];
                line.Points.Add(new DataPoint((double)x, val));
            }

            foreach (var line in lines)
            {
                tmp.Series.Add(line.Value);
            }
            this.ScatterModel = tmp;
        }

        public void SetData(IList<RowModel> rows, string columnX, string columnY)
        {
            var tmp = new PlotModel
            {
                Title = $"Scatter plot Y: {columnY}, X: {columnX}"
            };
            var s1 = new LineSeries
            {
                StrokeThickness = 0,
                MarkerSize = 3,
                MarkerStroke = OxyColors.ForestGreen,
                MarkerType = MarkerType.Diamond
            };

            foreach (var row in rows)
            {
                var x = (decimal)row[columnX];
                var y = (decimal)row[columnY];

                s1.Points.Add(new DataPoint((double)x, (double)y));
            }

            tmp.Series.Add(s1);
            this.ScatterModel = tmp;
        }
    }
}
