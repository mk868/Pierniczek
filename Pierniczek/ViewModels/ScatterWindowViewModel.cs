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
        public ScatterWindowViewModel()
        {
            //var tmp = new PlotModel { Title = "Scatter plot", Subtitle = "Barnsley fern (IFS)" };
            //var s1 = new LineSeries
            //{
            //    StrokeThickness = 0,
            //    MarkerSize = 3,
            //    MarkerStroke = OxyColors.ForestGreen,
            //    MarkerType = MarkerType.Plus
            //};

            //var random = new Random(27);
            //double x = 0;
            //double y = 0;
            //for (int i = 0; i < 100; i++)
            //{
            //    x += 2 + random.NextDouble();
            //    y += 1 + random.NextDouble();

            //    s1.Points.Add(new DataPoint(x, y));
            //}

            //tmp.Series.Add(s1);
            //this.ScatterModel = tmp;
        }


        public PlotModel ScatterModel { get; set; }

        public void SetData(IList<RowModel> rows, string columnX, string columnY)
        {
            var tmp = new PlotModel {
                Title = "Scatter plot",
                Subtitle = "Barnsley fern (IFS)"
            };
            var s1 = new LineSeries
            {
                StrokeThickness = 0,
                MarkerSize = 3,
                MarkerStroke = OxyColors.ForestGreen,
                MarkerType = MarkerType.Diamond
            };

            foreach(var row in rows)
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
