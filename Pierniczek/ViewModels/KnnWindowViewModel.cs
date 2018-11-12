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
    public class KnnWindowViewModel : ViewModelBase
    {
        private readonly IColorService _colorService;
        private readonly IMessageService _messageService;

        public KnnWindowViewModel(IColorService colorService, IMessageService messageService)
        {
            this._colorService = colorService;
            this._messageService = messageService;
            Methods = new List<KnnMethodEnum>();
            Methods.Add(KnnMethodEnum.EuclideanDistance);
            Methods.Add(KnnMethodEnum.Manhattan);

            Search = new TaskCommand(OnSearchExecute, SearchCanExecute);
        }

        public PlotModel ScatterModel { get; set; }
        public IList<KnnMethodEnum> Methods { get; set; }
        public KnnMethodEnum? SelectedMethod { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public int K { get; set; } = 3;

        private IList<Tuple<DataPoint, string>> allPoints = new List<Tuple<DataPoint, string>>();

        public void SetData(IList<RowModel> rows, string columnX, string columnY, string columnClass)
        {
            var tmp = new PlotModel
            {
                Title = $"Scatter plot Y: {columnY}, X: {columnX}"
            };

            var lines = rows
                .Select(s => s[columnClass] as string)
                .Distinct()
                .ToDictionary(x => x, x =>
                {
                    var color = _colorService.GetUniqueColorByName(x);
                    return new LineSeries
                    {
                        StrokeThickness = 0,
                        MarkerSize = 3,
                        MarkerStroke = OxyColor.FromRgb(color.R, color.G, color.B),
                        MarkerType = MarkerType.Diamond,
                        Title = x
                    };
                });


            foreach (var row in rows)
            {
                var x = (decimal)row[columnX];
                var y = (decimal)row[columnY];
                var className = row[columnClass] as string;

                var line = lines[className];
                var point = new DataPoint((double)x, (double)y);
                line.Points.Add(point);
                allPoints.Add(new Tuple<DataPoint, string>(point, className));
            }

            foreach (var line in lines)
            {
                tmp.Series.Add(line.Value);
            }
            this.ScatterModel = tmp;
        }

        private double CalculateDistance(double x1, double y1, double x2, double y2, KnnMethodEnum method)//TODO: replace enum by class, move calculateDistance to this class
        {
            if(method == KnnMethodEnum.EuclideanDistance)
            {
                return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            }
            if (method == KnnMethodEnum.Manhattan)
            {
                return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
            }
            throw new NotImplementedException();
        }

        private async Task OnSearchExecute()
        {
            List<Tuple<DataPoint, string, double>> found = new List<Tuple<DataPoint, string, double>>();
            //point, className, distance

            foreach(var point in allPoints)
            {
                double distance = CalculateDistance(X, Y, point.Item1.X, point.Item1.Y, SelectedMethod.Value);
                found.Add(new Tuple<DataPoint, string, double>(
                    point.Item1,
                    point.Item2,
                    distance
                    ));
            }

            found = found.OrderBy(o => o.Item3).ToList();

            while(found.Count > K)
            {
                found.RemoveAt(found.Count - 1);
            }

            var result = found
                .GroupBy(g => g.Item2)
                .ToDictionary(d => d.Key, d => d.Count())
                .OrderByDescending(o => o.Value)
                .Select(s => s.Key)
                .FirstOrDefault();
            
            await _messageService.ShowInformationAsync("Class: " + result);
        }

        private bool SearchCanExecute()
        {
            return K > 0 && SelectedMethod != null;
        }

        public TaskCommand Search { get; private set; }
    }
}
