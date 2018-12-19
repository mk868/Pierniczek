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
    public class KnnLeaveOneOutWindowViewModel : ViewModelBase
    {
        private readonly IColorService _colorService;
        private readonly IMessageService _messageService;
        private readonly IClassificationService _classificationService;

        public KnnLeaveOneOutWindowViewModel(IColorService colorService, IMessageService messageService, IClassificationService classificationService)
        {
            this._colorService = colorService;
            this._messageService = messageService;
            this._classificationService = classificationService;
            Methods = new List<DistanceMeasureMethodEnum>();
            Methods.Add(DistanceMeasureMethodEnum.Euclidean);
            Methods.Add(DistanceMeasureMethodEnum.Manhattan);
            Methods.Add(DistanceMeasureMethodEnum.Infinity);
            Methods.Add(DistanceMeasureMethodEnum.Mahalanobis);

            Execute = new TaskCommand(OnExecuteExecute, ExecuteCanExecute);
        }

        public PlotModel ScatterModel { get; set; }
        public IList<DistanceMeasureMethodEnum> Methods { get; set; }
        public DistanceMeasureMethodEnum? SelectedMethod { get; set; }


        public IList<RowModel> Rows { get; set; }
        public string ColumnX { get; set; }
        public string ColumnY { get; set; }
        public string ColumnClass { get; set; }

        public void PrintPlot(Dictionary<int, double> values)
        {
            var tmp = new PlotModel
            {
                Title = $"Scatter plot"
            };

            var line = new LineSeries
            {
                StrokeThickness = 0,
                MarkerSize = 3,
                MarkerStroke = OxyColors.BlueViolet,
                MarkerType = MarkerType.Diamond,
            };
            tmp.Series.Add(line);

            foreach (var val in values)
            {
                var point = new DataPoint(val.Key, val.Value);
                line.Points.Add(point);
            }

            this.ScatterModel = tmp;
        }



        private async Task OnExecuteExecute()
        {
            var result = _classificationService.KnnLeaveOneOut(Rows, ColumnX, ColumnY, ColumnClass, 1, Rows.Count - 1, SelectedMethod.Value);

            PrintPlot(result);
        }

        private bool ExecuteCanExecute()
        {
            return SelectedMethod != null;
        }

        public TaskCommand Execute { get; private set; }
    }
}
