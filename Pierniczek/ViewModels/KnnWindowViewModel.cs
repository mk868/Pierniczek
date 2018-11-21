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
        private readonly IClassificationService _classificationService;

        public KnnWindowViewModel(IColorService colorService, IMessageService messageService, IClassificationService classificationService)
        {
            this._colorService = colorService;
            this._messageService = messageService;
            this._classificationService = classificationService;
            Methods = new List<KnnMethodEnum>();
            Methods.Add(KnnMethodEnum.EuclideanDistance);
            Methods.Add(KnnMethodEnum.Manhattan);

            Search = new TaskCommand(OnSearchExecute, SearchCanExecute);
        }

        public PlotModel ScatterModel { get; set; }
        public IList<KnnMethodEnum> Methods { get; set; }
        public KnnMethodEnum? SelectedMethod { get; set; }
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public int K { get; set; } = 3;

        
        public IList<RowModel> Rows { get; set; }
        public string ColumnX { get; set; }
        public string ColumnY { get; set; }
        public string ColumnClass { get; set; }

        public void Init()
        {
            var tmp = new PlotModel
            {
                Title = $"Scatter plot Y: {ColumnY}, X: {ColumnX}"
            };

            var lines = Rows
                .Select(s => s[ColumnClass] as string)
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


            foreach (var row in Rows)
            {
                var x = (decimal)row[ColumnX];
                var y = (decimal)row[ColumnY];
                var className = row[ColumnClass] as string;

                var line = lines[className];
                var point = new DataPoint((double)x, (double)y);
                line.Points.Add(point);
            }

            foreach (var line in lines)
            {
                tmp.Series.Add(line.Value);
            }
            this.ScatterModel = tmp;
        }

        

        private async Task OnSearchExecute()
        {
            var result = _classificationService.Knn(Rows, ColumnX, ColumnY, ColumnClass, K, SelectedMethod.Value, X, Y);

            await _messageService.ShowInformationAsync("Class: " + result);
        }

        private bool SearchCanExecute()
        {
            return K > 0 && SelectedMethod != null;
        }

        public TaskCommand Search { get; private set; }
    }
}
